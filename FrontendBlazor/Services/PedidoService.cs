using System.Net.Http.Headers;
using System.Net.Http.Json;
using FrontendBlazor.Models;

namespace FrontendBlazor.Services
{
    public class PedidoService
    {
        private readonly HttpClient _http;
        private readonly CustomAuthStateProvider _auth;

        private readonly IConfiguration _config;

        public PedidoService(HttpClient http, CustomAuthStateProvider auth, IConfiguration config)
        {
            _http = http;
            _auth = auth;
            _config = config;
            _http.BaseAddress = new Uri(_config["ApiGatewayUrl"]!);
        }

        public async Task<List<PedidoDTO>> ObterPedidosAsync()
        {
            var token = await _auth.GetTokenAsync();

            if (string.IsNullOrEmpty(token))
                return new List<PedidoDTO>();

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetAsync("gateway/pedidos");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<PedidoDTO>();
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<PedidoDTO>>()
                   ?? new List<PedidoDTO>();
        }

        public async Task<bool> CriarPedido(List<PedidoItemDTO> itens)
        {
            var response = await _http.PostAsJsonAsync("gateway/pedido", itens);
            return response.IsSuccessStatusCode;
        }
    }
}