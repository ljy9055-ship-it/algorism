
using UnityEngine;
using Unity.Netcode;

public class CoinPickup : NetworkBehaviour
{
    public int coinId;
    public float pickupDistance = 2f;

    public NetworkVariable<bool> IsCollected =
        new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    private Renderer coinRenderer;
    private Collider coinCollider;

    private void Awake()
    {
        coinRenderer = GetComponent<Renderer>();
        coinCollider = GetComponent<Collider>();
    }

    public override void OnNetworkSpawn()
    {
        IsCollected.OnValueChanged += OnCollectedChanged;
        UpdateAppearance();
    }

    public override void OnNetworkDespawn()
    {
        IsCollected.OnValueChanged -= OnCollectedChanged;
    }

    private void OnCollectedChanged(bool oldValue, bool newValue)
    {
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        if (coinRenderer != null)
            coinRenderer.enabled = !IsCollected.Value;

        if (coinCollider != null)
            coinCollider.enabled = !IsCollected.Value;
    }

    public void CollectOnServer()
    {
        if (!IsServer || IsCollected.Value)
            return;

        IsCollected.Value = true;
    }
}
