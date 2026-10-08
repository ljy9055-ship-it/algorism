using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // UdpClient는 UDP 데이터그램을 보내거나 받는 객체입니다.
        using UdpClient receiver = new(7778);
        Console.WriteLine("UDP 수신 대기");
        // ReceiveAsync는 UDP 데이터그램 하나가 도착할 때까지 기다립니다.
        UdpReceiveResult result = await receiver.ReceiveAsync();
        // UTF8.GetString은 받은 바이트 배열을 사람이 읽는 문자열로 바꿉니다.
        string message = Encoding.UTF8.GetString(result.Buffer);
        Console.WriteLine($"수신: {message}");
    }
}