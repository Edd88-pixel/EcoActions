using System.Text.Json.Serialization;

namespace EcoActions.Core;

[JsonSerializable(typeof(AppData))]
public partial class AppJsonContext : JsonSerializerContext;
