using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class networkConnect : MonoBehaviour
{
    [Header("Botones de Conexión")]
    [SerializeField] private Button botonHost;
    [SerializeField] private Button botonClient;

    [Header("Paneles de la UI")]
    // Cambiamos el Canvas completo por los paneles específicos
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelJuego;

    void Start()
    {
        // Al inicio, nos aseguramos de que el menú se vea y el juego esté oculto (opcional)
        if (panelMenu != null) panelMenu.SetActive(true);
        if (panelJuego != null) panelJuego.SetActive(false);

        botonHost.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            TransicionAlJuego();
        });

        botonClient.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            TransicionAlJuego();
        });
    }

    void TransicionAlJuego()
    {
        // Apagamos SOLO los botones del menú de inicio
        if (panelMenu != null) panelMenu.SetActive(false);

        // Encendemos el reloj y elementos del juego
        if (panelJuego != null) panelJuego.SetActive(true);
    }
}