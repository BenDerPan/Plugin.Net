using PluginDotNet.Contexts;
using PluginDotNet.Metas;
using PluginDotNet.Locators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDotNet.Providers
{
    public class FolderPluginSourceOptions
    {
        /// <summary>
        /// Gets or sets if subfolders should be included. Defaults to true.
        /// </summary>
        public bool IncludeSubfolders { get; set; } = true;

        /// <summary>
        /// Gets or sets the search patterns when locating plugins. By default only located dll-files.
        /// </summary>
        public List<string> SearchPatterns { get; set; } = new List<string>() { "*.dll" };

        /// <summary>
        /// Gets or sets the <see cref="PluginLoadContextOptions"/>.
        /// </summary>
        public PluginLoadContextOptions PluginLoadContextOptions { get; set; } = new PluginLoadContextOptions();

        /// <summary>
        /// Gets or sets the <see cref="TypeLocatorOptions"/>.
        /// </summary>
        public TypeLocatorOptions TypeLocatorOptions { get; set; } = new TypeLocatorOptions();

        /// <summary>
        /// Gets or sets how the plugin names and version should be defined. <seealso cref="PluginNameOptions"/>
        /// </summary>
        public IPluginMetaDataLoader PluginMetaDataLoader { get; set; } = new DefaultPluginMetaDataLoader();

    }
}
