using MassTransit;
using MensagensCompartilhadas.Messages;
using Vendas.Context;
using Vendas.DTOs;
using Vendas.Models;

namespace Vendas.Services
{
    public class PedidoService
    {
        private readonly IRequestClient<VerificarItensPedidoMessage> _estoqueClient;
        private readonly VendasContext _context;

        public PedidoService(
            IRequestClient<VerificarItensPedidoMessage> estoqueClient,
            VendasContext context)
        {
            _estoqueClient = estoqueClient;
            _context = context;
        }

        public async Task<Pedido> CriarPedidoAsync(int clienteId, List<ItemPedidoDTO> itens)
        {
            if (itens == null || !itens.Any())
                throw new ArgumentException("O pedido deve conter ao menos um item.");

            // Verifica a disponibilidade
            var verificarMsg = new VerificarItensPedidoMessage
            {
                Itens = itens.Select(i => new ItemPedidoMessage
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList()
            };

            var response = await _estoqueClient.GetResponse<RespostaItensPedidoMessage>(
                verificarMsg, CancellationToken.None, default);

            if (!response.Message.Disponivel)
                throw new InvalidOperationException(response.Message.Mensagem ?? "Produto indisponível.");

            // Cria o pedido e os itens apenas se o estoque estiver OK
            var pedido = new Pedido
            {
                DataPedido = DateTime.Now,
                ClienteId = clienteId,
                Itens = itens.Select(i => new ItemPedido
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList()
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return pedido;
        }
    }
}