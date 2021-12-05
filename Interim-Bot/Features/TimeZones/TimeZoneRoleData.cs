using System.Text.Json.Serialization;

namespace Interim.Features.TimeZones;

[Serializable]
public class TimeZoneRole
{
	public ulong ID { get; set;  }
	public string WindowsTimeZoneID { get; set;  }
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

	[JsonInclude]
	public List<TimeZoneRole> Roles { get; set; } = new();

	[JsonIgnore]
	private Dictionary<string, TimeZoneRole>? windowsTimeZoneIdToRole;
	[JsonIgnore]
	private Dictionary<string, TimeZoneRole> WindowsTimeZoneIdToRole
	{
		get
		{
			if (windowsTimeZoneIdToRole != null)
				return windowsTimeZoneIdToRole;

			windowsTimeZoneIdToRole = new Dictionary<string, TimeZoneRole>();
			foreach (TimeZoneRole role in Roles)
				windowsTimeZoneIdToRole.Add(role.WindowsTimeZoneID, role);
			return windowsTimeZoneIdToRole;
		}
	}

	public async ValueTask<DiscordRole> GetOrCreateRoleAsync(DiscordGuild guild, TimeZoneInfo zone)
	{
		await using SaveScope saveScope = new(this, guild);
		if (TryGetRole(guild, zone.Id, out DiscordRole? discordRole))
			return discordRole!;

		// If there is a preexisting time zone that has a role already, use that.
		var collapsedTimeZones = CollapsedTimeZones.Instance;
		HashSet<string> similarTimeZones = collapsedTimeZones.GetSimilarTimeZones(zone.Id);
		foreach (string similarTimeZone in similarTimeZones)
		{
			if (similarTimeZone == zone.Id) continue; // Ignore *this* timezone
			if (!TryGetRole(guild, similarTimeZone, out discordRole)) continue;
			AddRolesIfNotPresent(similarTimeZones, discordRole!.Id);
			return discordRole;
		}

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
			discordRole = guild.GetRole(role.ID);
			if (discordRole != null)
				return true;
			// Discord role was not found, update data structure.
			WindowsTimeZoneIdToRole.Remove(windowsTimeZoneID);
			Roles.Remove(role);
			SaveScope.Dirty();
		}

		discordRole = null;
		return false;
	}

	/// <summary>
	/// Adds <see cref="rolesToAdd"/> to this data structure if not already added, using the provided role <see cref="id"/>.
	/// </summary>
	private void AddRolesIfNotPresent(HashSet<string> rolesToAdd, ulong id)
	{
		foreach (string role in rolesToAdd)
			AddRoleIfNotPresent(role, id);
	}

	/// <summary>
	/// Adds <see cref="role"/> to this data structure if not already added, using the provided role <see cref="id"/>.
	/// </summary>
	private void AddRoleIfNotPresent(string role, ulong id)
	{
		if (WindowsTimeZoneIdToRole.TryGetValue(role, out TimeZoneRole? value))
		{
			Console.WriteLine($"\"{value.WindowsTimeZoneID}\" was ignored in a role update pass as it already exists.");
			return;
		}
		
#if DEBUG
		value = AddRole(role, id);
		Console.WriteLine($"\"{value.WindowsTimeZoneID}\" was added from a pre-existing role, discord role id: \"{id}\".");
#else
		AddRole(role, id);
#endif
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
		WindowsTimeZoneIdToRole.Add(windowsTimeZoneId, value);
		SaveScope.Dirty();
		return value;
	}

	private Task SaveAsync(DiscordGuild guild) => SaveLoad.SaveAsync(this, guild.Id, FileNameWithExtension, TimeZoneRoleDataJsonContext.Default);

	public static ValueTask<TimeZoneRoleData> LoadAsync(ulong guildId) => SaveLoad.LoadAsync<TimeZoneRoleData>(guildId, FileNameWithExtension, TimeZoneRoleDataJsonContext.Default);
	public static Task<string> LoadSerializedDataAsync(ulong guildId) => SaveLoad.LoadSerializedDataAsync(guildId, FileNameWithExtension);
}