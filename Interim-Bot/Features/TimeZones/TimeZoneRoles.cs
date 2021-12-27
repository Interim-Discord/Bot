using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;
using Interim.Commands;
using Interim.Extensions;
using TimeZoneConverter;

namespace Interim.Features.TimeZones;

/// <summary>
/// A singleton used as the entry point for managing <see cref="TimeZoneRoleData"/>.
/// </summary>
public class TimeZoneRoles : FeatureSingleton<TimeZoneRoles>, IComponentInteractionHandler
{
	public const string TimeZoneAssignmentInteractionId = "assign_timezone";
	public const string TimeZoneAssignmentInteractionIdReset = "assign_timezone_reset";
	private DiscordClient? _discordClient;
	private Dictionary<ulong, TimeZoneRoleData> GuildToRoleData { get; } = new();

	public async Task<TimeZoneRoleData> LoadAsync(ulong guildId)
	{
		var data = await TimeZoneRoleData.LoadAsync(guildId);
		GuildToRoleData.Add(guildId, data);
		return data;
	}

	public override async Task InitialiseAsync(DiscordClient discord, IBackgroundTaskQueue taskQueue)
	{
		_ = TZConvert.IanaToWindows("Etc/UTC"); // Call to initialise TZConvert.
		_ = CollapsedTimeZones.Instance; // Call to initialise CollapsedTimeZones.

		_discordClient = discord;
		List<Task> tasks = new();
		foreach (string directory in Directory.EnumerateDirectories(Constants.ApplicationDirectoryPath))
		{
			if (!ulong.TryParse(Path.GetFileName(directory), out ulong guildId)) continue;
			tasks.Add(LoadAsync(guildId));
		}

		await Task.WhenAll(tasks);
		await Task.Factory.StartNew(UpdateRolesLoopAsync, TaskCreationOptions.LongRunning);
		discord.GuildAvailable += OnGuildAvailable;
	}

	private Task OnGuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
	{
		DateTime now = DateTime.Now;
		DateTime target = now.RoundUp(TimeSpan.FromMinutes(30));
		TimeSpan delay = target - now;

		// If the next role update is over 5 minutes away, do one now.
		return delay > TimeSpan.FromMinutes(5) ? UpdateRolesAsync(now, true) : Task.CompletedTask;
	}

