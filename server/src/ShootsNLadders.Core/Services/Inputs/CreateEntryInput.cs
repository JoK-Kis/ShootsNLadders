namespace ShootsNLadders.Core.Services;

/// <summary>
/// The data needed to create an entry. The controller builds this from the request DTO,
/// because Core cannot reference the Api project's DTOs.
/// </summary>
public sealed record CreateEntryInput(
    DateOnly Date,
    int Physical,
    int Mental,
    int Emotional,
    string? Note);
