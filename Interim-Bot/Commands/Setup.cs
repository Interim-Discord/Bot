using Interim.Extensions;
using Interim.Features.Preferences;

namespace Interim.Commands;

[UsedImplicitly]
public class SetupModule : BaseCommandModule
{
	[Command("setup"), UsedImplicitly, RequireUserPermissions(Permissions.Administrator)]
	public Task SetupCommand(CommandContext ctx, [RemainingText] string context) => CommandsHandler.Handle(ctx, context, commands, "setup");
	
	private static readonly Dictionary<string, Func<CommandContext, ValueTask>> commands = new()
	{
		{ "log", SetupLoggingChannel },
		{ "thread started deletion on", SetupThreadStartedDeletion },
		{ "thread started deletion off", DisableThreadStartedDeletion },
		{ "time zone assignment", TimeZoneModule.SetupTimeZoneAssignment },
		{ "time zone colors on", SetupTimeZoneColours },
		{ "time zone colors off", DisableTimeZoneColours },
		{ "time zone colours on", SetupTimeZoneColours },
		{ "time zone colours off", DisableTimeZoneColours },
	};

	// Logging
	private static async ValueTask SetupLoggingChannel(CommandContext ctx)
	{
		Preferences prefs = Preferences.Instance;
		await prefs.ChangeLoggingChannelAsync(ctx);
		await ctx.Log($"Log channel changed to {ctx.Channel.Name}");
	}
	
	// Thread Started
	private static async ValueTask SetupThreadStartedDeletion(CommandContext ctx)
	{
		Preferences prefs = Preferences.Instance;
		await prefs.SetThreadMessageDeletionState(ctx, true);
		await ctx.Log(":green_circle: Thread message deletion has been enabled.");
	}
	
	private static async ValueTask DisableThreadStartedDeletion(CommandContext ctx)
	{
		Preferences prefs = Preferences.Instance;
		await prefs.SetThreadMessageDeletionState(ctx, false);
		await ctx.Log(":red_circle: Thread message deletion has been disabled.");
	}
	
	// Time Zone Colours
	private static async ValueTask SetupTimeZoneColours(CommandContext ctx)
	{
		Preferences prefs = Preferences.Instance;
		await prefs.SetTimeZoneColoursState(ctx, true);
		await ctx.Log(":green_circle: Time zone colours have been enabled.");
	}
	
	// Time Zone Colours
	private static async ValueTask DisableTimeZoneColours(CommandContext ctx)
	{
		Preferences prefs = Preferences.Instance;
		await prefs.SetTimeZoneColoursState(ctx, false);
		await ctx.Log(":red_circle: Time zone colours have been disabled.");
	}
}