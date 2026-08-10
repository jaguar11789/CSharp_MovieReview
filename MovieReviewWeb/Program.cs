var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapFallback(async context =>
{
    var path = context.Request.Path.Value?.Trim('/');

    // https://localhost:7185/
    // ¡æ wwwroot/index.html
    if (string.IsNullOrEmpty(path))
    {
        var indexPath = Path.Combine(app.Environment.WebRootPath, "index.html");

        if (File.Exists(indexPath))
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync(indexPath);
            return;
        }

        context.Response.StatusCode = 404;
        return;
    }
    // /movies ¡æ wwwroot/movies/index.html
    var directoryIndexPath =
        Path.Combine(
            app.Environment.WebRootPath,
            path,
            "index.html"
        );
    if (File.Exists(directoryIndexPath))
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.SendFileAsync(directoryIndexPath);
        return;
    }
    // /login ¡æ wwwroot/login.html
    // /accounts/login ¡æ wwwroot/accounts/login.html
    var filePath = Path.Combine(app.Environment.WebRootPath, $"{path}.html");

    if (!File.Exists(filePath))
    {
        context.Response.StatusCode = 404;

        return;
    }

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(filePath);
});

app.Run();