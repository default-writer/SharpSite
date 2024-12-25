using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;
using SharpSite.Abstractions.Plugins;
using SharpSite.Plugins.Constants;
using SharpSite.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSite.Plugins;

public class RegisterPluginServices : IRegisterPluginServices
{
	public IHostApplicationBuilder RegisterServices(IHostApplicationBuilder builder)
	{
		Directory.CreateDirectory(Paths.PluginsDirectory);
		Directory.CreateDirectory(Paths.PluginsUploadDirectory);
		Directory.CreateDirectory(Paths.PluginsRootDirectory);

		// Configure applicatin state and the PluginManager
		builder.Services.AddSingleton<IApplicationState, ApplicationState>();
		builder.Services.AddSingleton<PluginManager>();
		//builder.Services.AddSingleton(appState);
		builder.Services.AddSingleton<PluginAssemblyManager>();
		//builder.Services.AddSingleton<PluginManager>();

		return builder;
	}
}
