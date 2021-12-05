using System.Globalization;

namespace Interim.Extensions;

public static class TimeExtensions
{
	public static string ToAmPmString(this DateTime dt) => dt.ToString("hh:mm tt", CultureInfo.InvariantCulture);

	public static DateTime ToHalfHour(this DateTime dt)
	{
		int minute;
		switch (dt.Minute)
		{
			case < 15:
				minute = 0;
				break;
			case < 45:
				minute = 30;
				break;
			default:
				return RoundUp(dt, TimeSpan.FromMinutes(60));
		}

		return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, minute, 0, dt.Kind);
	}

	public static DateTime RoundUp(this DateTime dt, TimeSpan d) => new((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);

	public static TimeSpan ToHalfHour(this TimeSpan input)
	{
		int minutes = 30;
		var halfRange = new TimeSpan(0, minutes / 2, 0);
		if (input.Ticks < 0)
			halfRange = halfRange.Negate();
		var totalMinutes = (int)(input + halfRange).TotalMinutes;
		return new TimeSpan(0, totalMinutes - totalMinutes % minutes, 0);
	}
}