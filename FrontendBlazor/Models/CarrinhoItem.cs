namespace FrontendBlazor.Models
{
    public class CarrinhoItem
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Quantidade { get; set; } = 1;
        public string? Descricao { get; set; }
    }
}