using Microsoft.AspNetCore.Builder;

namespace SharpSite.Web.Logging;

public static class SharpSiteLoggingMiddlewareExtensions
{
	public static IApplicationBuilder UseSharpSiteLogging(this IApplicationBuilder builder) => builder.UseMiddleware<SharpSiteLoggingMiddleware>();
}
