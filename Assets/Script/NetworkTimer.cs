using Unity.Netcode;
using UnityEngine;
using TMPro;

public class NetworkTimer : NetworkBehaviour
{
    [Header("UI Config")]
    [SerializeField] private TextMeshProUGUI textoRelojUI;

    [Header("Configuración del Tiempo")]
    [SerializeField] private float tiempoInicial = 60f;

    // NetworkVariable para sincronizar el tiempo flotante. 
    // Solo el servidor escribe, todos leen.
    private NetworkVariable<float> tiempoRestante = new NetworkVariable<float>();
    private bool juegoTerminado = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            tiempoRestante.Value = tiempoInicial;
        }

        // Nos suscribimos al cambio de tiempo para actualizar la UI en todos lados
        tiempoRestante.OnValueChanged += ActualizarTextoReloj;

        // Inicializar el texto al spawnear
        MostrarTiempoEnPantalla(tiempoRestante.Value);
    }

    public override void OnNetworkDespawn()
    {
        tiempoRestante.OnValueChanged -= ActualizarTextoReloj;
    }

    void Update()
    {
        // REGLA: Solo el servidor resta el tiempo
        if (!IsServer || juegoTerminado) return;

        if (tiempoRestante.Value > 0)
        {
            // Restamos el tiempo pasado en este frame
            tiempoRestante.Value -= Time.deltaTime;

            if (tiempoRestante.Value <= 0)
            {
                tiempoRestante.Value = 0;
                TerminarJuego();
            }
        }
    }

    private void ActualizarTextoReloj(float valorAntiguo, float valorNuevo)
    {
        MostrarTiempoEnPantalla(valorNuevo);
    }

    private void MostrarTiempoEnPantalla(float tiempo)
    {
        if (textoRelojUI == null) return;

        // Formateamos el tiempo para que muestre minutos y segundos (00:00)
        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        textoRelojUI.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    private void TerminarJuego()
    {
        juegoTerminado = true;
        Debug.Log("¡El tiempo se ha agotado! Fin de la partida.");

        // Enviamos un RPC a todos los clientes para avisarles que el juego terminó
        NotificarFinJuegoClientRpc();
    }

    [ClientRpc]
    private void NotificarFinJuegoClientRpc()
    {
        // Aquí pones lo que quieres que pase en las pantallas de todos cuando acabe el tiempo
        if (textoRelojUI != null)
        {
            textoRelojUI.text = "¡FIN DEL JUEGO!";
            textoRelojUI.color = Color.red;
        }

        // Bloquear el movimiento de los jugadores locales
        var localPlayer = NetworkManager.Singleton.LocalClient?.PlayerObject?.GetComponent<controlJugador>();
        if (localPlayer != null)
        {
            localPlayer.enabled = false; // Apagamos su script de movimiento
        }

        // TODO: Aquí podrías activar un panel que compare los puntajes de PuntajeJugador 
        // para ver quién juntó más monedas en la meta y declarar un ganador.
    }
}