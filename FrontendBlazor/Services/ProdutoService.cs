using System.Net.Http.Json;
using FrontendBlazor.Models;

namespace FrontendBlazor.Services
{
    public class ProdutoService
    {
        private readonly HttpClient _http;
        //private readonly IConfiguration _config;

        public ProdutoService(HttpClient http)
        {
            _http = http;
            //_config = config;
            //_http.BaseAddress = new Uri(_config["ApiGatewayUrl"]!);
        }

        public async Task<List<Produto>> ObterTodosAsync()
        {
            try
            {
                var produtos = await _http.GetFromJsonAsync<List<Produto>>("produtos");
                return produtos ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar produtos: {ex.Message}");
                return [];
            }
        }

        public async Task<bool> CadastrarProduto(Produto produto)
        {
            var response = await _http.PostAsJsonAsync("", produto);
            return response.IsSuccessStatusCode;
        }
    }
}