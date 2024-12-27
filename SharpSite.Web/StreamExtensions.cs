using SharpSite.Plugins;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpSite.Web;

public static class StreamExtensions
{
	public static PluginManifest ReadManifest(this Stream manifestStream)
	{
		var options = new JsonSerializerOptions
		{
			Converters = { new JsonStringEnumConverter() }
		};
		return JsonSerializer.Deserialize<PluginManifest>(manifestStream, options)!;
	}
}
