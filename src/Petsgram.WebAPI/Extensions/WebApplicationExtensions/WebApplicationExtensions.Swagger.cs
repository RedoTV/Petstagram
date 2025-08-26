namespace Petsgram.WebAPI.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(
        this IApplicationBuilder  app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }
}