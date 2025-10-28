using MensagensCompartilhadas.DTOs;

namespace MensagensCompartilhadas.Messages
{
    public class RespostaItensPedidoMessage
    {
        public bool Disponivel { get; set; }
        public string? Mensagem { get; set; }
        public List<ProdutoDTO> ProdutosDisponiveis { get; set; } = new();
    }
}