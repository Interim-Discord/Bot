using Interim.Extensions;

namespace Interim.Commands;

[UsedImplicitly]
public class InterimModule : BaseCommandModule
{
	private const string InteractionEraseId = "interim_Erase";
	private const string InteractionRoleEraseId = "interim_RoleErase";
	private static HashSet<string>? _validEraseGuids;

	[Command("interim"), UsedImplicitly, RequireUserPermissions(Permissions.Administrator)]
	public Task SetupCommand(CommandContext ctx, [RemainingText] string context) => CommandsHandler.Handle(ctx, context, commands, "interim");

	private static readonly Dictionary<string, Func<CommandContext, ValueTask>> commands = new()
	{
		{ "tear down data", TearDown },
		{ "tear down roles", TearDownRoles },
	};

	private static async ValueTask TearDown(CommandContext arg)
	{
		_validEraseGuids ??= new HashSet<string>();
		string eraseId = InteractionEraseId.ToInteractionId(out string eraseGuid);
		_validEraseGuids.Add(eraseGuid);
		await arg.RespondAsync(
			new DiscordMessageBuilder()
				.WithContent("This will remove all serialized data for this server.\nDo you wish to continue?")
				.AddComponents(
					new DiscordButtonComponent(ButtonStyle.Danger, eraseId, "Erase"),
					new DiscordButtonComponent(ButtonStyle.Secondary, Constants.InteractionCancelId, "Cancel")
				)
		);
	}
	
	private static async ValueTask TearDownRoles(CommandContext arg)
	{
		_validEraseGuids ??= new HashSet<string>();
		string eraseId = InteractionRoleEraseId.ToInteractionId(out string eraseGuid);
		_validEraseGuids.Add(eraseGuid);
		await arg.RespondAsync(
			new DiscordMessageBuilder()
				.WithContent("This will remove all time related roles (roles that look like times, \"00:00 AM/PM\") from the server.\nDo you wish to continue?")
				.AddComponents(
					new DiscordButtonComponent(ButtonStyle.Danger, eraseId, "Remove"),
					new DiscordButtonComponent(ButtonStyle.Secondary, Constants.InteractionCancelId, "Cancel")
				)
		);
	}
	
	public static bool ValidateEraseId(string id) => ValidateEraseId(InteractionEraseId, id);
	public static bool ValidateRoleEraseId(string id) => ValidateEraseId(InteractionRoleEraseId, id);
	
	/// <summary>
	/// Only returns true if the ID was created with the erase command generated by a tear-down message.
	/// </summary>
	private static bool ValidateEraseId(string prefix, string id)
	{
		if (!id.StartsWith(prefix)) return false;
		id.FromInteractionId(out string guid);
		return _validEraseGuids != null && _validEraseGuids.Remove(guid);
	}
}