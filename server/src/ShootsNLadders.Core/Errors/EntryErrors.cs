using ShootsNLadders.Core.Common;

namespace ShootsNLadders.Core.Errors;

/// <summary>
/// Every expected failure for entries, defined once and reused by the service and tests.
/// </summary>
public static class EntryErrors
{
    public static readonly Error AlreadyExists = new(
        "entry.already_exists",
        "An entry for this date already exists.",
        ErrorType.Conflict);

    public static readonly Error FutureDate = new(
        "entry.future_date",
        "Entries can't be created for future dates.",
        ErrorType.Validation);
}
