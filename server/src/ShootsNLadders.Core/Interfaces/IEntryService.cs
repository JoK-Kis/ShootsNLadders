using ShootsNLadders.Core.Common;
using ShootsNLadders.Core.Entities;
using ShootsNLadders.Core.Services.Inputs;

namespace ShootsNLadders.Core.Interfaces;

/// <summary>
/// Business actions for entries. Implemented by EntryService; used by the API controllers.
/// </summary>
public interface IEntryService
{
    /// <summary>
    /// Creates the entry for a day. Fails with EntryErrors.FutureDate or EntryErrors.AlreadyExists.
    /// </summary>
    Task<Result<Entry>> CreateAsync(CreateEntryInput input, CancellationToken ct = default);

    /// <summary>Returns all entries, newest date first. Cannot fail in an expected way.</summary>
    Task<IReadOnlyList<Entry>> GetAllAsync(CancellationToken ct = default);
}
