namespace Vendas.DTOs
{
    public class PedidoDetalhadoDTO
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public float ValorTotal { get; set; }

        public List<ItemPedidoDetalhadoDTO> Itens { get; set; } = new();
    }
}