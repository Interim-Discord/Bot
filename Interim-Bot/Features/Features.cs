using DSharpPlus.EventArgs;
using Interim.Commands;
using Interim.Extensions;
using Interim.Features.Threads;
using Interim.Features.TimeZones;

namespace Interim.Features;

public static class Features
{
	private static QueuedHostedService? hostingService;
	private static IEnumerable<IFeature> Instances => new List<IFeature>
	{
		TimeZoneRoles.Instance,
		ThreadStartedDeleter.Instance
	};

	public static async Task InitialiseAsync(DiscordClient discord, IBackgroundTaskQueue taskQueue)
	{
		hostingService = new QueuedHostedService(taskQueue);
		await Task.Run(() => hostingService.StartAsync(CancellationToken.None));
		
		// Initialise preferences before other features.
		await Preferences.Preferences.Instance.InitialiseAsync(discord, taskQueue);
		// Initialise remaining features.
		await Task.WhenAll(Instances.Select(instance => instance.InitialiseAsync(discord, taskQueue)));
		discord.ComponentInteractionCreated += OnComponentInteractionCreated;
	}

	private static async Task OnComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e)
	{
		try
		{
			switch (e.Id)
			{
				case Constants.InteractionCancelId:
					await e.Message.DeleteAsync();
					e.Handled = true;
					return;
				case { } s when InterimModule.ValidateEraseId(s):
					SaveLoad.EraseAllData(e.Guild.Id);
					await e.UpdateMessageEphemeral(":wastebasket: Erase", "Successful.", DiscordColor.Green);
					e.Handled = true;
					return;
			}

			foreach (IFeature instance in Instances)
			{
				if (instance is not IComponentInteractionHandler handler) continue;
				if (await handler.OnComponentInteractionCreated(sender, e)) return;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}
}