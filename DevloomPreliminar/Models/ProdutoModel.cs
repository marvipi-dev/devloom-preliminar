namespace DevloomPreliminar.Models;

public class ProdutoModel
{
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
    public required decimal PrecoUnitario { get; init; }
    public required Guid CategoriaId { get; init; }
}