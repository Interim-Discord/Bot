using System.Collections.ObjectModel;

namespace Interim.Features.TimeZones;

public class CollapsedTimeZones
{
	public static CollapsedTimeZones Instance { get; }
	public HashSet<string>[] ZoneGroups { get; }

	static CollapsedTimeZones() => Instance = new CollapsedTimeZones();
	
	private CollapsedTimeZones()
	{
		ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();
		Dictionary<TimeZoneInfo, HashSet<string>> processedZones = new();
		foreach (TimeZoneInfo zone in zones)
		{
			TimeZoneInfo? sameZone = null;
			foreach (var processedZoneGroup in processedZones)
			{
				if (!zone.HasApproximatelySameRules(processedZoneGroup.Key))
					continue;
				sameZone = processedZoneGroup.Key;
				break;
			}

			if (sameZone == null)
				processedZones.Add(zone, new HashSet<string> { zone.Id });
			else
				processedZones[sameZone].Add(zone.Id);
		}

		ZoneGroups = processedZones.Values.ToArray();
		foreach (HashSet<string> group in ZoneGroups)
		{
			string first = group.First();
			string time = TimeZoneInfo.FindSystemTimeZoneById(first).ToNowAmPmString();
			foreach (string zone in group)
			{
				string compareTime = TimeZoneInfo.FindSystemTimeZoneById(zone).ToNowAmPmString();
				if (time != compareTime)
					Console.WriteLine($"!! {first} : {time} does not equal {zone} : {compareTime}");
			}
		}
	}

	public HashSet<string> GetSimilarTimeZones(string windowsTimeZoneID)
	{
		foreach (HashSet<string> zoneGroup in ZoneGroups)
			if (zoneGroup.Contains(windowsTimeZoneID))
				return zoneGroup;
		Console.WriteLine($"Unsupported timezone was queried: \"{windowsTimeZoneID}\".");
		return new HashSet<string>();
	}
}