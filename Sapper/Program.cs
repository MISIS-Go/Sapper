namespace Sapper;

public class Program
{
    public static void Main(string[] args)
    {
        var contentRootPath = Directory.GetCurrentDirectory();
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = contentRootPath,
            WebRootPath = ResolveFrontendRoot(contentRootPath),
        });

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapFallbackToFile("index.html");

        app.Run();
    }

    private static string ResolveFrontendRoot(string contentRootPath)
    {
        var publishedFrontendPath = Path.Combine(contentRootPath, "static");
        if (Directory.Exists(publishedFrontendPath))
        {
            return publishedFrontendPath;
        }

        return Path.GetFullPath(Path.Combine(contentRootPath, "..", "static", "dist"));
    }
}
