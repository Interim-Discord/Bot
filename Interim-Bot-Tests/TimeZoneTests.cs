using FluentAssertions;
using FluentAssertions.Execution;
using Interim.Features.TimeZones;
using Xunit;

namespace Interim_Bot_Tests;

public class TimeZoneTests
{
	/// <summary>
	/// Tests that all iana time zone ids are valid and have matching windows time ids.
	/// </summary>
	[Fact]
	public void ValidateAllTimeZones()
	{
		using var scope = new AssertionScope();
		foreach (KeyValuePair<string, LocalGroup> group in TimeZoneLookup.Groups)
		{
			foreach (Locale[] localGroup in group.Value.Locales)
			{
				foreach (Locale locale in localGroup)
				{
					if (locale.ID.StartsWith("continent_") || locale.ID.StartsWith("country_"))
						continue;
					if (locale.ID.StartsWith("dropdown_"))
						continue;
					locale.ID.Should().StartWith("tz_");

					string ianaTimeZoneId = locale.ID["tz_".Length..];
					TimeZoneRoles.TryGetTimeZoneInfo(ianaTimeZoneId, out _)
						.Should()
						.NotBe(TimeZoneRoles.Support.Invalid, $"\"{ianaTimeZoneId}\" does not have an associated {nameof(TimeZoneInfo)}");
				}
			}
		}
	}

	/// <summary>
	/// Tests that buttons are a max of 5x5
	/// Tests that dropdowns are a max of 5x25
	/// </summary>
	[Fact]
	public void ValidateTimeZoneCounts()
	{
		using var scope = new AssertionScope();
		foreach (KeyValuePair<string, LocalGroup> group in TimeZoneLookup.Groups)
		{
			LocalGroup localGroup = group.Value;
			localGroup.Locales.Length.Should().BeLessThanOrEqualTo(5);
			switch (localGroup.Mode)
			{
				case InteractionMode.Buttons:
					foreach (Locale[] locales in localGroup.Locales)
						locales.Length.Should().BeLessThanOrEqualTo(5);
					break;
				case InteractionMode.Dropdowns:
					foreach (Locale[] locales in localGroup.Locales)
						locales.Length.Should().BeLessThanOrEqualTo(25);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}