using Unity.Netcode;
using UnityEngine;

public class Moneda : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // REGLA DE ORO: Solo el servidor procesa la recolección y la destrucción
        if (!IsServer) return;

        // Comprobamos si lo que tocó la moneda es un Jugador
        if (other.TryGetComponent<puntajeJugador>(out puntajeJugador jugador))
        {
            // 1. Le sumamos la moneda al jugador
            jugador.SumarMoneda();

            // 2. Desaparecemos la moneda de la red (se destruye en todas las pantallas)
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}