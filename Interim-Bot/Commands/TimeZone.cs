using Interim.Extensions;
using Interim.Features.TimeZones;

namespace Interim.Commands;

[UsedImplicitly]
public class TimeZoneModule : BaseCommandModule
{
	[Command("timezone"), UsedImplicitly]
	public async Task TimeZoneCommand(CommandContext ctx, string context)
	{
		try
		{
			switch (TimeZoneRoles.TryGetTimeZoneInfo(context, out TimeZoneInfo timeZoneInfo))
			{
				case TimeZoneRoles.Support.Invalid:
					await ctx.LogError($"\"{context}\" is not a valid time zone or IANA time zone ID.");
					return;
				case TimeZoneRoles.Support.Valid:
					break;
				case TimeZoneRoles.Support.Custom:
					await ctx.LogError($"\"{context}\" is not supported, sorry!");
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}

			await TimeZoneRoles.Instance.AssignTimeZoneAsync(ctx, timeZoneInfo);
		}
		catch (Exception e)
		{
			await ctx.LogException(e);
		}
	}

	public static async ValueTask SetupTimeZoneAssignment(CommandContext ctx) =>
		await Task.WhenAll(
			ctx.Message.DeleteAsync(),
			new DiscordMessageBuilder()
				.AddEmbed(
					new DiscordEmbedBuilder()
						.WithColor(new DiscordColor(1f, 0.6f, 0.1f))
						.WithTitle(":clock4: Local Time")
						.WithDescription("Using a time zone, your role will update with a local time.")
				)
				.AddComponents(
					new DiscordButtonComponent(ButtonStyle.Primary, TimeZoneRoles.TimeZoneAssignmentInteractionId, "Assign me a time!")
				).SendAsync(ctx.Channel)
		);
}