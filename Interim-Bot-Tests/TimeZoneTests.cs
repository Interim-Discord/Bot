using FluentAssertions;
using FluentAssertions.Execution;
using Interim.Features.TimeZones;
using Xunit;

namespace Interim_Bot_Tests;

public class TimeZoneTests
{
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
}