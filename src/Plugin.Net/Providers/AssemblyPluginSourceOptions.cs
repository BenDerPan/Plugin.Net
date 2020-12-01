using PluginDotNet.Contexts;
using PluginDotNet.Locators;
using PluginDotNet.Metas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDotNet.Providers
{
    public class AssemblyPluginSourceOptions
    {
        /// <summary>
        /// Gets or sets the <see cref="PluginLoadContextOptions"/>.
        /// </summary>
        public PluginLoadContextOptions PluginLoadContextOptions = new PluginLoadContextOptions();

        /// <summary>
        /// Gets or sets how the plugin names and version should be defined. <seealso cref="PluginNameOptions"/>.
        /// </summary>
        public IPluginMetaDataLoader PluginNameOptions { get; set; } = new DefaultPluginMetaDataLoader();

        /// <summary>
        /// Gets or sets the <see cref="TypeLocatorOptions"/>. 
        /// </summary>
        public TypeLocatorOptions TypeLocatorOptions { get; set; } = new TypeLocatorOptions();
    }
}
