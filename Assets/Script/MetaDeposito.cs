using Unity.Netcode;
using UnityEngine;

public class MetaDeposito : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Mismo principio: Solo el servidor procesa los depósitos de puntos
        if (!IsServer) return;

        if (other.TryGetComponent<PuntajeJugador>(out PuntajeJugador jugador))
        {
            // 1. Le pedimos al jugador que nos dé las monedas que trae y limpie su contador
            int monedasTraidas = jugador.EntregarMonedas();

            if (monedasTraidas > 0)
            {
                // 2. Sincronizamos los puntos en la meta
                jugador.AsegurarMonedas(monedasTraidas);

                // Notificar al cliente dueño para limpiar su interfaz local
                LimpiarInterfazClienteClientRpc(other.GetComponent<NetworkObject>().OwnerClientId);

                // 3. Buscamos las monedas físicas que se pegaron al jugador y las destruimos de la red
                Moneda[] monedasPegadas = jugador.GetComponentsInChildren<Moneda>();
                foreach (Moneda moneda in monedasPegadas)
                {
                    moneda.GetComponent<NetworkObject>().Despawn();
                    Destroy(moneda.gameObject);
                }

                Debug.Log($"¡Jugador {jugador.OwnerClientId} depositó {monedasTraidas} monedas!");
            }
        }
    }

    [ClientRpc]
    private void LimpiarInterfazClienteClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PuntajeJugador>();
            if (localPlayer != null && !IsServer)
            {
                localPlayer.EntregarMonedas();
            }
        }
    }
}