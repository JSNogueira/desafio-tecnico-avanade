namespace MensagensCompartilhadas.DTOs
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public float Preco { get; set; }
        public int QuantidadeDisponivel { get; set; }
    }
}