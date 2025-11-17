namespace FrontendBlazor.Models
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public float ValorTotal { get; set; }
        public List<ItemPedidoDTO> Itens { get; set; } = new();
        //public string UsuarioId { get; set; } = string.Empty;
    }
}