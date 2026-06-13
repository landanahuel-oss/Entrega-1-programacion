using Unity.Netcode;
using UnityEngine;
using TMPro; // 1. Importamos la librería de texto

public class puntajeJugador : NetworkBehaviour
{
    [Header("UI Config")]
    [SerializeField] private GameObject TextoPuntajePrefab; // El prefab del texto que creamos
    private TextMeshProUGUI textoPuntajeInstancia; // La copia del texto en nuestra pantalla

    // Nuestra variable de red para las monedas
    public NetworkVariable<int> monedasRecolectadas = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Se ejecuta automáticamente cuando el objeto aparece en la red
    public override void OnNetworkSpawn()
    {
        // REGLA: Solo creamos el marcador en pantalla si ESTE es MI jugador
        if (IsOwner)
        {
            // Buscamos el Canvas de la escena para meter el texto dentro
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                // Clonamos el prefab del texto dentro del Canvas
                GameObject go = Instantiate(TextoPuntajePrefab, canvas.transform);
                textoPuntajeInstancia = go.GetComponent<TextMeshProUGUI>();

                // Si el jugador 2 se conecta más tarde, le ponemos una posición diferente para que no se encimen
                if (OwnerClientId != 0)
                {
                    textoPuntajeInstancia.rectTransform.anchoredPosition += new Vector2(0, -50 * (float) OwnerClientId);
                }

                // Inicializamos el texto
                ActualizarTextoUI(0, monedasRecolectadas.Value);
            }
        }

        // Nos suscribimos al evento: "Cuando cambie el valor de las monedas, ejecuta ActualizarTextoUI"
        monedasRecolectadas.OnValueChanged += ActualizarTextoUI;
    }

    // Se ejecuta al desconectarse (limpieza de memoria)
    public override void OnNetworkDespawn()
    {
        monedasRecolectadas.OnValueChanged -= ActualizarTextoUI;
        if (textoPuntajeInstancia != null) Destroy(textoPuntajeInstancia.gameObject);
    }

    // Esta función se activa automáticamente gracias a Netcode cuando cambia el valor
    private void ActualizarTextoUI(int valorAntiguo, int valorNuevo)
    {
        // Solo actualizamos la UI si somos el dueño de este script/jugador
        if (IsOwner && textoPuntajeInstancia != null)
        {
            textoPuntajeInstancia.text = $"Mis Monedas: {valorNuevo}";
        }
    }

    // Esta función SOLAMENTE la llama el Servidor (la moneda al colisionar)
    public void SumarMoneda()
    {
        if (!IsServer) return;
        monedasRecolectadas.Value += 1;
    }
}