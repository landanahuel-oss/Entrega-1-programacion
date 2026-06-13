using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class networkConnect : MonoBehaviour
{
    [SerializeField] private Button botonHost;
    [SerializeField] private Button botonClient;

    void Start()
    {
        // Asignamos las funciones a los clicks de los botones por código
        botonHost.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            DesactivarMenu();
        });

        botonClient.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            DesactivarMenu();
        });
    }

    void DesactivarMenu()
    {
        // Opcional: Desactiva este script o el Canvas para que desaparezcan los botones al jugar
        gameObject.SetActive(false);
    }
}