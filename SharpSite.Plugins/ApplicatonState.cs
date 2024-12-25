using SharpSite.Abstractions.Plugins;
using SharpSite.Abstractions.Theme;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpSite.Plugins;

public class ApplicationStateOptions
{
	public CurrentThemeRecord? CurrentTheme { get; set; }
	/// <summary>
	/// Maximum file upload size in megabytes.
	/// </summary>
	public long MaximumUploadSizeMB { get; set; } = 10; // 10MB
}

public class ApplicationState: ApplicationStateOptions, IApplicationState
{
	public IPluginManifest[] Themes => Plugins.Values
			.Where(p => p.Features.Contains(Enum.GetName(PluginFeatures.Theme)?.ToLowerInvariant()))
			.ToArray();

	/// <summary>
	/// Maximum file upload size in megabytes.
	/// </summary>
	public long MaximumUploadSizeMB { get; set; } = 10; // 10MB

	[JsonIgnore]
	public Type? ThemeType
	{
		get
		{
			if (CurrentTheme is null) return null;
			var themeManifest = Plugins.Values.FirstOrDefault(p => p.IdVersion == CurrentTheme.IdVersion);
			if (themeManifest is not null)
			{
				var pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == themeManifest.Id);
				var themeType = pluginAssembly?.GetTypes().FirstOrDefault(t => typeof(IHasStylesheets).IsAssignableFrom(t));
				return themeType!;
			}

			return null;

		}
	}

	/// <summary>
	/// List of the plugins that are currently loaded.
	/// </summary>
	[JsonIgnore]
	public Dictionary<string, IPluginManifest> Plugins { get; } = new();

	public void AddPlugin(string pluginName, IPluginManifest manifest)
	{
		if (!Plugins.ContainsKey(pluginName))
		{
			Plugins.Add(pluginName, manifest);
		}
		else
		{
			Plugins[pluginName] = manifest;
		}
	}

	public void SetTheme(IPluginManifest manifest)
	{
		// identify the pluginAssembly in memory that's named after the manifest.Id
		var pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == manifest.Id);

		var themeType = pluginAssembly?.GetTypes().FirstOrDefault(t => typeof(IHasStylesheets).IsAssignableFrom(t));
		if (themeType is not null) CurrentTheme = new(manifest.IdVersion);
	}

	public ApplicationState()
	{
		// load application state from applicationState.json in the root of the plugins folder
		var appStateFile = Path.Combine("plugins", "applicationState.json");

		if (File.Exists(appStateFile))
		{
			var json = File.ReadAllText(appStateFile);
			var state = JsonSerializer.Deserialize<ApplicationState>(json);
			if (state is not null)
			{
				CurrentTheme = state.CurrentTheme;
				MaximumUploadSizeMB = state.MaximumUploadSizeMB;
			}
		}
	}

	public async Task Save()
	{
		// save application state to applicationState.json in the root of the plugins folder
		var appStateFile = Path.Combine("plugins", "applicationState.json");

		var json = JsonSerializer.Serialize(this);
		await File.WriteAllTextAsync(appStateFile, json);
	}
}
