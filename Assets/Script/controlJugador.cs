using Unity.Netcode;
using UnityEngine;

public class controlJugador : NetworkBehaviour
{
    public float velocidad = 5f;

    void Update()
    {
        // REGLA: Si este no es MI jugador, ignoro el teclado
        if (!IsOwner) return;

        float moverH = Input.GetAxis("Horizontal");
        float moverV = Input.GetAxis("Vertical");

        Vector3 movimiento = new Vector3(moverH, 0, moverV) * velocidad * Time.deltaTime;
        transform.Translate(movimiento
            );
    }
}