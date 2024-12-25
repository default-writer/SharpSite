using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace SharpSite.Web.Logging;

public static class Constants
{
	public const string SharpSiteScope = "SharpSite";
}

public class SharpSiteLoggingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<SharpSiteLoggingMiddleware> _logger;

	public SharpSiteLoggingMiddleware(RequestDelegate next, ILogger<SharpSiteLoggingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext httpContext)
	{
		using (_logger.BeginScope(Constants.SharpSiteScope))
		{
			await _next(httpContext);
		}
	}
}
