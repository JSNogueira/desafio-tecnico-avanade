namespace Vendas.DTOs
{
    public class ItemPedidoDTO
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public float PrecoUnitario { get; set; }
    }
}