	public async Task<bool> OnComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e)
	{
		// Potentially the press of the timezone button.
		if (e.Id == TimeZoneAssignmentInteractionId)
			await HandleTimeAssignmentSelectedResponseAsync(e);
		else if (e.Id == TimeZoneAssignmentInteractionIdReset)
			await HandleTimeAssignmentSelectedResponseAsync(e, InteractionResponseType.UpdateMessage);
		else if (e.Id.StartsWith("continent_") || e.Id.StartsWith("country_"))
			await HandleGeneralSelectedResponseAsync(e, TimeZoneLookup.Groups);
		else if (e.Id.StartsWith("tz_"))
			await HandleLocalSelectedResponseAsync(e);
		else if (e.Id.StartsWith("dropdown_"))
		{
			string id = e.Values[0];
			if (id.StartsWith("continent_") || id.StartsWith("country_"))
				await HandleGeneralSelectedResponseAsync(e, TimeZoneLookup.Groups, id.FromInteractionId());
			else if (id.StartsWith("tz_"))
				await HandleLocalSelectedResponseAsync(e, id.FromInteractionId());
			else
				return false;
		}
		else if (InterimModule.ValidateRoleEraseId(e.Id))
		{
			List<Task> tasks = new();
			StringBuilder builder = new();
			builder.AppendLine("The roles:");
			foreach (DiscordRole role in e.Guild.Roles.Values.ToArray().Where(role => TimeRegex.IsMatch(role.Name)))
			{
				builder.Append("\t• ");
				builder.AppendLine(role.ToString());
				tasks.Add(role.DeleteAsync());
			}

			if (tasks.Count == 0)
			{
				await e.UpdateMessageEphemeral(":wastebasket: Role deletion", "No roles were found to delete.", DiscordColor.Green);
				return true;
			}

			await Task.WhenAll(tasks);
			builder.Append("were deleted.");
			await e.UpdateMessageEphemeral(":wastebasket: Role deletion", builder.ToString().Truncate(2000)!, DiscordColor.Green);
		}
		else
			return false;

		return true;
	}

	private Task HandleTimeAssignmentSelectedResponseAsync(ComponentInteractionCreateEventArgs e,
		InteractionResponseType responseType = InteractionResponseType.ChannelMessageWithSource)
	{
		e.Handled = true;
		var builder = new DiscordInteractionResponseBuilder()
			.AsEphemeral(true)
			.WithContent("Which continent are you on?");

		foreach (Locale[] continents in TimeZoneLookup.Continents)
		{
			builder.AddComponents(
				continents.Select(c => c.ToButton(_discordClient!))
			);
		}

		return e.Interaction.CreateResponseAsync(responseType, builder);
	}

	private Task HandleGeneralSelectedResponseAsync(ComponentInteractionCreateEventArgs e, Dictionary<string, LocalGroup> context, string? idOverride = null)
	{
		if (!context.TryGetValue(idOverride ?? e.FromInteractionId(), out var localData))
		{
			return e.Interaction.CreateResponseAsync(
				InteractionResponseType.UpdateMessage,
				new DiscordInteractionResponseBuilder()
					.AsEphemeral(true)
					.AddEmbed(
						new DiscordEmbedBuilder()
							.WithTitle("Failure!")
							.WithDescription($"Internal failure for {e.Id}. Please let @vertx#7893 know!")
					)
			);
		}

		e.Handled = true;

		var builder = new DiscordInteractionResponseBuilder()
			.AsEphemeral(true)
			.WithContent(localData.Question);

		switch (localData.Mode)
		{
			case InteractionMode.Buttons:
			{
				foreach (Locale[] locale in localData.Locales)
				{
					builder.AddComponents(
						locale.Select(c => c.ToButton(_discordClient!))
					);
				}

				break;
			}
			case InteractionMode.Dropdowns:
				for (var i = 0; i < localData.Locales.Length; i++)
				{
					Locale[] locale = localData.Locales[i];
					char from = i == 0 ? 'A' : locale[0].Name[0];
					char to = i == localData.Locales.Length - 1 ? 'Z' : locale[^1].Name[0];
					builder.AddComponents(
						new DiscordSelectComponent(
							"dropdown_".ToInteractionId(),
							$"{from} to {to}",
							locale.Select(c => c.ToSelectOption(_discordClient!)
							))
					);
				}

				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		return e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, builder);
	}

	public enum Support
	{
		Invalid,
		Valid,
		Custom
	}

	public static Support TryGetTimeZoneInfo(string ianaTimeZoneId, out TimeZoneInfo timeZoneInfo)
	{
		if (TZConvert.TryGetTimeZoneInfo(ianaTimeZoneId, out timeZoneInfo))
			return Support.Valid;
		return ianaTimeZoneId == "Antarctica/Troll" ? Support.Custom : Support.Invalid;
	}

	private async Task HandleLocalSelectedResponseAsync(ComponentInteractionCreateEventArgs e, string? idOverride = null)
	{
		string ianaTimeZoneId = (idOverride ?? e.FromInteractionId())["tz_".Length..];
		switch (TryGetTimeZoneInfo(ianaTimeZoneId, out TimeZoneInfo timeZoneInfo))
		{
			case Support.Invalid:
				await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
					new DiscordInteractionResponseBuilder()
						.AsEphemeral(true)
						.AddEmbed(
							new DiscordEmbedBuilder()
								.WithTitle("Failure!")
								.WithDescription($"Internal failure for {e.Id}. Please let @vertx#7893 know!")
						)
				);
				return;
			case Support.Valid:
				break;
			case Support.Custom:
				await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
					new DiscordInteractionResponseBuilder()
						.AsEphemeral(true)
						.AddEmbed(
							new DiscordEmbedBuilder()
								.WithTitle("Unsupported!")
								.WithDescription($"Sorry, {ianaTimeZoneId} is not supported!")
						).AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, TimeZoneAssignmentInteractionIdReset, "Return"))
				);
				return;
			default:
				throw new ArgumentOutOfRangeException();
		}

		var task = e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
			new DiscordInteractionResponseBuilder()
				.AsEphemeral(true)
				.AddEmbed(
					new DiscordEmbedBuilder()
						.WithTitle(":clock4: You have been assigned a new role!")
						.WithDescription("You can safely dismiss this message.")
				)
		);

		await Task.WhenAll(
			task,
			AssignTimeZoneAsync(e.Guild, (DiscordMember)e.User, timeZoneInfo)
		);
	}

	private async Task UpdateRolesLoopAsync()
	{
		DateTime now = DateTime.Now;
		TimeSpan delay = GetDelayUntilNext15Min(now);

		// If the next role update is over 15 minutes away, do one now.
		if (delay > TimeSpan.FromMinutes(15))
			await UpdateRolesAsync(now, true);

		while (true)
		{
			await Task.Delay(delay);
			now = DateTime.Now;
			delay = GetDelayUntilNext15Min(now);
			Console.WriteLine($"Updating roles at {now}.");
			try
			{
				await UpdateRolesAsync(now);
				Console.WriteLine("Roles updated.");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				delay = TimeSpan.FromMinutes(1); // Try again in a minute.
			}
		}
		// ReSharper disable once FunctionNeverReturns
	}

	private static TimeSpan GetDelayUntilNext15Min(DateTime time)
	{
		int minute = time.Minute;
		return minute switch
		{
			< 15 => TimeSpan.FromMinutes(15 - minute),
			< 45 => TimeSpan.FromMinutes(45 - minute),
			_ => TimeSpan.FromMinutes(75 - minute)
		};
	}

	private Task UpdateRolesAsync(DateTime now, bool force = false)
	{
		StringBuilder text = new StringBuilder();
		List<KeyValuePair<ulong, TimeZoneRoleData>> pairs = GuildToRoleData.ToList();
		List<Task> modifications = new List<Task>();
		foreach ((ulong guildId, TimeZoneRoleData roleData) in pairs)
		{
			if (!_discordClient!.Guilds.TryGetValue(guildId, out var guild)) continue;
			if (roleData.Roles.Count == 0) continue;
			bool colored = Preferences.Preferences.Instance.IsTimeZoneColorsEnabled(guildId);
			text.AppendLine($"Updating roles in {guild.Name}:");
			foreach (var role in roleData.Roles)
			{
				if (!guild.Roles.TryGetValue(role.ID, out var discordRole)) continue;
				string name = role.TimeZoneInfo.ToNowAmPmString(now, out DateTime converted);
				if (!force && discordRole.Name == name) continue;
				if (colored)
				{
					DiscordColor colour = TimeZoneColors.GetColor(TimeOnly.FromDateTime(converted));
					text.AppendLine($"\tUpdating {discordRole.Name} ({role.ID}) to {name} : {colour}.");
					modifications.Add(discordRole.ModifyAsync(name, color: colour));
				}
				else
				{
					text.AppendLine($"\tUpdating {discordRole.Name} ({role.ID}) to {name}.");
					modifications.Add(discordRole.ModifyAsync(name));
				}
			}
		}

		Console.Write(text);

		return Task.WhenAll(modifications);
	}

	public Task UpdateRolesAsync(ulong guildId, DateTime now, bool force = false)
	{
		if (!GuildToRoleData.TryGetValue(guildId, out var roleData)) return Task.CompletedTask;
		if (!_discordClient!.Guilds.TryGetValue(guildId, out var guild)) return Task.CompletedTask;
		bool colored = Preferences.Preferences.Instance.IsTimeZoneColorsEnabled(guildId);
		List<Task> modifications = new List<Task>();
		foreach (var role in roleData.Roles)
		{
			if (!guild.Roles.TryGetValue(role.ID, out var discordRole)) continue;
			string name = role.TimeZoneInfo.ToNowAmPmString(now, out DateTime converted);
			if (!force && discordRole.Name == name) continue;
			modifications.Add(!colored ? discordRole.ModifyAsync(name) : discordRole.ModifyAsync(name, color: TimeZoneColors.GetColor(TimeOnly.FromDateTime(converted))));
		}

		return Task.WhenAll(modifications);
	}


	public Task AssignTimeZoneAsync(CommandContext ctx, TimeZoneInfo timeZoneInfo) => AssignTimeZoneAsync(ctx.Guild, ctx.Member, timeZoneInfo);

	public static readonly Regex TimeRegex = new("^[0-9]{2}:[0-9]{2} [AP]M$", RegexOptions.Compiled);

	public async Task AssignTimeZoneAsync(DiscordGuild guild, DiscordMember member, TimeZoneInfo timeZoneInfo)
	{
		DiscordRole role = await GetRoleAsync(guild, timeZoneInfo);

		// Remove previously assigned roles that look exactly like times.
		foreach (DiscordRole assignedRole in member.Roles)
		{
			if (!TimeRegex.IsMatch(assignedRole.Name))
				continue;
			Console.WriteLine($"Removed the role \"{role}\" from \"{member}\".");
			await member.RevokeRoleAsync(assignedRole);
		}

		Console.WriteLine($"Assigned \"{member}\" the role \"{role}\".");

		// Grant this new timezone role.
		await member.GrantRoleAsync(role);
	}

	private async ValueTask<DiscordRole> GetRoleAsync(DiscordGuild guild, TimeZoneInfo timeZoneInfo)
	{
		ulong guildId = guild.Id;
		// Get the role data associated with the guild.
		if (!GuildToRoleData.TryGetValue(guildId, out TimeZoneRoleData? roleData))
			roleData = await LoadAsync(guildId);

		return await roleData.GetOrCreateRoleAsync(guild, timeZoneInfo);
	}
}