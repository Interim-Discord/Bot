using DSharpPlus.EventArgs;

namespace Interim.Extensions;

public static class CommandInteractionExtensions
{
	public static Task UpdateMessageEphemeral(this ComponentInteractionCreateEventArgs e, string title, string description, DiscordColor color) =>
		e.Interaction.CreateResponseAsync(
			InteractionResponseType.UpdateMessage,
			new DiscordInteractionResponseBuilder()
				.WithContent(string.Empty)
				.AsEphemeral(true)
				.AddEmbed(
					new DiscordEmbedBuilder()
						.WithTitle(title)
						.WithDescription(description)
						.WithColor(color)
				)
		);
}