using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Http;

namespace SharpSite.Logging;
	
public class SharpSiteLoggerProvider(IExternalScopeProvider externalScopeProvider) : ILoggerProvider, ISupportExternalScope
{
	private readonly ConcurrentDictionary<string, SharpSiteLogger> _loggers = new ConcurrentDictionary<string, SharpSiteLogger>();

	private IExternalScopeProvider? _externalScopeProvider = externalScopeProvider;
	private bool disposedValue;

	void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider externalScopeProvider) => _externalScopeProvider = externalScopeProvider;

	public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, new SharpSiteLogger(categoryName, _externalScopeProvider));

	/// <inheritdoc />
	public void SetScopeProvider(IExternalScopeProvider scopeProvider)
	{
		_externalScopeProvider = scopeProvider;

		foreach (var logger in _loggers.Values)
			logger.ScopeProvider = scopeProvider;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			disposedValue = true;
		}
	}

	// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	// ~SharpSiteLoggingProvider()
	// {
	//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
