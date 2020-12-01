using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PluginDotNet.Abstractions
{
    public interface IPluginSource
    {
        void Initialize();
        Task InitializeAsync();

        bool IsInitialized { get; }

        List<Plugin> GetPlugins();

        Plugin GetPlugin(string name, Version version);
    }
}
