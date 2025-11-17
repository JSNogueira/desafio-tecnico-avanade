using FrontendBlazor.Models;

namespace FrontendBlazor.Services
{
    public class CarrinhoService
    {
        private readonly List<CarrinhoItem> _itens = [];

        public event Action? OnCarrinhoAtualizado;

        public IReadOnlyList<CarrinhoItem> Itens => _itens;

        public void AdicionarItem(CarrinhoItem item)
        {
            var existente = _itens.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);
            if (existente != null)
            {
                existente.Quantidade += item.Quantidade;
            }
            else
            {
                _itens.Add(item);
            }

            OnCarrinhoAtualizado?.Invoke();
        }

        public void RemoverItem(int produtoId)
        {
            var item = _itens.FirstOrDefault(p => p.ProdutoId == produtoId);
            if (item != null)
            {
                _itens.Remove(item);
                OnCarrinhoAtualizado?.Invoke();
            }
        }

        public void Limpar()
        {
            _itens.Clear();
            OnCarrinhoAtualizado?.Invoke();
        }

        public decimal CalcularTotal() => _itens.Sum(i => i.Preco * i.Quantidade);
    }
}