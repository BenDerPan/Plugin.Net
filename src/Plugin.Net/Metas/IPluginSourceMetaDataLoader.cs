using PluginDotNet.Locators;
using System;
using System.Collections.Generic;
using System.Text;

namespace PluginDotNet.Metas
{
    public interface IPluginSourceMetaDataLoader
    {
        IPluginMetaDataLoader PluginMetaDataLoader { get; }

        TypeLocatorOptions TypeLocatorOptions { get; }

        ITypeLocatorContext TypeLocatorContext { get; }
    }
}
