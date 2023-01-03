namespace Api.DTO.Response;
#pragma warning disable CS8618

public sealed record CategoryReadDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Color { get; init; }
}
