using Interim.Extensions;

namespace Interim.Features.TimeZones;

public static class TimeZoneInfoExtensions
{
	public static string ToNowAmPmString(this TimeZoneInfo zone) => TimeZoneInfo.ConvertTime(DateTime.Now, zone).ToHalfHour().ToAmPmString();
	public static string ToNowAmPmString(this TimeZoneInfo zone, out DateTime now, out DateTime converted)
	{
		now = DateTime.Now;
		return ToNowAmPmString(zone, now, out converted);
	}

	public static string ToNowAmPmString(this TimeZoneInfo zone, DateTime dateTime, out DateTime converted)
	{
		converted = TimeZoneInfo.ConvertTime(dateTime, zone).ToHalfHour();
		return converted.ToAmPmString();
	}

	public static bool HasApproximatelySameRules(this TimeZoneInfo zone, TimeZoneInfo other)
	{
		if (other == null)
		{
			throw new ArgumentNullException(nameof(other));
		}

		// check the utcOffset and supportsDaylightSavingTime members
		if (zone.BaseUtcOffset.ToHalfHour() != other.BaseUtcOffset.ToHalfHour() ||
		    zone.SupportsDaylightSavingTime != other.SupportsDaylightSavingTime)
		{
			return false;
		}

		bool IsRelevant(TimeZoneInfo.AdjustmentRule rule) => rule.DateEnd >= DateTime.Now;
		TimeZoneInfo.AdjustmentRule[] currentRules = zone.GetAdjustmentRules().Where(IsRelevant).ToArray();
		TimeZoneInfo.AdjustmentRule[] otherRules = other.GetAdjustmentRules().Where(IsRelevant).ToArray();
		return currentRules.AsSpan().SequenceEqual(otherRules);
	}

	public static void Compare(string id, string otherId)
	{
		var a = TimeZoneInfo.FindSystemTimeZoneById(id);
		var b = TimeZoneInfo.FindSystemTimeZoneById(otherId);
		if (a.BaseUtcOffset.ToHalfHour() != b.BaseUtcOffset.ToHalfHour())
		{
			Console.WriteLine("Times are different.");
			return;
		}
		if (a.SupportsDaylightSavingTime != b.SupportsDaylightSavingTime)
		{
			Console.WriteLine("DST is different.");
			return;
		}

		bool IsRelevant(TimeZoneInfo.AdjustmentRule rule) => rule.DateEnd >= DateTime.Now;
		TimeZoneInfo.AdjustmentRule[] currentRules = a.GetAdjustmentRules().Where(IsRelevant).ToArray();
		TimeZoneInfo.AdjustmentRule[] otherRules = b.GetAdjustmentRules().Where(IsRelevant).ToArray();

		if (!currentRules.AsSpan().SequenceEqual(otherRules))
		{
			Console.WriteLine("Rules are different.");
			return;
		}
		
		Console.WriteLine("Same?");
	}
}