using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // TcpClient는 TCP 서버에 연결하는 클라이언트 객체입니다.
        using TcpClient client = new();
        // ConnectAsync는 IP 주소와 포트를 가진 서버에 비동기로 연결합니다.
        await client.ConnectAsync("127.0.0.1", 7777);
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new(stream);
        using StreamWriter writer = new(stream) { AutoFlush = true };

        await writer.WriteLineAsync("CHAT|안녕하세요");
        string? response = await reader.ReadLineAsync();
        Console.WriteLine($"서버 응답: {response}");
    }
}