using DSharpPlus.EventArgs;

namespace Interim.Extensions;

public static class IdExtensions
{
	public static string FromInteractionId(this ComponentInteractionCreateEventArgs e) => e.Id.FromInteractionId();
	
	public static string FromInteractionId(this string id)
	{
		int last = id.LastIndexOf('_');
		return id[..last];
	}
	
	public static string FromInteractionId(this string id, out string guid)
	{
		int last = id.LastIndexOf('_');
		guid = id[(last + 1)..];
		return id[..last];
	}

	public static string ToInteractionId(this string id) => $"{id}_{Guid.NewGuid()}";
	
	public static string ToInteractionId(this string id, out string guid)
	{
		guid = Guid.NewGuid().ToString();
		return $"{id}_{guid}";
	}
}