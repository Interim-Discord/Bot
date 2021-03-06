namespace Interim.Features;

public abstract class FeatureSingleton<T> : IFeature where T : FeatureSingleton<T>, new()
{
	public abstract Task InitialiseAsync(DiscordClient discord, IBackgroundTaskQueue taskQueue);
	
	public static T Instance { get; }
	
	static FeatureSingleton() => Instance = new T();
}