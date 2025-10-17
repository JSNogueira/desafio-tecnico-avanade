using Estoque.Context;
using Estoque.Models;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {

        private readonly EstoqueContext _context;

        public ProdutoController(EstoqueContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var produto = _context.Produtos.Find(id);

            if (produto == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(produto);
            }
        }

        [HttpGet("ObterPorNome")]
        public IActionResult ObterPorNome(string nome)
        {
            var produto = _context.Produtos.Where(x => x.Nome.Contains(nome));
            return Ok(produto);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var produto = _context.Produtos;
            return Ok(produto);
        }

        [HttpPost]
        public IActionResult Cadastrar(Produto produto)
        {
            if (produto.Preco <= 0)
                return BadRequest(new { Erro = "O preço do produto deve ser maior que zero" });

            if (produto.Quantidade < 0)
                return BadRequest(new { Erro = "A quantidade não pode ser menor que zero" });

            _context.Add(produto);
            _context.SaveChanges();
            return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Produto produto)
        {
            var produtoBanco = _context.Produtos.Find(id);

            if (produtoBanco == null)
                return NotFound();

            if (produto.Preco <= 0)
                return BadRequest(new { Erro = "O preço do produto deve ser maior que zero" });

            if (produto.Quantidade < 0)
                return BadRequest(new { Erro = "A quantidade não pode ser menor que zero" });    

            produtoBanco.Nome = produto.Nome;
            produtoBanco.Descricao = produto.Descricao;
            produtoBanco.Preco = produto.Preco;
            produtoBanco.Quantidade = produto.Quantidade;

            _context.Produtos.Update(produtoBanco);
            _context.SaveChanges();
            return Ok(produtoBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var produtoBanco = _context.Produtos.Find(id);

            if (produtoBanco == null)
                return NotFound();

            _context.Produtos.Remove(produtoBanco);
            _context.SaveChanges();
            return NoContent();
        }

    }
}