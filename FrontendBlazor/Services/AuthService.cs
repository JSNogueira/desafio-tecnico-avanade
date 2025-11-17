using System.Net.Http.Json;
using FrontendBlazor.Models;

namespace FrontendBlazor.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public AuthService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
            _http.BaseAddress = new Uri(_config["ApiGatewayUrl"]!);
        }

        public async Task<string?> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("/gateway/login", request);
                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result?.Token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao autenticar: {ex.Message}");
                return null;
            }
        }
    }
}