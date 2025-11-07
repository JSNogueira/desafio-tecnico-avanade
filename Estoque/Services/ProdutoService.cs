using Estoque.Context;
using Estoque.Models;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Services
{
    public class ProdutoService
    {
        private readonly EstoqueContext _context;

        public ProdutoService(EstoqueContext context)
        {
            _context = context;
        }

        public async Task<Produto?> ObterPorIdAsync(int id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public async Task<IEnumerable<Produto>> ObterPorNomeAsync(string nome)
        {
            return await _context.Produtos
                .Where(p => p.Nome.Contains(nome))
                .ToListAsync();
        }

        public async Task<IEnumerable<Produto>> ObterTodosAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<Produto> CadastrarAsync(Produto produto)
        {
            if (produto.Preco <= 0)
                throw new ArgumentException("O preço do produto deve ser maior que zero");

            if (produto.Quantidade < 0)
                throw new ArgumentException("A quantidade não pode ser menor que zero");

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<Produto?> AtualizarAsync(int id, Produto produto)
        {
            var produtoBanco = await _context.Produtos.FindAsync(id);
            if (produtoBanco == null) return null;

            if (produto.Preco <= 0)
                throw new ArgumentException("O preço do produto deve ser maior que zero");

            if (produto.Quantidade < 0)
                throw new ArgumentException("A quantidade não pode ser menor que zero");

            produtoBanco.Nome = produto.Nome;
            produtoBanco.Descricao = produto.Descricao;
            produtoBanco.Preco = produto.Preco;
            produtoBanco.Quantidade = produto.Quantidade;

            _context.Produtos.Update(produtoBanco);
            await _context.SaveChangesAsync();
            return produtoBanco;
        }

        public async Task<bool> DeletarAsync(int id)
        {
            var produtoBanco = await _context.Produtos.FindAsync(id);
            if (produtoBanco == null) return false;

            _context.Produtos.Remove(produtoBanco);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}