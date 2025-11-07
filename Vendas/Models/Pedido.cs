namespace Vendas.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public required DateTime DataPedido { get; set; }
        public required int ClienteId { get; set; }
        public required List<ItemPedido> Itens { get; set; }
    }
}