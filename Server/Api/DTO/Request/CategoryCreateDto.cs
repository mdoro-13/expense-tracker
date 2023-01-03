namespace Api.DTO.Request;
#pragma warning disable CS8618

public sealed record CategoryCreateDto
{
    public string Name { get; init; }
    public string Color { get; init; }
}
