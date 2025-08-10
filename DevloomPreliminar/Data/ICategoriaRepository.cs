using DevloomPreliminar.Models;

namespace DevloomPreliminar.Data;

public interface ICategoriaRepository
{
    public Task<IEnumerable<CategoriaModel>> LerCategoriasAsync();
    public Task<CategoriaModel?> CriarAsync(CategoriaModel categoria);
}