using MassTransit;
using MensagensCompartilhadas.Messages;
using Microsoft.EntityFrameworkCore;
using Moq;
using Vendas.Context;
using Vendas.DTOs;
using Vendas.Services;

namespace VendasTest;

[TestClass]
public class PedidoServiceTests
{
    private PedidoService _service = null!;
    private Mock<IRequestClient<VerificarItensPedidoMessage>> _estoqueClientMock = null!;
    private VendasContext _context = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<VendasContext>()
            .UseInMemoryDatabase("VendasTestDb")
            .Options;

        _context = new VendasContext(options);

        _estoqueClientMock = new Mock<IRequestClient<VerificarItensPedidoMessage>>();

        _service = new PedidoService(_estoqueClientMock.Object, _context);
    }

    [TestMethod]
    public async Task Deve_CriarPedido_QuandoEstoqueDisponivel()
    {
        // Arrange
        var itens = new List<ItemPedidoDTO>
            {
                new ItemPedidoDTO { ProdutoId = 1, Quantidade = 2 }
            };

        var resposta = new RespostaItensPedidoMessage
        {
            Disponivel = true
        };

        var responseMock = new Mock<Response<RespostaItensPedidoMessage>>();
        responseMock.Setup(r => r.Message).Returns(resposta);

        _estoqueClientMock
            .Setup(c => c.GetResponse<RespostaItensPedidoMessage>(
                It.IsAny<VerificarItensPedidoMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()
            ))
            .ReturnsAsync(responseMock.Object);

        // Act
        var pedido = await _service.CriarPedidoAsync(1, itens);

        // Assert
        Assert.IsNotNull(pedido);
        Assert.AreEqual(1, pedido.ClienteId);
    }

    [TestMethod]
    public async Task CriarPedido_DeveFalhar_QuandoProdutoIndisponivel()
    {
        // Arrange
        var itens = new List<ItemPedidoDTO>
            {
                new ItemPedidoDTO { ProdutoId = 1, Quantidade = 5 }
            };

        var respostaMensagem = new RespostaItensPedidoMessage
        {
            Disponivel = false,
            Mensagem = "Produto indisponível."
        };

        var responseMock = new Mock<Response<RespostaItensPedidoMessage>>();
        responseMock.Setup(r => r.Message).Returns(respostaMensagem);

        _estoqueClientMock
            .Setup(c => c.GetResponse<RespostaItensPedidoMessage>(
                It.IsAny<VerificarItensPedidoMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()
            ))
            .ReturnsAsync(responseMock.Object);

        // Act + Assert
        var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            _service.CriarPedidoAsync(1, itens));

        Assert.AreEqual("Produto indisponível.", ex.Message);
        Assert.AreEqual(0, _context.Pedidos.Count());
    }
}