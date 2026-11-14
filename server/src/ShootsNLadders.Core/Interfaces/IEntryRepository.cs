using ShootsNLadders.Core.Entities;

namespace ShootsNLadders.Core.Interfaces;

public interface IEntryRepository
{
    void Add(Entry entry);

    Task<Entry?> GetByDateAsync(DateOnly date, CancellationToken ct = default);

    Task<IReadOnlyList<Entry>> GetAllAsync(CancellationToken ct = default);
}
