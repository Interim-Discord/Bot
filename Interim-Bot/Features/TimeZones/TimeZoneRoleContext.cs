using System.Text.Json.Serialization;

namespace Interim.Features.TimeZones;

[JsonSourceGenerationOptions(
	GenerationMode = JsonSourceGenerationMode.Default,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, 
	IncludeFields = true
)]
[JsonSerializable(typeof(TimeZoneRoleData))]
internal partial class TimeZoneRoleDataJsonContext : JsonSerializerContext { }