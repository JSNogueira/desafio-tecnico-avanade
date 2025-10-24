using Estoque.Context;
using MassTransit;
using MensagensCompartilhadas.DTOs;
using MensagensCompartilhadas.Messages;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Consumers
{
    public class VerificarProdutosPedidoConsumer : IConsumer<VerificarProdutosPedidoMessage>
    {
        private readonly EstoqueContext _context;

        public VerificarProdutosPedidoConsumer(EstoqueContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<VerificarProdutosPedidoMessage> context)
        {
            var ids = context.Message.IdsProdutos;

            var produtos = await _context.Produtos
                .Where(p => ids.Contains(p.Id))
                .Select(p => new ProdutoDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Preco = p.Preco,
                    QuantidadeDisponivel = p.Quantidade
                })
                .ToListAsync();

            await context.RespondAsync(new ListarProdutosPedidoMessage
            {
                Produtos = produtos
            });

        }
    }
}