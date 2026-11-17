using ShootsNLadders.Core.Common;
using ShootsNLadders.Core.Entities;
using ShootsNLadders.Core.Errors;
using ShootsNLadders.Core.Interfaces;
using ShootsNLadders.Core.Services.Inputs;

namespace ShootsNLadders.Core.Services;

/// <summary>
/// Business rules for entries: no future dates, one entry per day.
/// Rating ranges and note length are already checked by the API's DTO validation and the database.
/// </summary>
public sealed class EntryService(
    IEntryRepository entries,
    IUnitOfWork unitOfWork,
    TimeProvider time) : IEntryService
{
    public async Task<Result<Entry>> CreateAsync(CreateEntryInput input, CancellationToken ct = default)
    {
        var now = time.GetUtcNow().UtcDateTime;

        // One-day margin so users in time zones ahead of UTC can log their own "today" (see redflags.md).
        var latestAllowedDate = DateOnly.FromDateTime(now).AddDays(1);
        if (input.Date > latestAllowedDate)
            return Result<Entry>.Failure(EntryErrors.FutureDate);

        if (await entries.GetByDateAsync(input.Date, ct) is not null)
            return Result<Entry>.Failure(EntryErrors.AlreadyExists);

        var entry = new Entry
        {
            Date = input.Date,
            Physical = input.Physical,
            Mental = input.Mental,
            Emotional = input.Emotional,
            Note = NormalizeNote(input.Note),
            CreatedAt = now,
        };

        entries.Add(entry);

        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintException)
        {
            // Another request saved this day between our check above and this save.
            return Result<Entry>.Failure(EntryErrors.AlreadyExists);
        }

        return Result<Entry>.Success(entry);
    }

    public Task<IReadOnlyList<Entry>> GetAllAsync(CancellationToken ct = default)
        => entries.GetAllAsync(ct);

    /// <summary>Trims the note; empty or whitespace-only notes become null.</summary>
    private static string? NormalizeNote(string? note)
        => string.IsNullOrWhiteSpace(note) ? null : note.Trim();
}
