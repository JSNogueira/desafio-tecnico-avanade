using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FrontendBlazor;
using FrontendBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using FrontendBlazor.Handlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Handler para JWT
builder.Services.AddTransient<JwtAuthorizationMessageHandler>();

// ProdutoService
builder.Services.AddHttpClient<ProdutoService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/gateway/produto");
})
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

// PedidoService
builder.Services.AddHttpClient<PedidoService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/gateway/pedidos/");
})
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

// Serviços normais
builder.Services.AddScoped<CarrinhoService>();
builder.Services.AddScoped<AuthService>();

// Autenticação
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    provider => provider.GetRequiredService<CustomAuthStateProvider>()
);

await builder.Build().RunAsync();