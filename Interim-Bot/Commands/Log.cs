using Interim.Extensions;
using Interim.Features.Preferences;
using Interim.Features.TimeZones;

namespace Interim.Commands;

[UsedImplicitly]
public class LogModule : BaseCommandModule
{
	[Command("log"), UsedImplicitly, RequireUserPermissions(Permissions.Administrator)]
	public Task SetupCommand(CommandContext ctx, [RemainingText] string context) => CommandsHandler.Handle(ctx, context, commands, "log");

	private static readonly Dictionary<string, Func<CommandContext, ValueTask>> commands = new()
	{
		{ "ping", async ctx => await ctx.RespondAsync("pong") },
		// { "timezones", LogTimezones },
		{ "time zone roles", LogTimezoneRoles },
		{ "preferences", async ctx => await ctx.Log(Preferences.Instance.GetJsonToPrint(ctx)) },
	};

	private static async ValueTask LogTimezoneRoles(CommandContext arg)
	{
		string data = await TimeZoneRoleData.LoadSerializedDataAsync(arg.Guild.Id);
		await LogAttachmentAsync(arg.Channel, TimeZoneRoleData.FileNameWithExtension, "Current Time Zones to Role IDs.", data);
	}

	private static async ValueTask LogTimezones(CommandContext ctx)
	{
		StringBuilder text = new StringBuilder(2000);
		CollapsedTimeZones collapsedTimeZones = CollapsedTimeZones.Instance;
		DiscordThreadChannel thread = await ctx.Channel.CreateThreadAsync("Timezones", AutoArchiveDuration.Hour, ChannelType.PublicThread);
		foreach (var zoneGroup in collapsedTimeZones.ZoneGroups)
		{
			text.Clear();
			foreach (string zone in zoneGroup)
				text.AppendLine(zone);
			DiscordMessageBuilder messageBuilder = new()
			{
				Content = text.ToString().Truncate(2000)
			};
			await thread.SendMessageAsync(messageBuilder);
		}
	}

	public static Task LogAttachmentAsync(DiscordGuild guild, string fileName, string content, Func<string> getAttachment) =>
		!Preferences.Instance.GetLoggingChannel(guild, out DiscordChannel? channel) ? Task.CompletedTask : LogAttachmentAsync(channel!, fileName, content, getAttachment());

	public static async Task LogAttachmentAsync(DiscordChannel channel, string fileName, string content, string attachment)
	{
		byte[] byteArray = Encoding.ASCII.GetBytes(attachment);
		await using MemoryStream stream = new MemoryStream(byteArray);
		await new DiscordMessageBuilder
		{
			Content = content
		}.WithFile(fileName, stream).SendAsync(channel);
	}
}