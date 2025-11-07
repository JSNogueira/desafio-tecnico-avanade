namespace Vendas.Models
{
    public class ItemPedido
    {
        public int Id { get; set; }
        public Pedido? Pedido { get; set; }
        public int PedidoId { get; set; }
        public required int ProdutoId { get; set; }
        public required int Quantidade { get; set; }
    }
}