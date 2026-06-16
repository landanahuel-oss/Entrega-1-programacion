using Unity.Netcode;
using Unity.Netcode.Transports.UTP; 
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class networkConnect : MonoBehaviour
{
    [Header("Botones de Conexión")]
    [SerializeField] private Button botonHost;
    [SerializeField] private Button botonClient;

    [Header("Entrada de Datos")]
    [SerializeField] private TMP_InputField inputFieldIP; 
    [Header("Paneles de la UI")]
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelJuego;

    void Start()
    {
        
        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient))
        {
            Debug.Log("La red ya estaba activa tras el reinicio. Saltando menú...");
            TransicionAlJuego(); 
            return; 
        }
        

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
       
        string ipIngresada = inputFieldIP.text.Trim();

        
        if (string.IsNullOrEmpty(ipIngresada))
        {
            ipIngresada = "127.0.0.1";
        }

        
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport != null)
        {
            
            transport.ConnectionData.Address = ipIngresada;
            Debug.Log($"Intentando conectar a la IP: {ipIngresada}");

            
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