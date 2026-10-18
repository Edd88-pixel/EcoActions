using EcoActions.Core;
using System.Text.Json;

var passed = 0;
var failed = 0;
var data = new AppData();
var now = new DateTimeOffset(2026, 10, 14, 12, 0, 0, TimeSpan.Zero);
var registration = Accounts.Register(data, "Eddy", "test@example.org", "Test-Phrase-2030", "Test-Phrase-2030");
var account = registration.Account;

Check("Adresse normalisée", () => Equal("test@example.org", Accounts.NormalizeEmail(" TEST@example.org ")));
Check("Adresse invalide refusée", () => Reject<ArgumentException>(() => Accounts.NormalizeEmail("eddy@")));
Check("Mot de passe court refusé", () => Reject<ArgumentException>(() => Accounts.Register(data, "Eddy", "short@example.org", "123", "123")));
Check("Confirmation différente refusée", () => Reject<ArgumentException>(() => Accounts.Register(data, "Eddy", "different@example.org", "Test-Phrase-2030", "Test-Phrase-2031")));
Check("Doublon refusé", () => Reject<ArgumentException>(() => Accounts.Register(data, "Eddy", "test@example.org", "Test-Phrase-2030", "Test-Phrase-2030")));
Check("Prénom vide refusé", () => Reject<ArgumentException>(() => Accounts.Register(data, " ", "empty@example.org", "Test-Phrase-2030", "Test-Phrase-2030")));
Check("Mot de passe non conservé en clair", () => True(!JsonSerializer.Serialize(data).Contains("Test-Phrase-2030")));
Check("Code de récupération non conservé en clair", () => True(!JsonSerializer.Serialize(data).Contains(registration.RecoveryCode)));
Check("Connexion correcte", () => Equal(account.Id, Accounts.Login(data, "test@example.org", "Test-Phrase-2030").Id));
Check("Mot de passe incorrect refusé", () => Reject<ArgumentException>(() => Accounts.Login(data, "test@example.org", "incorrect-password")));
Check("Compte inconnu refusé", () => Reject<ArgumentException>(() => Accounts.Login(data, "absent@example.org", "incorrect-password")));
Check("Code de récupération incorrect refusé", () => Reject<ArgumentException>(() => Accounts.ResetPassword(data, "test@example.org", "BAD-CODE", "New-Phrase-2030")));
Check("Récupération correcte", () => Accounts.ResetPassword(data, "test@example.org", registration.RecoveryCode, "New-Phrase-2030"));
Check("Ancien mot de passe invalidé", () => Reject<ArgumentException>(() => Accounts.Login(data, "test@example.org", "Test-Phrase-2030")));
Check("Nouveau mot de passe accepté", () => Accounts.Login(data, "test@example.org", "New-Phrase-2030"));
Check("Recherche sans résultat", () => Equal(0, GestureCatalog.Search("voiture", null).Count()));
Check("Filtre mobilité", () => Equal(4, GestureCatalog.Search("", "Mobilité").Count()));
Check("Recherche insensible à la casse", () => True(GestureCatalog.Search("GOURDE", null).Any(g => g.Id == "gourde")));
Check("Quantité négative refusée", () => Reject<ArgumentException>(() => Journal.Add(account, "marche", -1, now)));
Check("Quantité nulle refusée", () => Reject<ArgumentException>(() => Journal.Add(account, "marche", 0, now)));
Check("Quantité excessive refusée", () => Reject<ArgumentException>(() => Journal.Add(account, "marche", 1001, now)));
Check("Bouteille fractionnaire refusée", () => Reject<ArgumentException>(() => Journal.Add(account, "gourde", 1.5m, now)));
Check("Distance décimale enregistrée", () => Equal(2.5m, Journal.Add(account, "marche", 2.5m, now).Quantity));
Check("Journal indépendant par compte", () =>
{
    var other = Accounts.Register(data, "Autre", "other@example.org", "Test-Phrase-2030", "Test-Phrase-2030").Account;
    Equal(0, other.Journal.Count);
    Equal(1, account.Journal.Count);
});
Check("Annuler le dernier geste", () => { Journal.Undo(account); Equal(0, account.Journal.Count); });
Check("Rétablir le geste", () => { Journal.Restore(account); Equal(1, account.Journal.Count); });
Check("Double rétablissement refusé", () => Reject<InvalidOperationException>(() => Journal.Restore(account)));
Check("Défi non rejoint", () => Equal(0, Journal.ChallengeProgress(account)));
account.ChallengeStartedAt = now.AddMinutes(1);
for (var i = 0; i < 8; i++) Journal.Add(account, "gourde", 1, now.AddMinutes(i + 2));
Check("Défi limité à sept gestes", () => Equal(7, Journal.ChallengeProgress(account)));
Check("Gestes avant le défi exclus", () => Equal(7, Journal.ChallengeProgress(account)));
Check("Historique hors semaine exclu", () =>
{
    Journal.Add(account, "repas", 1, now.AddDays(-10));
    Equal(9, Journal.Period(account, now.AddHours(1), false).Count);
});
Check("Historique du mois inclus", () => Equal(10, Journal.Period(account, now.AddHours(1), true).Count));
Check("Unités et pluriels", () => Equal("2 bouteilles évitées", Journal.Summary(GestureCatalog.Get("gourde"), 2)));
Check("Sérialisation et relecture", () =>
{
    var loaded = JsonSerializer.Deserialize(JsonSerializer.Serialize(data, AppJsonContext.Default.AppData), AppJsonContext.Default.AppData)!;
    Equal(account.Journal.Count, loaded.Accounts.Single(a => a.Id == account.Id).Journal.Count);
    Equal(account.ChallengeStartedAt, loaded.Accounts.Single(a => a.Id == account.Id).ChallengeStartedAt);
});
Check("Catalogue sans identifiants en doublon", () => Equal(16, GestureCatalog.All.Select(g => g.Id).Distinct().Count()));
Check("Chaque geste se laisse enregistrer", () =>
{
    var isolated = Accounts.Register(new AppData(), "Catalogue", "catalogue@example.org", "Test-Phrase-2030", "Test-Phrase-2030").Account;
    foreach (var gesture in GestureCatalog.All) Journal.Add(isolated, gesture.Id, gesture.DefaultQuantity, now);
    Equal(16, isolated.Journal.Count);
});
Check("Favori ajouté sans modifier le journal", () =>
{
    var count = account.Journal.Count;
    FavoriteGestures.Toggle(account, "escaliers");
    True(account.FavoriteGestureIds.Contains("escaliers"));
    Equal(count, account.Journal.Count);
});
Check("Favoris conservés à la relecture", () =>
{
    var loaded = JsonSerializer.Deserialize(JsonSerializer.Serialize(data, AppJsonContext.Default.AppData), AppJsonContext.Default.AppData)!;
    True(loaded.Accounts.Single(a => a.Id == account.Id).FavoriteGestureIds.Contains("escaliers"));
});
Check("Favori retiré", () => { FavoriteGestures.Toggle(account, "escaliers"); Equal(0, account.FavoriteGestureIds.Count); });
Check("Favori inconnu refusé", () => Reject<InvalidOperationException>(() => FavoriteGestures.Toggle(account, "inconnu")));
Check("Nouvelles unités au pluriel", () => Equal("2 veilles coupées", Journal.Summary(GestureCatalog.Get("veille"), 2)));
Check("Nouveau geste fractionnaire refusé", () => Reject<ArgumentException>(() => Journal.Add(account, "recharge", 1.5m, now)));
Check("Anciennes données sans favoris compatibles", () =>
{
    var serialized = JsonSerializer.Serialize(data, AppJsonContext.Default.AppData).Replace("\"FavoriteGestureIds\":[],", "");
    var loaded = JsonSerializer.Deserialize(serialized, AppJsonContext.Default.AppData)!;
    Equal(0, loaded.Accounts[0].FavoriteGestureIds.Count);
});
Check("Ancienne liste de favoris null compatible", () =>
{
    var serialized = JsonSerializer.Serialize(data, AppJsonContext.Default.AppData).Replace("\"FavoriteGestureIds\":[]", "\"FavoriteGestureIds\":null");
    var loaded = JsonSerializer.Deserialize(serialized, AppJsonContext.Default.AppData)!;
    Equal(0, loaded.Accounts[0].FavoriteGestureIds.Count);
});
Check("Retrait ciblé parmi plusieurs occurrences du même geste", () =>
{
    var isolated = Accounts.Register(new AppData(), "Journal", "journal@example.org", "Test-Phrase-2030", "Test-Phrase-2030").Account;
    isolated.ChallengeStartedAt = now.AddMinutes(-1);
    var first = Journal.Add(isolated, "marche", 5, now);
    var second = Journal.Add(isolated, "marche", 2, now.AddMinutes(1));
    Journal.Remove(isolated, first.Id);
    Equal(second.Id, isolated.Journal.Single().Id);
    Equal(first.Id, isolated.LastRemoved!.Id);
    Equal(1, Journal.ChallengeProgress(isolated));
    Equal(2m, Journal.Period(isolated, now.AddHours(1), false).Sum(e => e.Quantity));
    var loaded = JsonSerializer.Deserialize(JsonSerializer.Serialize(isolated, AppJsonContext.Default.Account), AppJsonContext.Default.Account)!;
    Equal(second.Id, loaded.Journal.Single().Id);
    Journal.Restore(loaded);
    Equal(2, loaded.Journal.Count);
    Equal(first, loaded.Journal.Single(e => e.Id == first.Id));
});
Check("Retrait absent ou appartenant à un autre compte refusé sans modifier le journal", () =>
{
    var entries = account.Journal.ToArray();
    var removed = account.LastRemoved;
    Reject<InvalidOperationException>(() => Journal.Remove(account, Guid.NewGuid()));
    var other = data.Accounts.Single(a => a.Email == "other@example.org");
    Reject<InvalidOperationException>(() => Journal.Remove(other, entries[0].Id));
    True(entries.SequenceEqual(account.Journal));
    Equal(removed, account.LastRemoved);
    Equal(0, other.Journal.Count);
});
Check("Double retrait refusé et dernier geste retiré rétablissable", () =>
{
    var isolated = Accounts.Register(new AppData(), "Journal", "journal@example.org", "Test-Phrase-2030", "Test-Phrase-2030").Account;
    var entry = Journal.Add(isolated, "gourde", 1, now);
    Journal.Remove(isolated, entry.Id);
    Reject<InvalidOperationException>(() => Journal.Remove(isolated, entry.Id));
    Equal(0, isolated.Journal.Count);
    Journal.Restore(isolated);
    Equal(entry, isolated.Journal.Single());
});
Console.WriteLine($"Résultat : {passed} réussis, {failed} échoués.");
return failed == 0 ? 0 : 1;

void Check(string name, Action action)
{
    try { action(); passed++; Console.WriteLine("PASS " + name); }
    catch (Exception e) { failed++; Console.WriteLine($"FAIL {name}: {e.Message}"); }
}

static void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new InvalidOperationException($"Attendu {expected}, obtenu {actual}");
}

static void True(bool value)
{
    if (!value) throw new InvalidOperationException("La condition attendue est fausse.");
}

static void Reject<T>(Action action) where T : Exception
{
    try { action(); }
    catch (T) { return; }
    throw new InvalidOperationException("L’entrée invalide a été acceptée.");
}
