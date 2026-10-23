using System.Linq;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private float verticalSpeed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        // 자신이 소유하지 않은 캐릭터는
        // CharacterController로 직접 움직이지 않는다.
        controller.enabled = IsOwner;
    }

    private void Update()
    {
        if (!IsSpawned || !IsOwner)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction =
            new Vector3(horizontal, 0f, vertical).normalized;

        if (controller.isGrounded && verticalSpeed < 0f)
            verticalSpeed = -2f;

        verticalSpeed += gravity * Time.deltaTime;

        Vector3 velocity =
            direction * moveSpeed +
            Vector3.up * verticalSpeed;

        controller.Move(velocity * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.E))
        {
            CoinPickup[] coins = FindObjectsByType<CoinPickup>(
                FindObjectsSortMode.None);

            CoinPickup nearestCoin = null;
            float nearestDistance = 2f;
            int nearestId = -1;

            for (int i = 0; i < coins.Length; i++)
            {
                if (!coins[i].IsSpawned || coins[i].IsCollected.Value)
                    continue;

                float distance = Vector3.Distance(
                    transform.position,
                    coins[i].transform.position
                );

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestCoin = coins[i];
                    nearestId = coins[i].coinId;
                }
            }

            if (nearestCoin != null && CoinGameManager.Instance != null)
            {
                CoinGameManager.Instance.RequestCoin(nearestId);
            }
        }


        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("[시험] 존재하지 않는 코인 ID 999 요청");
            CoinGameManager.Instance?.RequestCoin(999);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            CoinPickup[] coins = FindObjectsByType<CoinPickup>(
                FindObjectsSortMode.None);

            CoinPickup farCoin = null;
            float farthestDistance = 0f;

            foreach (CoinPickup coin in coins)
            {
                if (!coin.IsSpawned || coin.IsCollected.Value)
                    continue;

                float distance = Vector3.Distance(
                    transform.position,
                    coin.transform.position);

                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    farCoin = coin;
                }
            }

            if (farCoin != null)
            {
                Debug.Log($"[시험] 먼 코인 ID {farCoin.coinId} 요청");
                CoinGameManager.Instance?.RequestCoin(farCoin.coinId);
            }
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            CoinPickup[] coins = FindObjectsByType<CoinPickup>(
                FindObjectsSortMode.None);

            foreach (CoinPickup coin in coins)
            {
                if (coin.IsSpawned && coin.IsCollected.Value)
                {
                    Debug.Log($"[시험] 이미 획득한 코인 ID {coin.coinId} 요청");
                    CoinGameManager.Instance?.RequestCoin(coin.coinId);
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            CoinPickup[] coins = FindObjectsByType<CoinPickup>(
                FindObjectsSortMode.None);

            foreach (CoinPickup coin in coins)
            {
                if (!coin.IsSpawned || coin.IsCollected.Value)
                    continue;

                float distance = Vector3.Distance(
                    transform.position,
                    coin.transform.position);

                if (distance > 2f)
                    continue;

                Debug.Log(
                    $"[중복 시험] 코인 ID {coin.coinId} 두 번 요청");

                CoinGameManager.Instance?.RequestCoin(coin.coinId);
                CoinGameManager.Instance?.RequestCoin(coin.coinId);

                break;
            }

        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            const int testCoinId = 0;

            Debug.Log(
                $"[경쟁 시험] Player {OwnerClientId}, Coin {testCoinId} 요청");

            CoinGameManager.Instance?.RequestCoin(testCoinId);
        }

    }


}
