using PluginDotNet.Abstractions;
using PluginDotNet.Locators;
using PluginDotNet.Metas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PluginDotNet.Providers
{
    public class TypePluginSourceProvider : IPluginSource
    {
        readonly Type _pluginType;
        readonly IPluginSourceMetaDataLoader _metaLoader;
        Plugin _plugin;

        public TypePluginSourceProvider(Type pluginType, IPluginSourceMetaDataLoader pluginSourceMetaDataLoader)
        {
            if (pluginType==null)
            {
                throw new ArgumentNullException(nameof(pluginType));
            }

            _pluginType = pluginType;

            _metaLoader = pluginSourceMetaDataLoader;
            if (_metaLoader==null)
            {
                _metaLoader = new DefaultPluginSourceMetaDataLoader();
            }

        }

        public void Initialize()
        {
            var marks = new List<string>();
            var locator = new TypeLocator();
            _plugin = new Plugin(_pluginType.Assembly, _pluginType, _metaLoader.PluginMetaDataLoader.GetPluginName(_pluginType),
                _metaLoader.PluginMetaDataLoader.GetPluginVersion(_pluginType),
                this, _metaLoader.PluginMetaDataLoader.GetPluginDescription(_pluginType),
                _metaLoader.PluginMetaDataLoader.GetPluginProductVersion(_pluginType), marks: marks);

            IsInitialized = true;
        }

        public Task InitializeAsync()
        {
            Initialize();
            return Task.CompletedTask;
        }

        public bool IsInitialized { get; private set; }

        public List<Plugin> GetPlugins()
        {
            return new List<Plugin> { _plugin };
        }

        public Plugin GetPlugin(string name, Version version)
        {
            if (!string.Equals(name, _plugin.Name, StringComparison.InvariantCultureIgnoreCase) ||
                version != _plugin.Version)
            {
                return null;
            }

            return _plugin;
        }
    }
}
