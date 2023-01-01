namespace Api.DTO.Response
#pragma warning disable CS8618

{
    public sealed record BudgetReadDto
    {
        public int Id { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public decimal Amount { get; init; }
    }
}
