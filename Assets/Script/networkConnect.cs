using Unity.Netcode;
using Unity.Netcode.Transports.UTP; // 1. IMPORTANTE: Añadir esta librería para controlar el transporte
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Librería para manejar el InputField de TextMeshPro

public class networkConnect : MonoBehaviour
{
    [Header("Botones de Conexión")]
    [SerializeField] private Button botonHost;
    [SerializeField] private Button botonClient;

    [Header("Entrada de Datos")]
    [SerializeField] private TMP_InputField inputFieldIP; // El campo para la IP

    [Header("Paneles de la UI")]
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelJuego;

    void Start()
    {
        // --- NUEVA VALIDACIÓN DE SEGURIDAD MULTIJUGADOR ---
        // Si la escena se recargó pero el NetworkManager YA ESTABA activo (Host o Cliente)...
        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient))
        {
            Debug.Log("La red ya estaba activa tras el reinicio. Saltando menú...");
            TransicionAlJuego(); // Apaga el menú y enciende el HUD directamente
            return; // Nos salimos para que no se ejecuten los listeners repetidos ni tire errores
        }
        // --------------------------------------------------

        if (panelMenu != null) panelMenu.SetActive(true);
        if (panelJuego != null) panelJuego.SetActive(false);

        botonHost.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            TransicionAlJuego();
        });

        botonClient.onClick.AddListener(() => {
            ConfigurarConexionCliente();
        });
    }

    void ConfigurarConexionCliente()
    {
        // Obtenemos el texto que escribió el usuario
        string ipIngresada = inputFieldIP.text.Trim();

        // Si el usuario no escribió nada, usamos 'localhost' por defecto para pruebas
        if (string.IsNullOrEmpty(ipIngresada))
        {
            ipIngresada = "127.0.0.1";
        }

        // Accedemos al componente UnityTransport asignado al NetworkManager
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport != null)
        {
            // Le cambiamos la IP de conexión en tiempo real antes de conectar
            transport.ConnectionData.Address = ipIngresada;
            Debug.Log($"Intentando conectar a la IP: {ipIngresada}");

            // Iniciamos el cliente con la nueva dirección
            NetworkManager.Singleton.StartClient();
            TransicionAlJuego();
        }
        else
        {
            Debug.LogError("No se encontró el componente UnityTransport en el NetworkManager.");
        }
    }

    void TransicionAlJuego()
    {
        if (panelMenu != null) panelMenu.SetActive(false);
        if (panelJuego != null) panelJuego.SetActive(true);
    }
}