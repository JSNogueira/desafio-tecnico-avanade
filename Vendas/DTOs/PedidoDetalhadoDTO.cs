namespace Vendas.DTOs
{
    public class PedidoDetalhadoDTO
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public int ClienteId { get; set; }
        public float ValorTotal { get; set; }

        public List<ItemPedidoDTO> Itens { get; set; } = new();
    }
}