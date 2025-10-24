using MensagensCompartilhadas.DTOs;

namespace Vendas.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    }
}