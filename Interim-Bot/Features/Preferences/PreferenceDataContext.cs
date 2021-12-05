using System.Text.Json.Serialization;

namespace Interim.Features.Preferences;

[JsonSourceGenerationOptions(
	GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, 
	IncludeFields = true
)]
[JsonSerializable(typeof(PreferenceData))]
internal partial class PreferenceDataJsonContext : JsonSerializerContext { }