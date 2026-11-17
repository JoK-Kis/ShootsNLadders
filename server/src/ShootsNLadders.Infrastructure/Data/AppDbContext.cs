using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ShootsNLadders.Core.Common;
using ShootsNLadders.Core.Entities;
using ShootsNLadders.Core.Interfaces;

namespace ShootsNLadders.Infrastructure.Data;

/// <summary>
/// EF Core's connection to the SQLite database. Also the app's unit of work:
/// repositories stage changes on it, services call SaveChangesAsync once per operation.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IUnitOfWork
{
    /// <summary>SQLite's extended error code for a broken UNIQUE rule (SQLITE_CONSTRAINT_UNIQUE).</summary>
    private const int SqliteConstraintUnique = 2067;

    public DbSet<Entry> Entries => Set<Entry>();

    /// <summary>
    /// Saves all staged changes in one transaction. A duplicate value (e.g. a second entry for the
    /// same day) is rethrown as Core's UniqueConstraintException so services can handle it.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException
        {
            SqliteExtendedErrorCode: SqliteConstraintUnique
        })
        {
            throw new UniqueConstraintException("A unique constraint was violated while saving.", ex);
        }
    }

    /// <summary>Applies every IEntityTypeConfiguration class in this project (e.g. EntryConfiguration).</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
