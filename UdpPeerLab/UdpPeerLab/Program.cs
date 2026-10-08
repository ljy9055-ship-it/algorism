using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // args는 실행 명령에서 -- 뒤에 전달한 문자열 배열입니다.
        if (args.Length != 3
            || !int.TryParse(args[0], out int localPort)
            || !int.TryParse(args[1], out int remotePort)
            || localPort < 1 || localPort > 65535
            || remotePort < 1 || remotePort > 65535)
        {
            Console.WriteLine("사용법: dotnet run -- <내 포트> <상대 포트> <이름>");
            Console.WriteLine("포트는 1~65535 사이의 정수를 사용하세요.");
            return;
        }

        string name = args[2];
        // Loopback은 같은 컴퓨터를 가리킵니다. 지정한 로컬 주소와 포트에서 받습니다.
        // using은 범위를 벗어날 때 Dispose를 호출하여 소켓 자원을 정리합니다.
        using (UdpClient peer = new UdpClient(new IPEndPoint(IPAddress.Loopback, localPort)))
        {
            IPEndPoint destination = new IPEndPoint(IPAddress.Loopback, remotePort);
            Console.WriteLine($"{name}: 내 포트 {localPort}, 상대 포트 {remotePort}");
            Console.WriteLine("메시지를 입력하세요. /exit으로 종료합니다.");

            // 수신과 입력·송신을 함께 진행합니다.
            Task receiveTask = ReceiveMessagesAsync(peer);
            // Console.ReadLine은 동기 입력이므로 스레드 풀에서 입력·송신을 실행합니다.
            Task sendTask = Task.Run(() => SendMessagesAsync(peer, destination, name));
            // WhenAll은 이미 시작한 두 작업이 모두 끝날 때까지 기다립니다.
            await Task.WhenAll(sendTask, receiveTask);
        }
    }

    private static async Task SendMessagesAsync(
        UdpClient peer, IPEndPoint destination, string name)
    {
        try
        {
            while (true)
            {
                string? message = Console.ReadLine();
                if (message is null || message == "/exit")
                {
                    return;
                }
                if (!string.IsNullOrWhiteSpace(message))
                {
                    // GetBytes는 이름과 메시지를 UTF-8 바이트 배열로 바꿉니다.
                    byte[] bytes = Encoding.UTF8.GetBytes($"[{name}] {message}");
                    // SendAsync는 지정한 상대에게 데이터그램 하나를 보냅니다.
                    await peer.SendAsync(bytes, bytes.Length, destination);
                    Console.WriteLine($"전송: {message}");
                }
            }
        }
        catch (SocketException exception)
        {
            Console.WriteLine($"송신 실패: {exception.Message}");
        }
        finally
        {
            // 입력이 끝나면 소켓을 닫아 대기 중인 ReceiveAsync도 종료시킵니다.
            peer.Close();
        }
    }

    private static async Task ReceiveMessagesAsync(UdpClient peer)
    {
        try
        {
            while (true)
            {
                // UdpReceiveResult는 받은 바이트와 실제 송신자의 주소·포트를 담습니다.
                UdpReceiveResult result = await peer.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                Console.WriteLine($"수신 {result.RemoteEndPoint}: {message}");
            }
        }
        catch (ObjectDisposedException)
        {
            // /exit으로 소켓을 닫으면 수신 대기가 이 예외로 끝날 수 있습니다.
        }
        catch (SocketException exception)
        {
            // Windows에서는 소켓을 닫을 때도 SocketException이 발생할 수 있습니다.
            // 다른 네트워크 오류일 수도 있으므로 메시지를 출력합니다.
            Console.WriteLine($"수신 종료: {exception.Message}");
        }
    }
}