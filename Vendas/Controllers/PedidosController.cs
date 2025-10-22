using MassTransit;
using MensagensCompartilhadas.Messages;
using Microsoft.AspNetCore.Mvc;
using Vendas.Context;
using Vendas.Models;

namespace Vendas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IRequestClient<VerificarEstoqueMessage> _estoqueClient;
        private readonly VendasContext _context;
        
        public PedidosController(IRequestClient<VerificarEstoqueMessage> estoqueClient, VendasContext context)
        {
            _estoqueClient = estoqueClient;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] VerificarEstoqueMessage pedido)
        {
            // Envia a requisição e aguarda a resposta do microserviço de Estoque
            var response = await _estoqueClient.GetResponse<RespostaEstoqueMessage>(pedido);

            if (!response.Message.Disponivel)
                return BadRequest("Produto indisponível.");

            // Cria e salva o pedido no banco
            var novoPedido = new Pedido
            {
                DataPedido = DateTime.Now
            };

            var novoItemPedido = new ItemPedido
            {
                Pedido = novoPedido,
                ProdutoId = pedido.ProdutoId,
                Quantidade = pedido.Quantidade
            };

            _context.Pedidos.Add(novoPedido);
            _context.ItensPedido.Add(novoItemPedido);
            _context.SaveChanges();

            return Ok(new
            {
                Mensagem = "Pedido confirmado com sucesso!",
                ProdutoId = pedido.ProdutoId,
                Quantidade = pedido.Quantidade
            });
        }
    }
}
