namespace DevloomPreliminar.ViewModels;

public record ProdutoResponse
{
    public Guid Id { get; init; }
    public string Nome { get; init; }
    public decimal PrecoUnitario { get; init; }
    public Guid CategoriaId { get; init; }
}