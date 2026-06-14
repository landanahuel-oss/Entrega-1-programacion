using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class controlJugador : NetworkBehaviour
{
    public float velocidad = 5f;

    // Esta función de Netcode se ejecuta AUTOMÁTICAMENTE en cuanto el Servidor
    // le asigna la autoridad de este objeto al Cliente correspondiente.
    public override void OnGainedOwnership()
    {
        // Forzamos el encendido del script. Es infalible tras el reinicio de red.
        this.enabled = true;
        Debug.Log($"[Netcode] ¡Autoridad recibida! Movimiento activado para el jugador dueño.");
    }

    // Por seguridad, también lo encendemos si ya nace siendo el dueño
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            this.enabled = true;
        }
    }

    void Update()
    {
        // Regla de oro: Si no es mi jugador, no leo su teclado
        if (!IsOwner) return;

        // Tu código de movimiento actual...
        float moverH = 0f;
        float moverV = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moverV = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moverV = -1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moverH = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moverH = 1f;
        }

        Vector3 movimiento = new Vector3(moverH, 0, moverV).normalized * velocidad * Time.deltaTime;
        transform.Translate(movimiento);
    }
}
