using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// Program은 HTTPS 로그인 콘솔 클라이언트의 시작 클래스입니다.
public class Program
{
    private const string ServerUrl = "https://localhost:5001";

    // Main은 일반적인 콘솔 시작 형식으로 비동기 작업을 끝까지 기다립니다.
    public static void Main(string[] args)
    {
        RunAsync().GetAwaiter().GetResult();
    }

    // RunAsync는 로그인 입력, HTTPS 요청, 응답 출력을 순서대로 처리합니다.
    private static async Task RunAsync()
    {
        Console.Write("아이디: ");
        string id = Console.ReadLine();
        Console.Write("비밀번호: ");
        string password = Console.ReadLine();

        LoginRequest loginRequest = new LoginRequest();
        loginRequest.Id = id;
        loginRequest.Password = password;

        // JsonSerializer는 C# 객체를 JSON 문자열로 바꾸는 도구입니다.
        string requestJson = JsonSerializer.Serialize(loginRequest);

        // HttpClient는 HTTP/HTTPS 요청을 보내는 객체입니다.
        using (HttpClient client = new HttpClient())
        // StringContent는 문자열을 HTTP 요청 본문으로 담는 객체입니다.
        using (StringContent content = new StringContent(
            requestJson,
            Encoding.UTF8,
            "application/json"))
        {
            // PostAsync는 /login 주소로 JSON POST 요청을 비동기로 보냅니다.
            HttpResponseMessage response = await client.PostAsync(
                ServerUrl + "/login",
                content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("로그인 실패: " + response.StatusCode);
                return;
            }

            // ReadAsStringAsync는 응답 본문의 JSON 텍스트를 비동기로 읽습니다.
            string responseJson = await response.Content.ReadAsStringAsync();
            // Deserialize는 JSON 텍스트를 LoginResponse 객체로 읽습니다.
            LoginResponse loginResponse = JsonSerializer.Deserialize<LoginResponse>(
                responseJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            Console.WriteLine("로그인 성공");
            Console.WriteLine("PlayerId: " + loginResponse.PlayerId);
            Console.WriteLine("AccessToken: " + loginResponse.AccessToken);
        }
    }
}

// LoginRequest는 서버에 보낼 로그인 JSON 구조입니다.
public class LoginRequest
{
    public string Id { get; set; }
    public string Password { get; set; }
}

// LoginResponse는 서버가 보낸 로그인 JSON 구조입니다.
public class LoginResponse
{
    public string AccessToken { get; set; }
    public int PlayerId { get; set; }
}