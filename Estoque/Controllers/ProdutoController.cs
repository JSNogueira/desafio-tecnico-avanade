using Estoque.Models;
using Estoque.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _produtoService;

        public ProdutoController(ProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);
            return produto == null ? NotFound() : Ok(produto);
        }

        [HttpGet("ObterPorNome")]
        public async Task<IActionResult> ObterPorNome(string nome)
        {
            var produtos = await _produtoService.ObterPorNomeAsync(nome);
            return Ok(produtos);
        }

        [HttpGet("ObterTodos")]
        public async Task<IActionResult> ObterTodos()
        {
            var produtos = await _produtoService.ObterTodosAsync();
            return Ok(produtos);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Cadastrar(Produto produto)
        {
            try
            {
                var novoProduto = await _produtoService.CadastrarAsync(produto);
                return CreatedAtAction(nameof(ObterPorId), new { id = novoProduto.Id }, novoProduto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, Produto produto)
        {
            try
            {
                var atualizado = await _produtoService.AtualizarAsync(id, produto);
                if (atualizado == null) return NotFound();
                return Ok(atualizado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var sucesso = await _produtoService.DeletarAsync(id);
            return sucesso ? NoContent() : NotFound();
        }
    }
}