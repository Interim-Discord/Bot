using Interim.Extensions;

namespace Interim.Commands;

public static class CommandsHandler
{
	public static async Task Handle(
		CommandContext ctx,
		string context,
		Dictionary<string, Func<CommandContext, ValueTask>> commands,
		string name
	)
	{
		string input = context.ToLowerInvariant();
		if (commands.TryGetValue(input, out var command))
		{
			try
			{
				await command.Invoke(ctx);
			}
			catch (Exception e)
			{
				await ctx.LogException(e);
			}

			return;
		}

		StringBuilder response = input == "help" ?
			new StringBuilder($"The options for {name} are:") :
			new StringBuilder($"{name} \"{context}\" was not found. The options are:");
		response.AppendLine();
		foreach (var logCommand in commands)
		{
			response.Append("- ");
			response.AppendLine(logCommand.Key);
		}

		await ctx.Log(response.ToString());
	}
}