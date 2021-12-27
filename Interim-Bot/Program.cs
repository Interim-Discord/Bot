using Interim.Commands;
using static Interim.Features.Features;

namespace Interim;

class Program
{
	static async Task Main(string[] args)
	{
		string? token = null;
		for (int i = 0; i < args.Length; i++)
		{
			if (!args[i].StartsWith("-token")) continue;
			token = args[i + 1];
			break;
		}

		if (string.IsNullOrEmpty(token))
		{
			Console.WriteLine("No token was provided as an argument to the program. \"-token BOT_TOKEN\"");
			return;
		}

		var discord = new DiscordClient(new DiscordConfiguration
		{
			Token = token,
			TokenType = TokenType.Bot,
			Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMessages
		});

		await discord.ConnectAsync();
		await InitialiseAsync(discord, new BackgroundTaskQueue(5));

		var commands = discord.UseCommandsNext(new CommandsNextConfiguration
		{
			StringPrefixes = new[] { "!" }
		});


		commands.RegisterCommands<LogModule>();
		commands.RegisterCommands<TimeZoneModule>();
		commands.RegisterCommands<SetupModule>();
		commands.RegisterCommands<InterimModule>();

		await Task.Delay(-1);
	}
}