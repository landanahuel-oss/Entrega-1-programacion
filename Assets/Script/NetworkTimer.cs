using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI; // Necesario para controlar los botones
using UnityEngine.SceneManagement; // Necesario para saber el nombre de la escena actual
using TMPro;

public class NetworkTimer : NetworkBehaviour
{
    [Header("UI Config")]
    [SerializeField] private TextMeshProUGUI textoRelojUI;
    [SerializeField] private TextMeshProUGUI textoGanadorUI;

    [Header("Botones Finales")]
    [SerializeField] private Button botonReiniciar;
    [SerializeField] private Button botonSalir;

    [Header("Configuración del Tiempo")]
    [SerializeField] private float tiempoInicial = 60f;

    private NetworkVariable<float> tiempoRestante = new NetworkVariable<float>();
    private bool juegoTerminado = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            tiempoRestante.Value = tiempoInicial;
        }

        tiempoRestante.OnValueChanged += ActualizarTextoReloj;
        MostrarTiempoEnPantalla(tiempoRestante.Value);

        // Asegurar que todo el menú final empiece oculto
        if (textoGanadorUI != null) textoGanadorUI.gameObject.SetActive(false);
        if (botonReiniciar != null) botonReiniciar.gameObject.SetActive(false);
        if (botonSalir != null) botonSalir.gameObject.SetActive(false);

        // Asignar funciones a los botones
        if (botonReiniciar != null) botonReiniciar.onClick.AddListener(ReiniciarPartida);
        if (botonSalir != null) botonSalir.onClick.AddListener(SalirDelJuego);
    }

    public override void OnNetworkDespawn()
    {
        tiempoRestante.OnValueChanged -= ActualizarTextoReloj;
    }

    void Update()
    {
        if (!IsServer || juegoTerminado) return;

        if (tiempoRestante.Value > 0)
        {
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
        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        textoRelojUI.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    private void TerminarJuego()
    {
        juegoTerminado = true;

        string mensajeResultado = "¡Empate!";
        int mayorPuntaje = -1;
        ulong idGanador = 0;
        bool hayEmpate = true;

        PuntajeJugador[] todosLosJugadores = FindObjectsByType<PuntajeJugador>(FindObjectsSortMode.None);

        foreach (PuntajeJugador jugador in todosLosJugadores)
        {
            int puntosActuales = jugador.monedasEnMeta.Value;

            if (puntosActuales > mayorPuntaje)
            {
                mayorPuntaje = puntosActuales;
                idGanador = jugador.OwnerClientId;
                hayEmpate = false;
            }
            else if (puntosActuales == mayorPuntaje)
            {
                hayEmpate = true;
            }
        }

        if (!hayEmpate && mayorPuntaje > 0)
        {
            mensajeResultado = idGanador == 0 ? "¡Ganó el Jugador 1 (Host)!" : $"¡Ganó el Jugador {idGanador + 1}!";
        }
        else if (mayorPuntaje == 0 && !hayEmpate)
        {
            mensajeResultado = "Nadie juntó monedas. ¡Empate!";
        }

        NotificarFinJuegoClientRpc(mensajeResultado);
    }

    [ClientRpc]
    private void NotificarFinJuegoClientRpc(string resultado)
    {
        if (textoRelojUI != null)
        {
            textoRelojUI.text = "00:00";
            textoRelojUI.color = Color.gray;
        }

        if (textoGanadorUI != null)
        {
            textoGanadorUI.gameObject.SetActive(true);
            textoGanadorUI.text = resultado;
        }

        if (botonSalir != null) botonSalir.gameObject.SetActive(true);

        if (botonReiniciar != null)
        {
            botonReiniciar.gameObject.SetActive(true);
            if (!IsServer)
            {
                botonReiniciar.interactable = false;
                botonReiniciar.GetComponentInChildren<TextMeshProUGUI>().text = "Esperando al Host...";
            }
        }

        // =================================================================
        // ELIMINADO: Ya no apagamos el ControlJugador aquí. 
        // Los jugadores se moverán libremente en el Game Over.
        // =================================================================
    }

    // --- LÓGICA DE LOS BOTONES ---

    private void ReiniciarPartida()
    {
        if (!IsServer) return;

        Debug.Log("Reiniciando partida en red...");

        // Buscamos todos los jugadores existentes en el servidor y les reactivamos el script
        controlJugador[] todosLosJugadores = FindObjectsByType<controlJugador>(FindObjectsSortMode.None);
        foreach (controlJugador jugador in todosLosJugadores)
        {
            jugador.enabled = true;
        }

        // Cargamos la escena de forma síncrona para toda la red
        string nombreEscenaActual = SceneManager.GetActiveScene().name;
        NetworkManager.Singleton.SceneManager.LoadScene(nombreEscenaActual, LoadSceneMode.Single);
    }

    private void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");

        // 1. Desconectamos el NetworkManager limpiamente de la red
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // 2. Cerramos la aplicación
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Si estamos en el editor, detiene el Play
#else
        Application.Quit(); // Si es el juego compilado, cierra la ventana (.exe)
#endif
    }
}