namespace MensagensCompartilhadas.Messages
{
    public class RespostaEstoqueMessage
    {
        public int ProdutoId { get; set; }
        public bool Disponivel { get; set; }
        public int QuantidadeRestante { get; set; }
    }
}