namespace EcoActions.Core;

public static class Journal
{
    public static JournalEntry Add(Account account, string gestureId, decimal quantity, DateTimeOffset now)
    {
        var gesture = GestureCatalog.Get(gestureId);
        if (quantity <= 0 || quantity > 1000) throw new InputError("quantity", "Indiquez une quantité supérieure à zéro, sans dépasser 1 000.");
        if (!gesture.AllowsFraction && quantity != decimal.Truncate(quantity))
            throw new InputError("quantity", "Pour ce geste, indiquez un nombre entier.");
        var entry = new JournalEntry(Guid.NewGuid(), gestureId, quantity, now);
        account.Journal.Add(entry);
        account.LastRemoved = null;
        return entry;
    }

    public static void Undo(Account account)
    {
        var entry = account.Journal.MaxBy(e => e.RecordedAt)
            ?? throw new InvalidOperationException("Aucun geste à annuler.");
        account.Journal.Remove(entry);
        account.LastRemoved = entry;
    }

    public static void Remove(Account account, Guid entryId)
    {
        var entry = account.Journal.SingleOrDefault(e => e.Id == entryId)
            ?? throw new InvalidOperationException("Ce geste n’est plus dans votre journal.");
        account.Journal.Remove(entry);
        account.LastRemoved = entry;
    }

    public static void Restore(Account account)
    {
        var entry = account.LastRemoved ?? throw new InvalidOperationException("Aucun geste à rétablir.");
        account.Journal.Add(entry);
        account.LastRemoved = null;
    }

    public static int ChallengeProgress(Account account) => account.ChallengeStartedAt is { } started
        ? Math.Min(7, account.Journal.Count(e => e.RecordedAt >= started)) : 0;

    public static IReadOnlyList<JournalEntry> Period(Account account, DateTimeOffset now, bool month)
    {
        var first = month ? new DateTime(now.Year, now.Month, 1) : now.LocalDateTime.Date.AddDays(-6);
        return account.Journal.Where(e => e.RecordedAt.LocalDateTime >= first && e.RecordedAt <= now).ToArray();
    }

    public static string Summary(Gesture gesture, decimal quantity)
    {
        var unit = quantity <= 1 ? gesture.Unit : gesture.PluralUnit ?? gesture.Id switch
        {
            "gourde" => "bouteilles évitées", "repas" => "repas végétaux",
            "energie" => "extinctions", _ => gesture.Unit
        };
        return $"{quantity:0.##} {unit}";
    }
}
