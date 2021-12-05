namespace Interim.Extensions;

public static class StringExtensions
{
	public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…")
	{
		return value?.Length > maxLength
			? $"{value[..(maxLength - truncationSuffix.Length)]}{truncationSuffix}"
			: value;
	}

	public static string? ToDiscordCode(this string? value)
	{
		if (value == null) return value;
		StringBuilder stringBuilder = new StringBuilder(value.Length + 8);
		stringBuilder.AppendLine("```");
		stringBuilder.AppendLine(value);
		stringBuilder.AppendLine("```");
		return stringBuilder.ToString();
	}
}