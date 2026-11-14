namespace ShootsNLadders.Core.Entities;
public class Entry
{
    public const int MinRating = 1;
    public const int MaxRating = 10;
    public const int NoteMaxLength = 500;

    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int Physical { get; set; }
    public int Mental { get; set; }
    public int Emotional { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }
    public int Total => Physical + Mental + Emotional;
}
