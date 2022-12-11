using Api.DTO.Request;
using FluentValidation;

namespace Api.Validation.DTO
{
    public class BudgetCreateValidator : AbstractValidator<BudgetCreateDto>
    {
        public BudgetCreateValidator()
        {
            RuleFor(b => b.StartDate)
                .GreaterThanOrEqualTo(b => b.EndDate)
                .WithMessage("End date cannot be greater than the start date");

        }
    }
}
