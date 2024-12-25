using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpSite.Logging;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace SharpSite.Logging;

public abstract class Log
{
	[JsonPropertyName("properties")]
	public Dictionary<string, object?>? Properties { get; set; }
}

public class Error
{
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	[JsonPropertyName("message")]
	public string? Message { get; set; }

	[JsonPropertyName("stack")]
	public string? Stack { get; set; }
}

public class Trace : Log
{
	/// <summary>
	/// A message string
	/// </summary>
	[JsonPropertyName("message")]
	public string? Message { get; set; }

	/// <summary>
	/// Severity level of the logging message used for filtering in the portal
	/// </summary>
	[JsonPropertyName("severityLevel")]
	public SeverityLevel? SeverityLevel { get; set; }
}

public class SharpSiteLogger : ILogger
{
	private readonly string _categoryName;

	private IExternalScopeProvider? _scopeProvider { get; set; }

	public SharpSiteLogger(string categoryName, IExternalScopeProvider? scopeProvider)
	{
		_categoryName = categoryName;
		if (scopeProvider != null)
		{
			ScopeProvider = scopeProvider;
		}
	}

	/// <summary>Include the logger category name in customDimensions under the 'CategoryName' key</summary>
	public bool IncludeCategoryName { get; set; }

	/// <summary>Include scope information in CustomDimensions</summary>
	public bool IncludeScopes { get; set; }

	/// <summary>Min LogLevel to write to app insights</summary>
	public LogLevel MinLogLevel { get; set; }

	/// <summary>Set the active scope provider</summary>
	internal IExternalScopeProvider ScopeProvider { private get; set; } = new LoggerExternalScopeProvider();

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => ScopeProvider.Push(state);

	private Dictionary<string, object?> GetCustomDimensions<TState>(TState state, EventId eventId)
	{
		var result = new Dictionary<string, object?>();

		ApplyScopes(result);
		ApplyLogState(result, state, eventId);
		return result;
	}
	private void ApplyLogState<TState>(Dictionary<string, object?> customDimensions, TState state, EventId eventId)
	{
		if (IncludeCategoryName && !string.IsNullOrEmpty(_categoryName))
			customDimensions["CategoryName"] = _categoryName;

		if (eventId.Id != 0)
			customDimensions["EventId"] = eventId.Id.ToString(CultureInfo.InvariantCulture);

		if (!string.IsNullOrEmpty(eventId.Name))
			customDimensions["EventName"] = eventId.Name;

		if (state is IReadOnlyCollection<KeyValuePair<string, object?>> stateDictionary)
			ApplyDictionary(customDimensions, stateDictionary);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ApplyScopes(Dictionary<string, object?> customDimensions)
	{
		if (!IncludeScopes)
			return;

		var scopeBuilder = new StringBuilder();
		ScopeProvider.ForEachScope(ApplyScope, (customDimensions, scopeBuilder));

		if (scopeBuilder.Length > 0)
			customDimensions["Scope"] = scopeBuilder.ToString();

		static void ApplyScope(object? scope, (Dictionary<string, object?> data, StringBuilder scopeBuilder) result)
		{
			if (scope is IReadOnlyCollection<KeyValuePair<string, object?>> scopeDictionary)
			{
				ApplyDictionary(result.data, scopeDictionary);
				return;
			}

			result.scopeBuilder.Append(" => ").Append(scope);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ApplyDictionary(Dictionary<string, object?> target, IReadOnlyCollection<KeyValuePair<string, object?>> source)
	{
		foreach (var kvp in source)
		{
			var key = kvp.Key == "{OriginalFormat}" ? "OriginalFormat" : kvp.Key;
			target[key] = Convert.ToString(kvp.Value, CultureInfo.InvariantCulture);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static SeverityLevel GetSeverityLevel(LogLevel logLevel) => logLevel switch
	{
		LogLevel.Trace => SeverityLevel.Verbose,
		LogLevel.Debug => SeverityLevel.Verbose,
		LogLevel.Information => SeverityLevel.Information,
		LogLevel.Warning => SeverityLevel.Warning,
		LogLevel.Error => SeverityLevel.Error,
		LogLevel.Critical => SeverityLevel.Critical,
		_ => SeverityLevel.Verbose
	};

	/// <inheritdoc />
	public bool IsEnabled(LogLevel logLevel) => this.MinLogLevel <= logLevel && logLevel != LogLevel.None;

	/// <inheritdoc />
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
			return;

		var severity = GetSeverityLevel(logLevel);
		var message = formatter(state, exception);
		var customDimensions = GetCustomDimensions(state, eventId);

		if (exception is null)
		{
			//_applicationInsights.TrackTrace(new Trace() { Message = message, SeverityLevel = severity, Properties = customDimensions });
			return;
		}

		var error = new Error
		{
			Name = exception.GetType().Name,
			Message = exception.Message,
			Stack = exception.ToString()
		};

		//_applicationInsights.TrackException(new() { Exception = error, Id = $"{eventId}", SeverityLevel = severity, Properties = customDimensions });
	}
}
