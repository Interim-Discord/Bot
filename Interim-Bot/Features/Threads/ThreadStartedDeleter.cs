using System.Reflection;
using DSharpPlus.EventArgs;

namespace Interim.Features.Threads;

public class ThreadStartedDeleter : FeatureSingleton<ThreadStartedDeleter>
{
	private DiscordClient? client;
	private readonly HashSet<ulong> enabledServers = new();

	public override Task InitialiseAsync(DiscordClient discord)
	{
		client = discord;
		enabledServers.UnionWith(Preferences.Preferences.Instance.ServersWithThreadMessageDeletion);
		client.MessageCreated += OnMessageCreated;
		return Task.CompletedTask;
	}

	private Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
	{
		if (!enabledServers.Contains(e.Guild.Id)) return Task.CompletedTask;
		if (e.Message.MessageType != (MessageType)18) return Task.CompletedTask;
		return DeleteThreadIfRequired(sender, e);
	}

	private async Task DeleteThreadIfRequired(DiscordClient sender, MessageCreateEventArgs e)
	{
		// Get the referenced channel id from this thread created message.
		object value = typeof(DiscordMessage).GetProperty("InternalReference", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(e.Message)!;
		ulong channelId = (ulong)value.GetType().GetProperty("ChannelId", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(value)!;
		DiscordChannel channel = await sender.GetChannelAsync(channelId);
		if (!channel.IsThread) return;
		DiscordMessage? message = (await e.Channel.GetMessagesBeforeAsync(e.Message.Id, 1)).FirstOrDefault();
		if (message == null) return;
		if (channel.Id != message.Id) return;
		await e.Message.DeleteAsync("Automatically removed redundant thread started message");
	}

	public void ModifiedGuild(ulong guildId, bool enabled)
	{
		if (enabled)
			enabledServers.Add(guildId);
		else
			enabledServers.Remove(guildId);
	}
}