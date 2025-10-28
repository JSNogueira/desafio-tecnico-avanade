namespace MensagensCompartilhadas.Messages
{
    public class VerificarItensPedidoMessage
    {
        public List<ItemPedidoMessage> Itens { get; set; } = new();
    }
}