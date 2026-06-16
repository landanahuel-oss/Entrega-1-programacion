using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PuntajeJugador : NetworkBehaviour
{
    [Header("UI Config")]
    [SerializeField] private GameObject textoPuntajePrefab;
    private TextMeshProUGUI textoPuntajeInstancia;

   
    public NetworkVariable<int> monedasEnMeta = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

   
    private int monedasCargadas = 0;

   
    [Header("Referencias")]
    public Transform puntoMonedasCargadas;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                GameObject go = Instantiate(textoPuntajePrefab, canvas.transform);
                textoPuntajeInstancia = go.GetComponent<TextMeshProUGUI>();

                if (OwnerClientId != 0)
                {
                    textoPuntajeInstancia.rectTransform.anchoredPosition += new Vector2(0, -50f * (float)OwnerClientId);
                }

                ActualizarTextoUI(0, monedasEnMeta.Value);
            }
        }
        monedasEnMeta.OnValueChanged += ActualizarTextoUI;
    }

    public override void OnNetworkDespawn()
    {
        monedasEnMeta.OnValueChanged -= ActualizarTextoUI;
        if (textoPuntajeInstancia != null) Destroy(textoPuntajeInstancia.gameObject);
    }

    private void ActualizarTextoUI(int valorAntiguo, int valorNuevo)
    {
        if (IsOwner && textoPuntajeInstancia != null)
        {
            textoPuntajeInstancia.text = $"Meta: {valorNuevo} | Cargando: {monedasCargadas}";
        }
    }

   

    public void RecogerMonedaParaLlevar()
    {
        monedasCargadas++;
        if (IsOwner) ActualizarTextoUI(0, monedasEnMeta.Value); 
    }

    public int EntregarMonedas()
    {
        int cantidadAEntregar = monedasCargadas;
        monedasCargadas = 0; 
        if (IsOwner) ActualizarTextoUI(0, monedasEnMeta.Value);
        return cantidadAEntregar; 
    }

    
    public void AsegurarMonedas(int cantidad)
    {
        if (!IsServer) return;
        monedasEnMeta.Value += cantidad;
    }
}