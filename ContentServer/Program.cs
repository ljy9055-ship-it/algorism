
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine(
        $"[REQUEST] {context.Request.Method} " +
        $"{context.Request.Path}");

    await next();

    Console.WriteLine(
        $"[RESPONSE] {context.Response.StatusCode} " +
        $"{context.Request.Path}");
});


// 정적 파일 MIME 형식 설정
var contentTypeProvider = new FileExtensionContentTypeProvider();

contentTypeProvider.Mappings[".bin"] = "application/octet-stream";
contentTypeProvider.Mappings[".bundle"] = "application/octet-stream";
contentTypeProvider.Mappings[".hash"] = "text/plain";
contentTypeProvider.Mappings[".json"] = "application/json";

// Addressables 콘텐츠 파일 제공
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider
});

// 서버 상태 확인
app.MapGet("/health", () => Results.Ok(new
{
    status = "OK",
    server = "PlatformCoinLab ContentServer"
}));

app.Run();
