using Unity.Netcode;
using UnityEngine;

public class MetaDeposito : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        if (!IsServer) return;

        if (other.TryGetComponent<PuntajeJugador>(out PuntajeJugador jugador))
        {
            
            int monedasTraidas = jugador.EntregarMonedas();

            if (monedasTraidas > 0)
            {
                
                jugador.AsegurarMonedas(monedasTraidas);

                
                LimpiarInterfazClienteClientRpc(other.GetComponent<NetworkObject>().OwnerClientId);

                
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