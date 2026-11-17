using Microsoft.EntityFrameworkCore;
using ShootsNLadders.Core.Entities;
using ShootsNLadders.Core.Interfaces;
using ShootsNLadders.Infrastructure.Data;

namespace ShootsNLadders.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IEntryRepository. Only stages changes; IUnitOfWork saves them.
/// </summary>
public sealed class EntryRepository(AppDbContext db) : IEntryRepository
{
    public void Add(Entry entry) => db.Entries.Add(entry);

    /// <summary>Tracked, so a caller can change the returned entry and save it (step 3 edits).</summary>
    public Task<Entry?> GetByDateAsync(DateOnly date, CancellationToken ct = default)
        => db.Entries.FirstOrDefaultAsync(e => e.Date == date, ct);

    /// <summary>Read-only list, so no change tracking.</summary>
    public async Task<IReadOnlyList<Entry>> GetAllAsync(CancellationToken ct = default)
        => await db.Entries
            .AsNoTracking()
            .OrderByDescending(e => e.Date)
            .ToListAsync(ct);
}
