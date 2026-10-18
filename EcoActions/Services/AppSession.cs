using System.Text.Json;
using EcoActions.Core;

namespace EcoActions.Services;

public sealed class AppSession
{
    private readonly SecureVault _vault = new();
    private readonly SemaphoreSlim _gate = new(1, 1);
    private AppData _data = new();
    public Account? Active => _data.Accounts.SingleOrDefault(a => a.Id == _data.ActiveAccountId);
    public Account Account => Active ?? throw new InvalidOperationException("Connectez-vous à votre compte.");

    public async Task OpenAsync() => _data = await _vault.OpenAsync();

    public async Task<T> ChangeAsync<T>(Func<AppData, T> change)
    {
        await _gate.WaitAsync();
        try
        {
            var next = JsonSerializer.Deserialize(JsonSerializer.Serialize(_data, AppJsonContext.Default.AppData), AppJsonContext.Default.AppData)!;
            var result = await Task.Run(() => change(next));
            await _vault.SaveAsync(next);
            _data = next;
            return result;
        }
        finally { _gate.Release(); }
    }

    public Task UpdateAccountAsync(Action<Account> change) => ChangeAsync(data =>
    {
        var account = data.Accounts.Single(a => a.Id == data.ActiveAccountId);
        change(account);
        return true;
    });

    public Task<JournalEntry> AddAsync(Gesture gesture, decimal quantity) => ChangeAsync(data =>
        Journal.Add(data.Accounts.Single(a => a.Id == data.ActiveAccountId), gesture.Id, quantity, DateTimeOffset.Now));

    public async Task LogoutAsync()
    {
        await ChangeAsync(data => { data.ActiveAccountId = null; return true; });
        ReminderService.Cancel();
    }
}
