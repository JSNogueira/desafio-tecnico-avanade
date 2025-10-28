using System.Text;
using System.Text.Json.Serialization;
using MassTransit;
using MensagensCompartilhadas.Messages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Vendas.Context;

var builder = WebApplication.CreateBuilder(args);

// Configuração JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "minha_chave_super_secreta_jwt_123456";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "AuthService";
var audience = builder.Configuration["Jwt:Audience"] ?? "Microservices";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        RoleClaimType = "tipoUsuario"
    };
});

builder.Services.AddAuthorization();

// Add services to the container.
var connectionString =
    builder.Configuration.GetConnectionString("MySql")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__MySql");

builder.Services.AddDbContext<VendasContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));

// Configurar MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Registra os RequestClients usados no controller
    x.AddRequestClient<VerificarEstoqueMessage>(new Uri("queue:verificar-estoque"));
    x.AddRequestClient<VerificarProdutosPedidoMessage>(new Uri("queue:verificar-produtos-pedido"));
    x.AddRequestClient<VerificarItensPedidoMessage>(new Uri("queue:verificar-itens-pedido"));

    // Configuração do host RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ__Host"] ?? "rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // permite que o MassTransit descubra os endpoints automaticamente
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Executa migrations automaticamente ao iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VendasContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();