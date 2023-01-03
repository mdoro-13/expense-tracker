namespace Api.DTO.Request
#pragma warning disable CS8618

{
    public sealed record BudgetCreateDto
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public decimal Amount { get; init; }
    }
}
