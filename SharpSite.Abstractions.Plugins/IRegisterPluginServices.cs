using Microsoft.Extensions.Hosting;

namespace SharpSite.Abstractions.Plugins;

public interface IRegisterPluginServices
{
	IHostApplicationBuilder RegisterServices(IHostApplicationBuilder services);

}
