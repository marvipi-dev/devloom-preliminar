using DevloomPreliminar.ViewModels;
using FluentValidation;

namespace DevloomPreliminar.Validators;

public class CategoriaPostRequestValidator : AbstractValidator<CategoriaPostRequest>
{
    public CategoriaPostRequestValidator()
    {
        RuleFor(categoria => categoria.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório e não pode estar em branco.")
            .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.");
    }
}