using Estoque.Context;
using Estoque.Models;
using Estoque.Services;
using Microsoft.EntityFrameworkCore;

namespace EstoqueTest;

[TestClass]
public sealed class ProdutoServiceTests
{
    private EstoqueContext _context = null!;
    private ProdutoService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        // Cria o banco em memória
        var options = new DbContextOptionsBuilder<EstoqueContext>()
            .UseInMemoryDatabase(databaseName: "EstoqueTestDb")
            .Options;

        _context = new EstoqueContext(options);
        _service = new ProdutoService(_context);
    }

    [TestMethod]
    public async Task CadastrarAsync_Deve_Criar_Produto_Valido()
    {
        // Arrange
        var produto = new Produto
        {
            Nome = "Mouse Gamer",
            Descricao = "Mouse RGB 7200 DPI",
            Preco = 150.00f,
            Quantidade = 10
        };

        // Act
        var resultado = await _service.CadastrarAsync(produto);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(1, _context.Produtos.Count());
        Assert.AreEqual("Mouse Gamer", resultado.Nome);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task CadastrarAsync_Deve_Falhar_Se_Preco_For_Invalido()
    {
        // Arrange
        var produto = new Produto
        {
            Nome = "Teclado",
            Descricao = "Teclado mecânico",
            Preco = 0, // preço inválido
            Quantidade = 5
        };

        // Act
        await _service.CadastrarAsync(produto);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task CadastrarAsync_Deve_Falhar_Se_Quantidade_Negativa()
    {
        // Arrange
        var produto = new Produto
        {
            Nome = "Monitor",
            Descricao = "Monitor 24 polegadas",
            Preco = 800,
            Quantidade = -1 // quantidade inválida
        };

        // Act
        await _service.CadastrarAsync(produto);
    }
}
