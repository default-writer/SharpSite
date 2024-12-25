using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSite.Abstractions.Plugins
{

	public interface IApplicationState
	{
		CurrentThemeRecord? CurrentTheme { get; set; }
		void AddPlugin(string id, IPluginManifest manifest);
		void SetTheme(IPluginManifest manifest);
		IPluginManifest[] Themes { get; }
		Task Save();
	}
}
