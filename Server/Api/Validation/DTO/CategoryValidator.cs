using Api.Domain.Entities;
using Api.DTO.Request;
using FluentValidation;

namespace Api.Core.Validation.DTO;

public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage("The name is required")
            .MaximumLength(50)
            .WithMessage("The name cannot be longer than 50 characters");

        RuleFor(dto => dto.Color)
            .Length(6)
            .WithMessage("The color code does not have the correct length");
    }
}