namespace EcoActions.Core;

public sealed class InputError(string field, string message) : ArgumentException(message)
{
    public string Field { get; } = field;
}
