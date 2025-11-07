using System.Security.Claims;
using MassTransit;
using MensagensCompartilhadas.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendas.Context;
using Vendas.DTOs;
using Vendas.Services;

namespace Vendas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Administrador, Cliente")]
    public class PedidosController : ControllerBase
    {
        private readonly IRequestClient<VerificarItensPedidoMessage> _estoqueClient;
        private readonly IRequestClient<VerificarProdutosPedidoMessage> _produtosClient;
        private readonly VendasContext _context;
        private readonly PedidoService _pedidoService;

        public PedidosController(
            IRequestClient<VerificarItensPedidoMessage> estoqueClient,
            IRequestClient<VerificarProdutosPedidoMessage> produtosClient,
            VendasContext context, PedidoService pedidoService)
        {
            _estoqueClient = estoqueClient;
            _produtosClient = produtosClient;
            _context = context;
            _pedidoService = pedidoService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] List<ItemPedidoDTO> itens)
        {
            var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? User.FindFirst("sub")?.Value;

            if (clienteIdClaim == null)
                return Unauthorized("Token inválido.");

            int clienteId = int.Parse(clienteIdClaim);

            try
            {
                var pedido = await _pedidoService.CriarPedidoAsync(clienteId, itens);
                return Ok(new
                {
                    Mensagem = "Pedido criado com sucesso!",
                    PedidoId = pedido.Id,
                    ClienteId = clienteId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> PedidoDetalhado(int id)
        {
            // Extrai o ID do usuário autenticado (sub do JWT)
            var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? User.FindFirst("sub")?.Value;
            if (clienteIdClaim == null)
                return Unauthorized("Token inválido.");

            int clienteId = int.Parse(clienteIdClaim);

            // Busca o pedido com os itens no banco
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.ClienteId != clienteId)
                //return Forbid();
                return StatusCode(StatusCodes.Status403Forbidden, new { mensagem = "Você não tem permissão para visualizar este pedido." });

            // Extrai os IDs dos produtos
            var idsProdutos = pedido.Itens.Select(i => i.ProdutoId).ToList();

            // Solicita ao Estoque as informações dos produtos
            var response = await _produtosClient.GetResponse<ListarProdutosPedidoMessage>(
                new VerificarProdutosPedidoMessage { IdsProdutos = idsProdutos });

            var produtos = response.Message.Produtos;

            // Monta o DTO de resposta
            var pedidoDetalhado = new PedidoDetalhadoDTO
            {
                Id = pedido.Id,
                DataPedido = pedido.DataPedido,
                ValorTotal = pedido.Itens.Sum(i =>
                {
                    var produto = produtos.FirstOrDefault(p => p.Id == i.ProdutoId);
                    return (produto?.Preco ?? 0) * i.Quantidade;
                }),
                Itens = pedido.Itens.Select(i =>
                {
                    var produto = produtos.FirstOrDefault(p => p.Id == i.ProdutoId);
                    return new ItemPedidoDetalhadoDTO
                    {
                        ProdutoId = i.ProdutoId,
                        NomeProduto = produto?.Nome ?? "Desconhecido",
                        PrecoUnitario = produto?.Preco ?? 0,
                        Quantidade = i.Quantidade
                    };
                }).ToList()
            };

            return Ok(pedidoDetalhado);
        }

        [HttpGet]
        public async Task<IActionResult> TodosOsPedidos()
        {
            var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(clienteIdClaim))
                return Unauthorized("Token inválido.");

            int clienteId = int.Parse(clienteIdClaim);

            // Busca todos os pedidos do cliente com os itens
            var pedidos = await _context.Pedidos
                .Include(p => p.Itens)
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();

            if (!pedidos.Any())
                return NotFound("Nenhum pedido encontrado para este cliente.");

            // Coleta todos os IDs de produtos de todos os pedidos
            var idsProdutos = pedidos
                .SelectMany(p => p.Itens.Select(i => i.ProdutoId))
                .Distinct()
                .ToList();

            // Consulta o microserviço de Estoque
            var response = await _produtosClient.GetResponse<ListarProdutosPedidoMessage>(
                new VerificarProdutosPedidoMessage { IdsProdutos = idsProdutos });

            var produtos = response.Message.Produtos;

            // Monta a lista detalhada de pedidos
            var resultado = pedidos.Select(p => new PedidoDetalhadoDTO
            {
                Id = p.Id,
                DataPedido = p.DataPedido,
                ValorTotal = p.Itens.Sum(i =>
                {
                    var produto = produtos.FirstOrDefault(pr => pr.Id == i.ProdutoId);
                    return (produto?.Preco ?? 0) * i.Quantidade;
                }),
                Itens = p.Itens.Select(i =>
                {
                    var produto = produtos.FirstOrDefault(pr => pr.Id == i.ProdutoId);
                    return new ItemPedidoDetalhadoDTO
                    {
                        ProdutoId = i.ProdutoId,
                        NomeProduto = produto?.Nome ?? "Desconhecido",
                        PrecoUnitario = produto?.Preco ?? 0,
                        Quantidade = i.Quantidade
                    };
                }).ToList()
            }).ToList();

            return Ok(resultado);
        }

    }
}
