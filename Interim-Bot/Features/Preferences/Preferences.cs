using System.Text.Json;
using Interim.Extensions;
using Interim.Features.Threads;
using Interim.Features.TimeZones;

namespace Interim.Features.Preferences;

[Serializable]
public class PreferenceData
{
	public ulong LoggingChannel { get; set; }
	public bool DeleteThreadStartedMessages { get; set; }
	public bool TimeZoneColors { get; set; }
}

public class Preferences : FeatureSingleton<Preferences>
{
	public const string FileNameWithExtension = "preferences.json";
	private Dictionary<ulong, PreferenceData> GuildToPreferenceData { get; } = new();
	public IEnumerable<ulong> ServersWithThreadMessageDeletion =>
		GuildToPreferenceData
			.Where(pair => pair.Value.DeleteThreadStartedMessages)
			.Select(pair => pair.Key);

	private Task SaveAsync(DiscordGuild guild)
	{
		ulong guildId = guild.Id;
		if (!GuildToPreferenceData.TryGetValue(guildId, out var data)) return Task.CompletedTask;
		return SaveLoad.SaveAsync(data, guildId, FileNameWithExtension, PreferenceDataJsonContext.Default);
	}

	private async Task<PreferenceData> LoadAsync(ulong guildId)
	{
		var data = await SaveLoad.LoadAsync<PreferenceData>(guildId, FileNameWithExtension, PreferenceDataJsonContext.Default);
		GuildToPreferenceData.Add(guildId, data);
		return data;
	}

	public override async Task InitialiseAsync(DiscordClient discord)
	{
		Console.WriteLine($"Save location: \"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\".");

		List<Task> tasks = new();
		string path = Constants.ApplicationDirectoryPath;
		Directory.CreateDirectory(path);
		foreach (string directory in Directory.EnumerateDirectories(path))
		{
			if (!ulong.TryParse(Path.GetFileName(directory), out ulong guildId)) continue;
			tasks.Add(LoadAsync(guildId));
		}

		await Task.WhenAll(tasks);

		// discord.GuildAvailable += OnGuildAvailable
	}

	public async Task ChangeLoggingChannelAsync(CommandContext ctx)
	{
		if (!GuildToPreferenceData.TryGetValue(ctx.Guild.Id, out var data))
			data = await LoadAsync(ctx.Guild.Id);
		if (data.LoggingChannel == ctx.Channel.Id) return;
		data.LoggingChannel = ctx.Channel.Id;
		await SaveAsync(ctx.Guild);
	}

	public string GetJsonToPrint(CommandContext ctx)
		=> !GuildToPreferenceData.TryGetValue(ctx.Guild.Id, out var prefs) ? "No Json" : JsonSerializer.Serialize(prefs, typeof(PreferenceData), PreferenceDataJsonContext.Default);

	public async Task SetThreadMessageDeletionState(CommandContext ctx, bool enabled)
	{
		if (!GuildToPreferenceData.TryGetValue(ctx.Guild.Id, out var data))
			data = await LoadAsync(ctx.Guild.Id);
		if (data.DeleteThreadStartedMessages == enabled)
			return;
		data.DeleteThreadStartedMessages = enabled;
		await SaveAsync(ctx.Guild);
		ThreadStartedDeleter.Instance.ModifiedGuild(ctx.Guild.Id, enabled);
	}

	public async Task SetTimeZoneColoursState(CommandContext ctx, bool enabled)
	{
		if (!GuildToPreferenceData.TryGetValue(ctx.Guild.Id, out var data))
			data = await LoadAsync(ctx.Guild.Id);
		if (data.TimeZoneColors == enabled)
			return;
		data.TimeZoneColors = enabled;
		await SaveAsync(ctx.Guild);
		await TimeZoneRoles.Instance.UpdateRolesAsync(ctx.Guild.Id, DateTime.Now.RoundUp(TimeSpan.FromMinutes(30)), true);
	}

	public bool IsTimeZoneColorsEnabled(ulong guildId) => GuildToPreferenceData.TryGetValue(guildId, out var data) && data.TimeZoneColors;

	public bool GetLoggingChannel(DiscordGuild guild, out DiscordChannel? channel)
	{
		if (GuildToPreferenceData.TryGetValue(guild.Id, out var data))
			return guild.Channels.TryGetValue(data.LoggingChannel, out channel);
		channel = null;
		return false;
	}
}