using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class controlJugador : NetworkBehaviour
{
    public float velocidad = 5f;

    
    void Start()
    {
        
    }

    void Update()
    {
        
        if (!IsOwner) return;

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