namespace Interim.Extensions;

public static class CommandContextExtensions
{
	public static Task LogException(this CommandContext ctx, Exception e)
	{
		DiscordEmbedBuilder embed = new DiscordEmbedBuilder
		{
			Title = e.GetType().Name,
			Color = new DiscordColor(1, 0.1f, 0.2f),
			Description = e.Message.Truncate(2000)
		};
#if DEBUG
		embed.AddField("Stack Trace", e.StackTrace.ToDiscordCode().Truncate(2000));
#endif
		return new DiscordMessageBuilder().WithEmbed(embed).SendAsync(ctx.Channel);
	}

	public static Task Log(this CommandContext ctx, string message)
	{
		DiscordEmbedBuilder embed = new DiscordEmbedBuilder
		{
			Color = new DiscordColor(1f, 1f, 1f)
		};
		embed.AddField("Log", message.Truncate(2000));
		return new DiscordMessageBuilder().WithEmbed(embed).SendAsync(ctx.Channel);
	}

	public static Task LogError(this CommandContext ctx, string message)
	{
		DiscordEmbedBuilder embed = new DiscordEmbedBuilder
		{
			Color = new DiscordColor(1, 0.1f, 0.2f)
		};
		embed.AddField("Error", message.Truncate(2000));
		return new DiscordMessageBuilder().WithEmbed(embed).SendAsync(ctx.Channel);
	}

	public static Task LogWarning(this CommandContext ctx, string message)
	{
		DiscordEmbedBuilder embed = new DiscordEmbedBuilder
		{
			Color = new DiscordColor(1, 0.6f, 0.2f)
		};
		embed.AddField("Warning", message.Truncate(2000));
		return new DiscordMessageBuilder().WithEmbed(embed).SendAsync(ctx.Channel);
	}
}