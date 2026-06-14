using Unity.Netcode;
using UnityEngine;

public class Moneda : NetworkBehaviour
{
    private bool yaFueRecogida = false;

    private void OnTriggerEnter(Collider other)
    {
        // Solo el servidor toma la decisión
        if (!IsServer) return;

        if (yaFueRecogida) return;

        if (other.TryGetComponent<PuntajeJugador>(out PuntajeJugador jugador))
        {
            yaFueRecogida = true;

            // 1. Le avisamos al jugador (en el servidor y a través de un RPC al cliente dueño)
            jugador.RecogerMonedaParaLlevar();
            NotificarClienteRecogidaClientRpc(other.GetComponent<NetworkObject>().OwnerClientId);

            // 2. Efecto visual: Pegar la moneda al jugador en la red
            GetComponent<Collider>().enabled = false; // Desactivar colisiones

            // Si creaste el "puntoMonedasCargadas" en el jugador, la movemos ahí, si no, al centro del jugador
            Transform puntoSujecion = jugador.puntoMonedasCargadas != null ? jugador.puntoMonedasCargadas : jugador.transform;

            // Sincronizar el movimiento del objeto hijo en la red
            transform.SetParent(puntoSujecion);
            transform.localPosition = new Vector3(0, 2f, 0); // Que flote arriba del jugador
        }
    }

    [ClientRpc]
    private void NotificarClienteRecogidaClientRpc(ulong clientId)
    {
        // Esto asegura que el cliente dueño también actualice su UI local de "Cargando: X"
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PuntajeJugador>();
            if (localPlayer != null && !IsServer) // Evitamos que el Host lo ejecute dos veces
            {
                localPlayer.RecogerMonedaParaLlevar();
            }
        }
    }
}