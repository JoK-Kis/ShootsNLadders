namespace ShootsNLadders.Core.Common;

/// <summary>
/// Thrown by the unit of work when the database rejects a save because a unique rule was broken
/// (e.g. a second entry for the same day). Infrastructure translates the EF Core/SQLite error into
/// this, so Core can react without knowing about EF Core.
/// </summary>
public sealed class UniqueConstraintException(string message, Exception? innerException = null)
    : Exception(message, innerException);
