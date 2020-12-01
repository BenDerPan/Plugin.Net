using PluginDotNet.Locators;
using System;
using System.Collections.Generic;
using System.Text;

namespace PluginDotNet.Metas
{
    public class DefaultPluginSourceMetaDataLoader : IPluginSourceMetaDataLoader
    {
        public IPluginMetaDataLoader PluginMetaDataLoader { get; set; } = new DefaultPluginMetaDataLoader();

        public TypeLocatorOptions TypeLocatorOptions { get; set; } = new TypeLocatorOptions();

        public ITypeLocatorContext TypeLocatorContext { get; set; } = null;
    }
}
