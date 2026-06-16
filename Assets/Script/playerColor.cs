using Unity.Netcode;
using UnityEngine;

public class playerColor : NetworkBehaviour
{
    [Header("Paleta de Colores")]
    [SerializeField] private Color colorParaHost = Color.blue;   
    [SerializeField] private Color colorParaCliente = Color.red; 

    [SerializeField] private MeshRenderer meshRenderer; 

    private readonly NetworkVariable<Color> netColor = new NetworkVariable<Color>(
        Color.white,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );    // solo el servidor escribe


    public override void OnNetworkSpawn()
    {
        netColor.OnValueChanged += OnColorChanged;        


        if (IsServer)
        {
            if (OwnerClientId == 0)          

            {
                netColor.Value = colorParaHost;
            }
            else
            {
                netColor.Value = colorParaCliente;
            }
        }

        ApplyColor(netColor.Value);        

    }

    public override void OnNetworkDespawn()
    {
        netColor.OnValueChanged -= OnColorChanged;        

    }

    private void OnColorChanged(Color previousValue, Color newValue)
    {
        ApplyColor(newValue);
    }

    private void ApplyColor(Color color)
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = color;            

        }
    }
}