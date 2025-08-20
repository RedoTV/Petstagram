using Petsgram.WebAPI.Extensions.ServiceCollectionExtensions;
using Petsgram.WebAPI.Extensions.WebApplicationExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationLayer()
    .AddInfrastructureLayer(builder.Configuration)
    .AddAppSettings(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddAuthorization()
    .AddWebApi();

var app = builder.Build();

app.UseGlobalExceptionHandling()
    .UseSwaggerWithUi()
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization()
    .UseStaticPhotoFiles();

app.MapControllers();

app.Run();