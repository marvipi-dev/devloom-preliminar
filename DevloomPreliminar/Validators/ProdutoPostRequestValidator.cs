using DevloomPreliminar.ViewModels;
using FluentValidation;

namespace DevloomPreliminar.Validators;

public class ProdutoPostRequestValidator : AbstractValidator<ProdutoPostRequest>
{
    public ProdutoPostRequestValidator()
    {
        RuleFor(produto => produto.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório e não pode estar em branco.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");
        
        RuleFor(produto => produto.PrecoUnitario)
            .NotNull().WithMessage("O preço unitário é obrigatório.")
            .GreaterThanOrEqualTo(0).WithMessage("O preço unitário deve ser maior ou igual a 0.");
        
        RuleFor(produto => produto.CategoriaId)
            .NotEmpty().WithMessage("O id da categoria é obrigatório e não pode ser vazio.");
    }
}