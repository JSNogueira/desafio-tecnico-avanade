namespace Vendas.DTOs
{
    public class CriarPedidoDTO
    {
        public List<ItemPedidoDTO> Itens { get; set; } = new();
    }
}