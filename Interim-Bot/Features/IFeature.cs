namespace Interim.Features;

public interface IFeature
{
	Task InitialiseAsync(DiscordClient discord, IBackgroundTaskQueue taskQueue);
}