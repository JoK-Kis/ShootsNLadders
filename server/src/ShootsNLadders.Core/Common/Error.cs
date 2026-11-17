namespace ShootsNLadders.Core.Common;

/// <summary>What kind of failure this is. The API maps each type to an HTTP status code.</summary>
public enum ErrorType
{
    Validation, // 400 Bad Request
    NotFound,   // 404 Not Found
    Conflict,   // 409 Conflict
}

/// <summary>An expected failure: a stable code for clients, a human-readable message and its type.</summary>
public sealed record Error(string Code, string Message, ErrorType Type);
