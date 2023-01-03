using Api.DTO.Request;
using FluentValidation;

namespace Api.Validation.DTO
{
    public class BudgetCreateValidator : AbstractValidator<BudgetCreateDto>
    {
        public BudgetCreateValidator()
        {
            RuleFor(b => b.StartDate)
                .LessThanOrEqualTo(b => b.EndDate)
                .WithMessage("Start date cannot be greater than the end date.");
        }
    }
}
