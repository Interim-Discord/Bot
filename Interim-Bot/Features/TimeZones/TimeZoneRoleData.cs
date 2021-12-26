using System.Text.Json.Serialization;

namespace Interim.Features.TimeZones;

[Serializable]
public class TimeZoneRole
{
	public ulong ID { get; set; }
	public string WindowsTimeZoneID { get; set; }
	[JsonIgnore]
	private TimeZoneInfo? _timeZoneInfo;
	[JsonIgnore]
	public TimeZoneInfo TimeZoneInfo => _timeZoneInfo ??= TimeZoneInfo.FindSystemTimeZoneById(WindowsTimeZoneID);

	public TimeZoneRole(ulong id, string windowsTimeZoneID)
	{
		ID = id;
		WindowsTimeZoneID = windowsTimeZoneID;
	}
}

[Serializable]
public class TimeZoneRoleData
{
	public const string FileNameWithExtension = "role-data.json";

	/// <summary>
	/// A collapsed structure that only contains the bare minimum structure.
	/// See <see cref="CollapsedTimeZones"/> to understand how this is collapsed.
	/// </summary>
	[JsonInclude]
	public List<TimeZoneRole> Roles { get; set; } = new();

	[JsonIgnore]
	private Dictionary<string, TimeZoneRole>? windowsTimeZoneIdToRole;

	/// <summary>
	/// An expanded structure that maps Windows Time Zone IDs to role data.
	/// </summary>
	[JsonIgnore]
	public IReadOnlyDictionary<string, TimeZoneRole> WindowsTimeZoneIdToRole => windowsTimeZoneIdToRole ?? InitialiseWindowsTimeZoneIdToRole();

	private Dictionary<string, TimeZoneRole> InitialiseWindowsTimeZoneIdToRole()
	{
		windowsTimeZoneIdToRole = new Dictionary<string, TimeZoneRole>();
		foreach (TimeZoneRole role in Roles)
			AddToWindowsTimeZoneIdToRole(role);

		return windowsTimeZoneIdToRole;
	}

	private void AddToWindowsTimeZoneIdToRole(TimeZoneRole role)
	{
		windowsTimeZoneIdToRole ??= InitialiseWindowsTimeZoneIdToRole();

		foreach (var windowsTimeZoneId in CollapsedTimeZones.Instance.GetSimilarTimeZones(role.WindowsTimeZoneID))
		{
			if (windowsTimeZoneIdToRole.ContainsKey(windowsTimeZoneId)) continue;
			windowsTimeZoneIdToRole.Add(windowsTimeZoneId, role);
		}
	}

	/// <summary>
	/// Gets a <see cref="DiscordRole"/> if it already exists for <see cref="zone"/>.
	/// If there is no role, a new one is created.
	/// </summary>
	public async ValueTask<DiscordRole> GetOrCreateRoleAsync(DiscordGuild guild, TimeZoneInfo zone)
	{
		await using SaveScope saveScope = new(this, guild);
		if (TryGetRole(guild, zone.Id, out DiscordRole? discordRole))
			return discordRole!;

		// There is no valid role, create one.
		return await CreateRoleAsync(guild, zone);
	}

	private struct SaveScope : IAsyncDisposable
	{
		private static bool dirty;
		private readonly TimeZoneRoleData roleData;
		private readonly DiscordGuild discordGuild;

		public SaveScope(TimeZoneRoleData roleData, DiscordGuild discordGuild)
		{
			this.roleData = roleData;
			this.discordGuild = discordGuild;
		}

		public static void Dirty() => dirty = true;

		public async ValueTask DisposeAsync()
		{
			if (dirty)
				await roleData.SaveAsync(discordGuild);
			dirty = false;
		}
	}

	/// <summary>
	/// Attempts to get a <see cref="DiscordRole"/> if one already exists for this time zone.
	/// </summary>
	private bool TryGetRole(DiscordGuild guild, string windowsTimeZoneID, out DiscordRole? discordRole)
	{
		if (WindowsTimeZoneIdToRole.TryGetValue(windowsTimeZoneID, out TimeZoneRole? role))
		{
			// Role was found, try to get discord role.
			if (guild.Roles.TryGetValue(role.ID, out discordRole))
				return true;
			// Discord role was not found, update data structure.
			Roles.Remove(role);
			// Reinitialise the role lookup structure.
			InitialiseWindowsTimeZoneIdToRole();
			SaveScope.Dirty();
		}

		discordRole = null;
		return false;
	}

	public async ValueTask<DiscordRole> CreateRoleAsync(DiscordGuild guild, TimeZoneInfo zone)
	{
		string name = zone.ToNowAmPmString(out _, out DateTime converted);
		DiscordRole newRole;
		if (Preferences.Preferences.Instance.IsTimeZoneColorsEnabled(guild.Id))
		{
			DiscordColor color = TimeZoneColors.GetColor(TimeOnly.FromDateTime(converted));
			newRole = await guild.CreateRoleAsync(name, mentionable: false, color: color, reason: zone.Id);
		}
		else
		{
			newRole = await guild.CreateRoleAsync(name, mentionable: false, reason: zone.Id);
		}

		AddRole(zone.Id, newRole.Id);
		Console.WriteLine($"\"{zone.Id}\" was created, discord role: \"{newRole}\".");
		return newRole;
	}

	public TimeZoneRole AddRole(string windowsTimeZoneId, ulong id)
	{
		TimeZoneRole value = new TimeZoneRole(id, windowsTimeZoneId);
		Roles.Add(value);
		AddToWindowsTimeZoneIdToRole(value);
		SaveScope.Dirty();
		return value;
	}

	private Task SaveAsync(DiscordGuild guild) => SaveLoad.SaveAsync(this, guild.Id, FileNameWithExtension, TimeZoneRoleDataJsonContext.Default);

	public static ValueTask<TimeZoneRoleData> LoadAsync(ulong guildId) => SaveLoad.LoadAsync<TimeZoneRoleData>(guildId, FileNameWithExtension, TimeZoneRoleDataJsonContext.Default);
	public static Task<string> LoadSerializedDataAsync(ulong guildId) => SaveLoad.LoadSerializedDataAsync(guildId, FileNameWithExtension);
}