using MensagensCompartilhadas.DTOs;

namespace MensagensCompartilhadas.Messages
{
    public class ListarProdutosPedidoMessage
    {
        public List<ProdutoDTO> Produtos { get; set; }
    }
}