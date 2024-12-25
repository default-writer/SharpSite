namespace SharpSite.Plugins.Constants;

public static class Strings
{
	public const string PluginsDirectory = "plugins";
	public const string PluginsUploadedDirectory = "_uploaded";
	public const string PluginsRootDirectory = "_wwwroot";
	public const string PluginsManifestFile = "mainfest.json";
	public const string PluginsApplicationStateFile = "applicationState.json";
}

public static class Paths
{
	public static readonly string PluginsDirectory = Strings.PluginsDirectory;
	public static readonly string PluginsUploadDirectory = Path.Combine(Strings.PluginsDirectory, Strings.PluginsUploadedDirectory);
	public static readonly string PluginsRootDirectory = Path.Combine(Strings.PluginsDirectory, Strings.PluginsRootDirectory);
	public static readonly string PluginsApplicationStateFile = Path.Combine(Strings.PluginsDirectory, Strings.PluginsApplicationStateFile);
	public static readonly string PluginsManifestFile = Strings.PluginsManifestFile;
}
