
using UnityEngine;
using Unity.Netcode;

public class CoinGameManager : NetworkBehaviour
{
    public static CoinGameManager Instance { get; private set; }

    public NetworkVariable<int> Score = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private CoinPickup[] coins;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        coins = FindObjectsByType<CoinPickup>(
            FindObjectsSortMode.None);
    }

    public void RequestCoin(int coinId)
    {
        if (!IsSpawned)
            return;

        RequestCoinRpc(coinId);
    }

    [Rpc(SendTo.Server)]
    private void RequestCoinRpc(
        int coinId,
        RpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;

        CoinPickup coin = null;

        if (coins != null)
        {
            foreach (CoinPickup candidate in coins)
            {
                if (candidate != null && candidate.coinId == coinId)
                {
                    coin = candidate;
                    break;
                }
            }
        }

        if (coin == null)
        {
            Debug.Log($"[거절] 잘못된 코인 ID: {coinId}");
            return;
        }

        if (coin == null || !coin.IsSpawned)
        {
            Debug.Log("[거절] 코인을 찾을 수 없음");
            return;
        }

        if (coin.IsCollected.Value)
        {
            Debug.Log("[거절] 이미 획득한 코인");
            return;
        }

        if (!NetworkManager.ConnectedClients.TryGetValue(
            senderId, out var client)
            || client.PlayerObject == null)
        {
            Debug.Log("[거절] 플레이어 없음");
            return;
        }

        float distance = Vector3.Distance(
            client.PlayerObject.transform.position,
            coin.transform.position
        );

        if (distance > coin.pickupDistance)
        {
            Debug.Log($"[거절] 거리 초과: {distance:F2}");
            return;
        }

        coin.CollectOnServer();
        Score.Value++;

        Debug.Log($"[획득 성공] Client {senderId}, Coin {coinId}");
    }

    private void OnGUI()
    {
        if (!IsSpawned)
            return;

        GUI.Box(new Rect(10, 240, 220, 60),
            $"공동 점수: {Score.Value}");
    }
}
