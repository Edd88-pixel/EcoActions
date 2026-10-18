namespace EcoActions.Core;

public sealed record Gesture(string Id, string Category, string Title, string ShortTitle,
    string Description, string Image, string Icon, string CategoryColor, string Unit,
    decimal DefaultQuantity, string[] Steps, string? PluralUnit = null, bool AllowsFraction = false);

public sealed record JournalEntry(Guid Id, string GestureId, decimal Quantity, DateTimeOffset RecordedAt);
public sealed record PasswordSecret(string Salt, string Hash);

public sealed class Account
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required PasswordSecret Password { get; set; }
    public required PasswordSecret Recovery { get; init; }
    public List<JournalEntry> Journal { get; init; } = [];
    private List<string> _favoriteGestureIds = [];
    [System.Diagnostics.CodeAnalysis.AllowNull]
    public List<string> FavoriteGestureIds
    {
        get => _favoriteGestureIds;
        // Accounts saved before favorites may contain a missing or null list.
        set => _favoriteGestureIds = value ?? [];
    }
    public JournalEntry? LastRemoved { get; set; }
    public DateTimeOffset? ChallengeStartedAt { get; set; }
    public bool RemindersEnabled { get; set; }
    public TimeSpan ReminderTime { get; set; } = new(18, 30, 0);
}

public sealed class AppData
{
    public int Version { get; init; } = 1;
    public List<Account> Accounts { get; init; } = [];
    public Guid? ActiveAccountId { get; set; }
}
