using Petsgram.Application;
using Petsgram.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Petsgram.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StorageSettings>(
    builder.Configuration.GetSection(StorageSettings.SectionName)
);
builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection(AuthSettings.SectionName)
);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authSettings = builder.Configuration.GetSection(AuthSettings.SectionName).Get<AuthSettings>();
        if (authSettings == null ||
            string.IsNullOrEmpty(authSettings.SecretKey) ||
            string.IsNullOrEmpty(authSettings.Issuer) ||
            string.IsNullOrEmpty(authSettings.Audience))
            throw new InvalidOperationException(JwtBearerDefaults.AuthenticationScheme);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = authSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = authSettings.GetSymmetricSecurityKey(),
            ValidateLifetime = true,
        };
    });

builder.Services.AddApplication();
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

var storageSettings = app.Services.GetRequiredService<IOptions<StorageSettings>>().Value;
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storageSettings.PhotoPhysicalPath),
    RequestPath = storageSettings.PhotoPublicPath
});

app.MapControllers();

app.Run();
