using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        using UdpClient sender = new();
        // UTF8.GetBytes는 전송할 문자열을 네트워크 바이트 배열로 바꿉니다.
        byte[] bytes = Encoding.UTF8.GetBytes("POSITION:3,5");
        // IPEndPoint는 보낼 대상의 IP 주소와 포트를 함께 나타냅니다.
        await sender.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Loopback, 7778));
        Console.WriteLine("전송: POSITION:3,5");
    }
}