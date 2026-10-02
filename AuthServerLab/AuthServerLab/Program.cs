using System;
using System.Collections.Concurrent;

// Program은 HTTPS 인증 서버를 시작하는 클래스입니다.
public class Program
{
    // ConcurrentDictionary는 여러 요청이 동시에 와도 안전하게 값을 보관하는 컬렉션입니다.
    private static readonly ConcurrentDictionary<string, int> IssuedTokens =
        new ConcurrentDictionary<string, int>();

    // Main은 서버 프로그램의 시작 메서드입니다.
    public static void Main(string[] args)
    {
        // WebApplicationBuilder는 ASP.NET Core 웹 서버 설정을 만드는 객체입니다.
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        // WebApplication은 HTTP/HTTPS 요청을 받는 서버 객체입니다.
        WebApplication app = builder.Build();

        // MapPost는 POST 요청 주소와 처리 메서드를 연결합니다.
        app.MapPost("/login", Login);
        app.MapPost("/validate", Validate);

        // Run은 localhost 5001번 포트에서 HTTPS 서버를 실행합니다.
        app.Run("https://localhost:5001");
    }

    // Login은 /login 요청의 계정 정보를 확인하고 토큰을 발급합니다.
    private static IResult Login(LoginRequest request)
    {
        // 학습용 고정 계정입니다. 실제 서비스는 DB와 비밀번호 해시를 사용합니다.
        if (request.Id != "player01" || request.Password != "1234")
        {
            return Results.Unauthorized();
        }

        // Guid는 중복 가능성이 매우 낮은 식별자를 만드는 타입입니다.
        // ToString("N")에서 "N"은 하이픈(-) 없이 32자리 16진수로 출력하라는 뜻입니다.
        // 예: <guid> 7e6d7b64-3145-42e4-9c85-4c18aab0e7f2 -> 7e6d7b64314542e49c854c18aab0e7f2
        string accessToken = Guid.NewGuid().ToString("N");
        IssuedTokens[accessToken] = 101;

        LoginResponse response = new LoginResponse();
        response.AccessToken = accessToken;
        response.PlayerId = 101;

        return Results.Ok(response);
    }

    // Validate는 게임 서버가 토큰을 확인하는 상황을 단순화한 API입니다.
    private static IResult Validate(TokenRequest request)
    {
        int playerId;

        if (!IssuedTokens.TryGetValue(request.AccessToken, out playerId))
        {
            return Results.Unauthorized();
        }

        ValidateResponse response = new ValidateResponse();
        response.PlayerId = playerId;

        return Results.Ok(response);
    }
}

// LoginRequest는 클라이언트가 보내는 로그인 JSON 구조입니다.
public class LoginRequest
{
    public string Id { get; set; }
    public string Password { get; set; }
}

// LoginResponse는 서버가 돌려주는 로그인 JSON 구조입니다.
public class LoginResponse
{
    public string AccessToken { get; set; }
    public int PlayerId { get; set; }
}

// TokenRequest는 토큰 검증 요청의 JSON 구조입니다.
public class TokenRequest
{
    public string AccessToken { get; set; }
}

// ValidateResponse는 토큰 검증 성공 뒤 돌려주는 JSON 구조입니다.
public class ValidateResponse
{
    public int PlayerId { get; set; }
}