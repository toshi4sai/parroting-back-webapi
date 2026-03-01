var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.UseHttpsRedirection();

// ルート: headerとbodyを両方返す
app.MapMethods("/", ["GET", "POST", "PUT", "DELETE", "PATCH"], async (HttpRequest request, HttpResponse response) =>
{
    var headers = request.Headers
        .ToDictionary(h => h.Key, h => string.Join(", ", h.Value!));

    foreach (var header in headers)
    {
        try
        {
            response.Headers[header.Key] = header.Value;
        }
        catch { }
    }

    if (request.Body.CanRead && request.ContentLength > 0)
    {
        await request.Body.CopyToAsync(response.Body);
    }
}).WithName("EchoAll");

// /header: リクエストのheaderをレスポンスのbodyに入れて返す
app.MapMethods("/header", ["GET", "POST", "PUT", "DELETE", "PATCH"], async (HttpRequest request, HttpResponse response) =>
{
    var headers = request.Headers
        .ToDictionary(h => h.Key, h => string.Join(", ", h.Value!));
    await response.WriteAsJsonAsync(headers);
}).WithName("EchoHeader");

// /body: リクエストのbodyをレスポンスのbodyに入れて返す
app.MapMethods("/body", ["GET", "POST", "PUT", "DELETE", "PATCH"], async (HttpRequest request, HttpResponse response) =>
{
    if (request.Body.CanRead && request.ContentLength > 0)
    {
        await request.Body.CopyToAsync(response.Body);
    }
}).WithName("EchoBody");

app.Run();
