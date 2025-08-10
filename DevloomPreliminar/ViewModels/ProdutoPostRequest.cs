namespace DevloomPreliminar.ViewModels;

public record ProdutoPostRequest
{
    public string? Nome { get; init; }
    public decimal PrecoUnitario { get; init; }
    public Guid CategoriaId { get; init; }
}