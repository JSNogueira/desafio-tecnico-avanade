using System.Security.Claims;
using MassTransit;
using MensagensCompartilhadas.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendas.Context;
using Vendas.DTOs;
using Vendas.Models;

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

        public PedidosController(
            IRequestClient<VerificarItensPedidoMessage> estoqueClient,
            IRequestClient<VerificarProdutosPedidoMessage> produtosClient,
            VendasContext context)
        {
            _estoqueClient = estoqueClient;
            _produtosClient = produtosClient;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] List<ItemPedidoDTO> itens)
        {
            if (itens == null || !itens.Any())
                return BadRequest("O pedido deve conter ao menos um item.");

            // Extrai o ID do usuário autenticado (sub do JWT)
            var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                 ?? User.FindFirst("sub")?.Value;
            if (clienteIdClaim == null)
                return Unauthorized("Token inválido.");

            int clienteId = int.Parse(clienteIdClaim);

            // Monta a mensagem para o microserviço de estoque
            var verificarMsg = new VerificarItensPedidoMessage
            {
                Itens = itens.Select(i => new ItemPedidoMessage
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList()
            };

            // Envia ao Estoque para verificar disponibilidade
            var response = await _estoqueClient.GetResponse<RespostaItensPedidoMessage>(verificarMsg);

            if (!response.Message.Disponivel)
                return BadRequest(response.Message.Mensagem ?? "Um ou mais produtos estão indisponíveis.");

            // Todos disponíveis → cria o pedido
            var novoPedido = new Pedido
            {
                DataPedido = DateTime.Now,
                ClienteId = clienteId
            };

            // Cria os itens do pedido
            var itensPedido = itens.Select(i => new ItemPedido
            {
                Pedido = novoPedido,
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            }).ToList();

            _context.Pedidos.Add(novoPedido);
            _context.ItensPedido.AddRange(itensPedido);
            await _context.SaveChangesAsync();

            var valorTotal = response.Message.ProdutosDisponiveis
                .Sum(p => p.Preco * (itens.First(i => i.ProdutoId == p.Id).Quantidade));

            return Ok(new
            {
                Mensagem = "Pedido criado com sucesso!",
                PedidoId = novoPedido.Id,
                ClienteId = clienteId,
                ValorTotal = valorTotal,
                Itens = itens
            });
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPedidoDetalhado(int id)
        {
            // 1️⃣ Busca o pedido com os itens no banco
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            // 2️⃣ Extrai os IDs dos produtos
            var idsProdutos = pedido.Itens.Select(i => i.ProdutoId).ToList();

            // 3️⃣ Solicita ao Estoque as informações dos produtos
            var response = await _produtosClient.GetResponse<ListarProdutosPedidoMessage>(
                new VerificarProdutosPedidoMessage { IdsProdutos = idsProdutos });

            var produtos = response.Message.Produtos;

            // 4️⃣ Monta o DTO de resposta
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

        // [HttpGet]
        // public async Task<IActionResult> ListarPedidos()
        // {
        //     var pedidos = await _context.Pedidos
        //         .OrderByDescending(p => p.DataPedido)
        //         .ToListAsync();

        //     return Ok(pedidos);
        // }

        // [HttpGet("{id:int}")]
        // public async Task<IActionResult> ObterPedidoPorId(int id)
        // {
        //     var pedido = await _context.Pedidos.FindAsync(id);

        //     if (pedido == null)
        //         return NotFound($"Pedido com ID {id} não encontrado.");

        //     return Ok(pedido);
        // }

    }
}
