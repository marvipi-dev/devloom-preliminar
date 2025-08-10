using DevloomPreliminar.Models;

namespace DevloomPreliminar.Data;

public interface IProdutoRepository
{
    public Task<IEnumerable<ProdutoModel>> LerProdutosAsync(Guid categoriaId, int page, int size);
    public Task<ProdutoModel?> CriarAsync(ProdutoModel produto);
}