using Petsgram.Application;
using Petsgram.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Petsgram.Application.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

var storageSettings = app.Services.GetRequiredService<IOptions<StorageSettings>>().Value;
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storageSettings.PhotoPhysicalPath),
    RequestPath = storageSettings.PhotoPublicPath
});

app.MapControllers();

app.Run();
