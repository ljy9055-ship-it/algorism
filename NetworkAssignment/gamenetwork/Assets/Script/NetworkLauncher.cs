
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkLauncher : MonoBehaviour
{
    private string address = "127.0.0.1";
    private string portText = "7777";
    private string status = "접속 전";

    private bool connecting = false;
    private bool pendingShutdown = false;
    private float connectionDeadline;

    private const float ConnectionTimeout = 10f;

    private NetworkManager manager;

    private void Start()
    {
        manager = NetworkManager.Singleton;

        if (manager == null)
        {
            status = "NetworkManager를 찾을 수 없음";
            return;
        }

        manager.OnClientConnectedCallback += OnClientConnected;
        manager.OnClientDisconnectCallback += OnClientDisconnected;
        manager.OnServerStarted += OnServerStarted;
        manager.OnTransportFailure += OnTransportFailure;
    }

    private void Update()
    {
        if (manager == null)
            return;

        if (connecting &&
            Time.unscaledTime >= connectionDeadline)
        {
            status = "연결 시간 초과";
            pendingShutdown = true;
        }

        if (pendingShutdown)
        {
            pendingShutdown = false;
            connecting = false;

            if (manager.IsListening)
                manager.Shutdown();
        }
    }

    private void OnServerStarted()
    {
        if (manager != null && manager.IsHost)
        {
            connecting = false;
            status = "Host 실행 중";
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (manager == null)
            return;

        if (clientId == manager.LocalClientId)
        {
            connecting = false;
            status = manager.IsHost
                ? "Host 실행 중"
                : "Client 연결 성공";
        }
        else if (manager.IsServer)
        {
            status = $"Host 실행 중 (Client {clientId} 접속)";
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (manager == null)
            return;

        if (manager.IsServer &&
            clientId != manager.LocalClientId)
        {
            status = $"Host 실행 중 (Client {clientId} 종료)";
            return;
        }

        if (!manager.IsServer)
        {
            connecting = false;
            status = "연결 종료 또는 실패";

            pendingShutdown = true;
        }
    }

    private void OnTransportFailure()
    {
        status = "네트워크 전송 오류";
        connecting = false;
        pendingShutdown = true;
    }

    private void StartNetwork(bool host)
    {
        if (manager == null ||
            manager.IsListening ||
            connecting)
            return;

        if (!ushort.TryParse(portText, out ushort port)
            || port == 0)
        {
            status = "잘못된 포트";
            return;
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            status = "주소를 입력하세요";
            return;
        }

        UnityTransport transport =
            manager.GetComponent<UnityTransport>();

        if (transport == null)
        {
            status = "UnityTransport 없음";
            return;
        }

        transport.SetConnectionData(address, port);

        connecting = true;
        connectionDeadline =
            Time.unscaledTime + ConnectionTimeout;

        status = host
            ? "Host 시작 중"
            : "Client 연결 시도 중";

        bool started = host
            ? manager.StartHost()
            : manager.StartClient();

        if (!started)
        {
            connecting = false;
            status = "시작 실패";
        }
    }

    private void StopNetwork()
    {
        if (manager == null)
            return;

        connecting = false;
        pendingShutdown = false;
        status = "연결 종료";

        if (manager.IsListening)
            manager.Shutdown();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(
            new Rect(10, 10, 300, 240),
            GUI.skin.box);

        GUILayout.Label("NGO Coin Game");

        GUILayout.Label("접속 주소");
        address = GUILayout.TextField(address);

        GUILayout.Label("포트");
        portText = GUILayout.TextField(portText);

        GUILayout.Label("상태: " + status);

        bool busy = manager != null &&
                    (manager.IsListening || connecting);

        GUI.enabled = !busy && manager != null;

        if (GUILayout.Button("Host 시작"))
            StartNetwork(true);

        if (GUILayout.Button("Client 시작"))
            StartNetwork(false);

        GUI.enabled = busy;

        if (GUILayout.Button("연결 종료"))
            StopNetwork();

        GUI.enabled = true;

        GUILayout.Label($"주소: {address}:{portText}");

        GUILayout.EndArea();
    }

    private void OnDestroy()
    {
        if (manager == null)
            return;

        manager.OnClientConnectedCallback -= OnClientConnected;
        manager.OnClientDisconnectCallback -= OnClientDisconnected;
        manager.OnServerStarted -= OnServerStarted;
        manager.OnTransportFailure -= OnTransportFailure;
    }
}
