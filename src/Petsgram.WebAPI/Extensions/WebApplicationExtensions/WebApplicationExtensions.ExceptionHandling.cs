namespace Petsgram.WebAPI.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(
        this IApplicationBuilder app)
    {
        return app.UseExceptionHandler();
    }
}