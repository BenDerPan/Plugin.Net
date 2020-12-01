using System;
using System.Collections.Generic;
using System.Text;

namespace PluginDotNet.Metas
{
    public interface IPluginMetaDataLoader
    {

        string GetPluginName(Type pluginType);
        Version GetPluginVersion(Type pluginType);

        string GetPluginDescription(Type pluginType);

        string GetPluginProductVersion(Type pluginType);

    }
}
