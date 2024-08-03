using System.Text.Json.Serialization;

namespace Coplt.ShaderReflections.Dx;

[JsonSerializable(typeof(ShaderMeta))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    UseStringEnumConverter = true
)]
internal partial class ShaderMetaJsonContext : JsonSerializerContext { }

[JsonSerializable(typeof(ShaderMeta))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    UseStringEnumConverter = true,
    WriteIndented = true
)]
internal partial class ShaderMetaJsonDebugContext : JsonSerializerContext { }
