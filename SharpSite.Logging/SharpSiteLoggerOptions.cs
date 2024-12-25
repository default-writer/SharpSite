using Microsoft.Extensions.Logging;

namespace SharpSite.Logging;

public class SharpSiteLoggerOptions
{
	/// <summary>Include the category name of the logger in customDimensions under the 'CategoryName' key</summary>
	public bool IncludeCategoryName { get; set; } = true;

	/// <summary>Include scope information in customDimensions</summary>
	public bool IncludeScopes { get; set; } = true;

	/// <summary>Min LogLevel to write to app insights defaults to Trace aka Verbose</summary>
	public LogLevel MinLogLevel { get; set; } = LogLevel.Trace;
}
