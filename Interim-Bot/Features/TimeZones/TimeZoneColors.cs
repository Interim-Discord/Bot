namespace Interim.Features.TimeZones;

public static class TimeZoneColors
{
	private static readonly DiscordColor[] _colors =
	{
		new(143, 130, 255), // 12:00 -- Midnight : 8F82FF
		new(153, 141, 255), // 12:30
		new(163, 152, 255), // 01:00
		new(172, 164, 255), // 01:30
		new(181, 175, 255), // 02:00
		new(190, 187, 255), // 02:30
		new(198, 198, 255), // 03:00
		new(206, 210, 255), // 03:30
		new(213, 221, 255), // 04:00
		new(221, 233, 255), // 04:30
		new(228, 245, 255), // 05:00 : E4F5FF
		// Transition
		new(213, 193, 195), // 05:30
		new(192, 143, 137), // 06:00
		new(167, 94, 84), // 06:30 : A75E54
		// Transition
		new(180, 82, 61), // 07:00
		new(192, 67, 37), // 07:30
		new(202, 48, 5), // 08:00 : CA3005
		// Transition
		new(226, 40, 74), // 08:30
		new(233, 62, 130), // 09:00
		new(223, 94, 181), // 09:30
		new(198, 127, 222), // 10:00
		new(164, 155, 249), // 10:30
		new(129, 180, 255), // 11:00
		new(108, 200, 255), // 11:30
		new(113, 217, 255), // 12:00 -- Midday : 71D9FF
		// Transition
		new(96, 210, 255), // 12:30
		new(79, 203, 255), // 01:00
		new(61, 195, 255), // 01:30
		new(42, 187, 255), // 02:00
		new(20, 180, 255), // 02:30
		new(0, 171, 255), // 03:00
		new(0, 163, 255), // 03:30
		new(0, 154, 255), // 04:00 : 009AFF
		// Transition
		new(100, 158, 255), // 04:30
		new(143, 162, 255), // 05:00
		new(176, 166, 255), // 05:30
		new(204, 171, 255), // 06:00
		new(227, 177, 255), // 06:30 : E3B1FF
		// Transition
		new(234, 162, 221), // 07:00
		new(241, 144, 180), // 07:30
		new(248, 125, 128), // 08:00
		new(255, 102, 0), // 08:30 : FF6600
		// Transition
		new(223, 100, 128), // 09:00
		new(186, 99, 180), // 09:30
		new(139, 97, 221), // 10:00
		new(65, 95, 255), // 10:30 : 415FFF
		// Transition
		new(98, 106, 255), // 11:00
		new(122, 118, 255) // 11:30
	};
	
	public static DiscordColor GetColor(TimeOnly time)
	{
		int index = time.Hour * 2;
		if (time.Minute >= 30)
			index += 1;
		return _colors[index];
	}
}