
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

Console.Write("피어 ID 입력 (A 또는 B): ");
string peerId = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

if (peerId != "A" && peerId != "B")
{
    Console.WriteLine("A 또는 B만 입력할 수 있습니다.");
    return;
}
string remotePeerId = peerId == "A" ? "B" : "A";

int defaultLocalPort = peerId == "A" ? 9001 : 9002;
int defaultRemotePort = peerId == "A" ? 9002 : 9001;

Console.WriteLine("\n===== 네트워크 설정 =====");
Console.WriteLine("기본값을 사용하려면 Enter를 누르세요.");

IPAddress localAddress = ReadAddress(
    "내 IP 주소", IPAddress.Loopback);

int localPort = ReadPort(
    "내 수신 포트", defaultLocalPort);

IPAddress remoteAddress = ReadAddress(
    "상대 IP 주소", IPAddress.Loopback);

int remotePort = ReadPort(
    "상대 수신 포트", defaultRemotePort);

if (localAddress.Equals(remoteAddress)
    && localPort == remotePort)
{
    Console.WriteLine("내 주소와 상대 주소가 같을 수 없습니다.");
    return;
}

using UdpClient udp = new UdpClient(
    new IPEndPoint(localAddress, localPort));

IPEndPoint remote = new IPEndPoint(
    remoteAddress, remotePort);


int x = 0;
int y = 0;
long sequence = 0;
string? previousMessage = null;
string? latestMessage = null;
// 상대 피어의 최신 상태
int remoteX = 0;
int remoteY = 0;
long lastRemoteSequence = -1;
object stateLock = new object();
long receivedCount = 0;
long ignoredCount = 0;
long invalidCount = 0;
long totalDatagrams = 0;

Console.WriteLine($"\n===== UDP Peer {peerId} =====");
Console.WriteLine($"내 주소: {localAddress}:{localPort}");
Console.WriteLine($"상대 주소: {remoteAddress}:{remotePort}");
Console.WriteLine("W/A/S/D: 이동");
Console.WriteLine("P: 현재 상태 확인");
Console.WriteLine("T: 이전 위치 메시지 재전송");
Console.WriteLine("R: 최신 위치 메시지 재전송");
Console.WriteLine("E: 잘못된 메시지 전송 시험");
Console.WriteLine("B: 전송량 비교 시험");
Console.WriteLine("Q: 종료");
using CancellationTokenSource cts = new();
Task receiverTask = ReceiveLoopAsync(cts.Token);

