namespace ShootsNLadders.Core.Interfaces;

/// <summary>
/// Commits all changes staged by repositories during the current request, in one transaction.
/// Implemented by the EF Core DbContext in Infrastructure.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
