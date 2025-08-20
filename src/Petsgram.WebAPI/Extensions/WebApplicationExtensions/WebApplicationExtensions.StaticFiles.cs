using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Petsgram.Application.Settings;

namespace Petsgram.WebAPI.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static IApplicationBuilder UseStaticPhotoFiles(
        this IApplicationBuilder  app)
    {
        var storage = app.ApplicationServices.GetRequiredService<IOptions<StorageSettings>>().Value;
        return app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(storage.PhotoPhysicalPath),
            RequestPath  = storage.PhotoPublicPath
        });
    }
}