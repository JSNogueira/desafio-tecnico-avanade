using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using LoggingShared.Config;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Configurar o Serilog compartilhado
LoggingConfiguration.ConfigureSerilog("ApiGateway");
builder.Host.UseSerilog();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "minha_chave_super_secreta_jwt_123456";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "AuthService";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

// CORS para o Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5088") // Porta do Blazor
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services
    .AddAuthentication()
    .AddJwtBearer("BearerCliente", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            RoleClaimType = "tipoUsuario"
        };
    })
    .AddJwtBearer("BearerAdmin", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            RoleClaimType = "tipoUsuario"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddOcelot();

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();