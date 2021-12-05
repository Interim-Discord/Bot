using System.Text.Json;
using System.Text.Json.Serialization;

namespace Interim;

public static class Constants
{
	public const string InteractionCancelId = "interim_Cancel";
	
	public const string ApplicationParentDirectoryName = "Vertx";
	public const string ApplicationDirectoryName = "Interim Bot";
	public static string ApplicationDirectoryPath =>
		Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			ApplicationParentDirectoryName,
			ApplicationDirectoryName
		);
	
	public static string GetDirectoryPath(DiscordGuild guild) => GetDirectoryPath(guild.Id);
	public static string GetDirectoryPath(ulong guildId) => Path.Combine(ApplicationDirectoryPath, guildId.ToString());
	public static string GetDirectoryPath(ulong guildId, string fileNameWithExtension) => Path.Combine(ApplicationDirectoryPath, guildId.ToString(), fileNameWithExtension);
}

public static class SaveLoad
{
	public static async Task SaveAsync<T>(T data, ulong guildId, string fileNameWithExtension, JsonSerializerContext? context)
	{
		try
		{
			string filePath = GetFilePath(guildId, fileNameWithExtension);
			await using FileStream file = File.Create(filePath);
			if(context != null)
				await JsonSerializer.SerializeAsync(file, data, typeof(T), context);
			else
				await JsonSerializer.SerializeAsync(file, data, typeof(T));
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}

	public static string GetFilePath(ulong guildId, string fileNameWithExtension)
	{
		string directoryPath = Constants.GetDirectoryPath(guildId);
		Directory.CreateDirectory(directoryPath);
		return Path.Combine(directoryPath, fileNameWithExtension);
	}

	public static async ValueTask<T> LoadAsync<T>(ulong guildId, string fileName, JsonSerializerContext context) where T : new()
	{
		string filePath = Constants.GetDirectoryPath(guildId, fileName);
		if (!File.Exists(filePath))
			return new T();

		await using FileStream file = File.OpenRead(filePath);
		return (T)(await JsonSerializer.DeserializeAsync(file, typeof(T), context))!;
	}
	
	public static Task<string> LoadSerializedDataAsync(ulong guildId, string fileName)
	{
		string filePath = Constants.GetDirectoryPath(guildId, fileName);
		return !File.Exists(filePath) ? Task.FromResult(string.Empty) : File.ReadAllTextAsync(filePath);
	}

	public static void EraseAllData(ulong guildId)
	{
		string directory = Constants.GetDirectoryPath(guildId);
		if (!Directory.Exists(directory)) return;
		Directory.Delete(directory, true);
	}
}