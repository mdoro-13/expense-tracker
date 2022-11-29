using Api.DTO.Request;
using FluentValidation;

namespace Api.Validation.DTO;

public class ExpenseCreateDtoValidator : AbstractValidator<ExpenseCreateDto>
{
	public ExpenseCreateDtoValidator()
	{
		RuleFor(dto => dto.Details)
			.MaximumLength(200)
			.WithMessage("Expense details cannot be longer than 200 characters");
	}
}
