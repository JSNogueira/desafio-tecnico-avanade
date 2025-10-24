using MassTransit;
using MensagensCompartilhadas.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendas.Context;
using Vendas.DTOs;
using Vendas.Models;

namespace Vendas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IRequestClient<VerificarEstoqueMessage> _estoqueClient;
        private readonly IRequestClient<VerificarProdutosPedidoMessage> _produtosClient;
        private readonly VendasContext _context;
        
        public PedidosController(IRequestClient<VerificarEstoqueMessage> estoqueClient, IRequestClient<VerificarProdutosPedidoMessage> produtosClient, VendasContext context)
        {
            _estoqueClient = estoqueClient;
            _produtosClient = produtosClient;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] VerificarEstoqueMessage pedido)
        {
            // Envia a requisição e aguarda a resposta do microserviço de Estoque
            var response = await _estoqueClient.GetResponse<RespostaEstoqueMessage>(pedido);

            if (!response.Message.Disponivel)
                return BadRequest("Produto indisponível.");

            if (response.Message.QuantidadeRestante < pedido.Quantidade)
                return BadRequest("Quantidade insuficiente.");

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
                    return new ItemPedidoDTO
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
