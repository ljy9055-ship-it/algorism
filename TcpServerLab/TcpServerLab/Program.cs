using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // TcpListener는 지정한 IP 주소와 포트에서 TCP 연결을 기다리는 서버입니다.
        TcpListener listener = new(IPAddress.Loopback, 7777);
        listener.Start();
        Console.WriteLine("클라이언트 연결 대기");

        // AcceptTcpClientAsync는 Client가 연결될 때까지 비동기로 기다립니다.
        using TcpClient client = await listener.AcceptTcpClientAsync();
        // NetworkStream은 연결된 Client와 바이트를 주고받는 통로입니다.
        using NetworkStream stream = client.GetStream();
        // StreamReader와 StreamWriter는 스트림을 한 줄 문자열로 읽고 씁니다.
        using StreamReader reader = new(stream);
        using StreamWriter writer = new(stream) { AutoFlush = true };

        // ReadLineAsync는 줄바꿈이 포함된 메시지 한 줄을 비동기로 받습니다.
        string? message = await reader.ReadLineAsync();
        Console.WriteLine($"서버 수신: {message}");
        // WriteLineAsync는 메시지 끝에 줄바꿈을 붙여 전송합니다.
        await writer.WriteLineAsync("PONG");
        listener.Stop();
    }
}