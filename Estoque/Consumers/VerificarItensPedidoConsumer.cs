using Estoque.Context;
using MassTransit;
using MensagensCompartilhadas.DTOs;
using MensagensCompartilhadas.Messages;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Consumers
{
    public class VerificarItensPedidoConsumer : IConsumer<VerificarItensPedidoMessage>
    {
        private readonly EstoqueContext _context;

        public VerificarItensPedidoConsumer(EstoqueContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<VerificarItensPedidoMessage> context)
        {
            var itens = context.Message.Itens;
            var idsProdutos = itens.Select(i => i.ProdutoId).ToList();

            var produtos = await _context.Produtos
                .Where(p => idsProdutos.Contains(p.Id))
                .ToListAsync();

            bool todosDisponiveis = true;
            string mensagem = "Todos os produtos estão disponíveis.";

            var produtosDTO = new List<ProdutoDTO>();

            foreach (var item in itens)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == item.ProdutoId);

                if (produto == null || produto.Quantidade < item.Quantidade)
                {
                    todosDisponiveis = false;
                    mensagem = $"Produto {item.ProdutoId} não disponível ou quantidade insuficiente.";
                    break;
                }

                produtosDTO.Add(new ProdutoDTO
                {
                    Id = produto.Id,
                    Nome = produto.Nome,
                    Preco = produto.Preco,
                    QuantidadeDisponivel = produto.Quantidade
                });
            }

            // Só dá baixa no estoque se todos os itens estiverem disponíveis
            if (todosDisponiveis)
            {
                foreach (var item in itens)
                {
                    var produto = produtos.First(p => p.Id == item.ProdutoId);
                    produto.Quantidade -= item.Quantidade;
                }

                await _context.SaveChangesAsync();
            }

            await context.RespondAsync(new RespostaItensPedidoMessage
            {
                Disponivel = todosDisponiveis,
                Mensagem = mensagem,
                ProdutosDisponiveis = produtosDTO
            });
        }
    }
}