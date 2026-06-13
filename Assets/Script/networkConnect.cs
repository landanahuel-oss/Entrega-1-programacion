using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class networkConnect : MonoBehaviour
{
    [Header("Botones de Conexión")]
    [SerializeField] private Button botonHost;
    [SerializeField] private Button botonClient;

    [Header("UI a Desactivar")]
    // Aquí arrastraremos el GameObject del Canvas completo
    [SerializeField] private GameObject canvasMenu;

    void Start()
    {
        // Al presionar Host, inicia la red y apaga el menú
        botonHost.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            ApagarMenu();
        });

        // Al presionar Cliente, se une y apaga el menú
        botonClient.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            ApagarMenu();
        });
    }

    void ApagarMenu()
    {
        if (canvasMenu != null)
        {
            canvasMenu.SetActive(false); // Apaga el Canvas por completo
        }
        else
        {
            // Failsafe: Si olvidaste asignar el Canvas, al menos apaga este objeto
            gameObject.SetActive(false);
        }
    }
}