while (true)
{
    Console.Write("\n입력 > ");
    string command = (Console.ReadLine() ?? "")
        .Trim().ToUpperInvariant();

    if (command == "Q")
        break;

if (command == "B")
{
    Console.Write("전송할 메시지 수 (100 또는 1000): ");
    string? input = Console.ReadLine();

    if (input != "100" && input != "1000")
    {
        Console.WriteLine("100 또는 1000만 입력하세요.");
        continue;
    }

    int count = int.Parse(input);
    int sentCount = 0;
    int sendErrors = 0;

    Stopwatch stopwatch = Stopwatch.StartNew();

    for (int i = 0; i < count; i++)
    {
        // 매번 새로운 위치 상태 생성
        x++;
        sequence++;

       string bulkMessage = $"{peerId},{sequence},{x},{y}";
byte[] bulkData = Encoding.UTF8.GetBytes(bulkMessage);

        try
{
    udp.Send(bulkData, bulkData.Length, remote);
    sentCount++;
}
catch (SocketException)
{
    sendErrors++;
}
        // 전송 사이에 별도의 대기 시간을 두지 않음
    }

    stopwatch.Stop();

    Console.WriteLine("\n===== 송신 시험 결과 =====");
    Console.WriteLine($"전송 시도: {count}");
    Console.WriteLine($"송신 성공: {sentCount}");
    Console.WriteLine($"송신 오류: {sendErrors}");
    Console.WriteLine($"소요 시간: {stopwatch.ElapsedMilliseconds}ms");

    continue;
}

    if (command == "P")
    {
        ShowStatus();
        continue;
    }

if (command == "E")
{
    string[] invalidMessages =
    {
        "",
        "INVALID",
        $"{peerId},abc,3,5",
        $"{peerId},1,hello,5",
        $"UNKNOWN,1,3,5"
    };

    foreach (string invalid in invalidMessages)
    {
        byte[] testData = Encoding.UTF8.GetBytes(invalid);

        try
        {
            udp.Send(testData, testData.Length, remote);
            Console.WriteLine($"[오류 시험 송신] {invalid}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"[송신 오류] {ex.Message}");
        }
    }

    continue;
}

if (command == "T" || command == "R")
{
    string? testMessage = command == "T"
        ? previousMessage
        : latestMessage;

    if (testMessage == null)
    {
        Console.WriteLine("재전송할 메시지가 없습니다.");
        continue;
    }

    byte[] testData = Encoding.UTF8.GetBytes(testMessage);

    try
    {
        udp.Send(testData, testData.Length, remote);
        Console.WriteLine($"[시험 재전송] {testMessage}");
    }
    catch (SocketException ex)
    {
        Console.WriteLine($"[송신 오류] {ex.Message}");
    }

    continue;
}

    int nextX = x;
    int nextY = y;

    switch (command)
    {
        case "W": nextY++; break;
        case "S": nextY--; break;
        case "A": nextX--; break;
        case "D": nextX++; break;
        default:
            Console.WriteLine("잘못된 명령어입니다.");
            continue;
    }

    x = nextX;
    y = nextY;
    sequence++;

    string message = $"{peerId},{sequence},{x},{y}";

previousMessage = latestMessage;
latestMessage = message;

byte[] data = Encoding.UTF8.GetBytes(message);

    try
    {
        udp.Send(data, data.Length, remote);
        Console.WriteLine($"[송신] {message}");
    }
    catch (SocketException ex)
    {
        Console.WriteLine($"[송신 오류] {ex.Message}");
    }
}

cts.Cancel();
udp.Close();
await receiverTask;


void ShowStatus()
{
    lock (stateLock)
    {
        Console.WriteLine("\n===== 현재 상태 =====");
        Console.WriteLine($"내 ID: {peerId}");
        Console.WriteLine($"내 위치: ({x}, {y})");
        Console.WriteLine($"내 순서 번호: {sequence}");

        Console.WriteLine($"상대 ID: {remotePeerId}");

        if (lastRemoteSequence < 0)
        {
            Console.WriteLine("상대 위치: 미수신");
        }
        else
        {
            Console.WriteLine(
                $"상대 위치: ({remoteX}, {remoteY})");
            Console.WriteLine(
                $"상대 순서 번호: {lastRemoteSequence}");
        }

       Console.WriteLine("\n===== 수신 통계 =====");
Console.WriteLine($"전체 수신 데이터그램: {totalDatagrams}");
Console.WriteLine($"정상 반영 수: {receivedCount}");
Console.WriteLine($"오래된/중복 메시지 수: {ignoredCount}");
Console.WriteLine($"잘못된 메시지 수: {invalidCount}");
    }
}


async Task ReceiveLoopAsync(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        try
        {
            UdpReceiveResult result =
                await udp.ReceiveAsync(token);
lock (stateLock)
{
    totalDatagrams++;
}
            string message =
                Encoding.UTF8.GetString(result.Buffer);

            string[] parts = message.Split(',');

            if (parts.Length != 4)
            {
                invalidCount++;
                Console.WriteLine("[거절] 잘못된 메시지 형식");
                continue;
            }

            string senderId = parts[0];

            if (senderId != remotePeerId)
            {
                invalidCount++;
                Console.WriteLine("[거절] 알 수 없는 피어 ID");
                continue;
            }

            bool validSequence =
                long.TryParse(parts[1], out long receivedSequence);

            bool validX =
                int.TryParse(parts[2], out int receivedX);

            bool validY =
                int.TryParse(parts[3], out int receivedY);

            if (!validSequence || !validX || !validY
                || receivedSequence < 1)
            {
                invalidCount++;
                Console.WriteLine("[거절] 잘못된 숫자 데이터");
                continue;
            }

            lock (stateLock)
            {
                if (receivedSequence <= lastRemoteSequence)
{
    ignoredCount++;

    Console.WriteLine(
        $"[무시] 오래되거나 중복된 번호: {receivedSequence}");
    continue;
}

                remoteX = receivedX;
                remoteY = receivedY;
                lastRemoteSequence = receivedSequence;
                receivedCount++;

                Console.WriteLine(
                    $"[수신 성공] {senderId} 위치: ({remoteX}, {remoteY}), 번호: {lastRemoteSequence}");
            }
        }
        catch (OperationCanceledException)
        {
            break;
        }
        catch (ObjectDisposedException)
        {
            break;
        }
        catch (SocketException ex)
        {
            if (token.IsCancellationRequested)
                break;

            Console.WriteLine($"[수신 오류] {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[처리 오류] {ex.Message}");
        }
    }
}

IPAddress ReadAddress(string label, IPAddress defaultValue)
{
    while (true)
    {
        Console.Write($"{label} [{defaultValue}]: ");
        string input = (Console.ReadLine() ?? "").Trim();

        if (input == "")
            return defaultValue;

        if (IPAddress.TryParse(input, out IPAddress? address)
            && address.AddressFamily == AddressFamily.InterNetwork)
        {
            return address;
        }

        Console.WriteLine("올바른 IPv4 주소를 입력하세요.");
    }
}

int ReadPort(string label, int defaultValue)
{
    while (true)
    {
        Console.Write($"{label} [{defaultValue}]: ");
        string input = (Console.ReadLine() ?? "").Trim();

        if (input == "")
            return defaultValue;

        if (int.TryParse(input, out int port)
            && port >= 1 && port <= 65535)
        {
            return port;
        }

        Console.WriteLine("포트는 1~65535 사이의 숫자여야 합니다.");
    }
}
