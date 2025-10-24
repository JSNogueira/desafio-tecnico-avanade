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

            bool disponivel = false;
            int quantidadeRestante = 0;

            if (produto != null)
            {
                quantidadeRestante = produto.Quantidade;
                if (produto.Quantidade > 0)
                {
                    disponivel = true;
                }

                if (produto.Quantidade >= context.Message.Quantidade)
                {
                    produto.Quantidade -= context.Message.Quantidade;
                    _context.Produtos.Update(produto);
                    _context.SaveChanges();
                }
            }

            await context.RespondAsync(new RespostaEstoqueMessage
            {
                ProdutoId = context.Message.ProdutoId,
                Disponivel = disponivel,
                QuantidadeRestante = quantidadeRestante
            });
        }
    }
}