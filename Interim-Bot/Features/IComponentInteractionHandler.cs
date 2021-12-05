using DSharpPlus.EventArgs;

namespace Interim.Features;

public interface IComponentInteractionHandler
{
	Task<bool> OnComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e);
}