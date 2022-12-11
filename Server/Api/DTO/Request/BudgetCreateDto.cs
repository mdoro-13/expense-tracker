namespace Api.DTO.Request
{
    public sealed record BudgetCreateDto
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public decimal Amount { get; init; }
    }
}
