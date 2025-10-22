using Estoque.Context;
using MassTransit;
using MensagensCompartilhadas.Messages;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Consumers
{
    public class VerificarEstoqueConsumer : IConsumer<VerificarEstoqueMessage>
    {
        private readonly EstoqueContext _context;

        public VerificarEstoqueConsumer(EstoqueContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<VerificarEstoqueMessage> context)
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == context.Message.ProdutoId);

            var disponivel = produto != null && produto.Quantidade >= context.Message.Quantidade;

            await context.RespondAsync(new RespostaEstoqueMessage
            {
                ProdutoId = context.Message.ProdutoId,
                Disponivel = disponivel
            });

            Console.WriteLine($"[Estoque] Produto {context.Message.ProdutoId}: {(disponivel ? "Disponível" : "Indisponível")}");
        }
    }
}