using Unity.Netcode;
using UnityEngine;

public class Moneda : NetworkBehaviour
{
    private bool yaFueRecogida = false;

    private void OnTriggerEnter(Collider other)
    {
       
        if (!IsServer) return;

        if (yaFueRecogida) return;

        if (other.TryGetComponent<PuntajeJugador>(out PuntajeJugador jugador))
        {
            yaFueRecogida = true;

            
            jugador.RecogerMonedaParaLlevar();
            NotificarClienteRecogidaClientRpc(other.GetComponent<NetworkObject>().OwnerClientId);

           
            GetComponent<Collider>().enabled = false; 

            
            Transform puntoSujecion = jugador.puntoMonedasCargadas != null ? jugador.puntoMonedasCargadas : jugador.transform;

            
            transform.SetParent(puntoSujecion);
            transform.localPosition = new Vector3(0, 2f, 0); 
        }
    }

    [ClientRpc]
    private void NotificarClienteRecogidaClientRpc(ulong clientId)
    {
        
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PuntajeJugador>();
            if (localPlayer != null && !IsServer) 
            {
                localPlayer.RecogerMonedaParaLlevar();
            }
        }
    }
}