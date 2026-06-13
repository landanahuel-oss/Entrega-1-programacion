using Unity.Netcode;
using UnityEngine;

public class puntajeJugador : NetworkBehaviour
{
    // Una variable de red que solo el servidor puede modificar, pero todos pueden leer
    public NetworkVariable<int> monedasRecolectadas = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Esta función SOLAMENTE la debe llamar el Servidor
    public void SumarMoneda()
    {
        if (!IsServer) return;

        monedasRecolectadas.Value += 1;
        Debug.Log($"Jugador {OwnerClientId} tiene: {monedasRecolectadas.Value} monedas.");
    }
}