using PluginDotNet.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace PluginDotNet.Metas
{
    public class DefaultPluginMetaDataLoader : IPluginMetaDataLoader
    {
        public string GetPluginName(Type pluginType)
        {
            var displayNameAttribute = pluginType.GetCustomAttribute(typeof(DisplayNameAttribute), true) as DisplayNameAttribute;
            if (displayNameAttribute==null)
            {
                return pluginType.FullName;
            }

            if (string.IsNullOrWhiteSpace(displayNameAttribute.DisplayName))
            {
                return pluginType.FullName;
            }

            return displayNameAttribute.DisplayName;
        }

        public Version GetPluginVersion(Type pluginType)
        {
            var asmLocation = pluginType.Assembly.Location;
            Version version;
            if (!string.IsNullOrWhiteSpace(asmLocation))
            {
                var verInfo = FileVersionInfo.GetVersionInfo(asmLocation);
                if (string.IsNullOrWhiteSpace(verInfo.FileVersion))
                {
                    version = new Version(1, 0, 0, 0);
                }
                else if (string.Equals(verInfo.FileVersion,"0.0.0.0"))
                {
                    version = new Version(1, 0, 0, 0);
                }
                else
                {
                    version = Version.Parse(verInfo.FileVersion);
                }
            }
            else
            {
                version = new Version(1, 0, 0, 0);
            }

            return version;
        }

        public string GetPluginDescription(Type pluginType)
        {
            var asmLocation = pluginType.Assembly.Location;
            if (string.IsNullOrWhiteSpace(asmLocation))
            {
                return string.Empty;
            }

            var verInfo = FileVersionInfo.GetVersionInfo(asmLocation);
            return verInfo.Comments;
        }

        public string GetPluginProductVersion(Type pluginType)
        {
            var asmLocation = pluginType.Assembly.Location;
            if (string.IsNullOrWhiteSpace(asmLocation))
            {
                return string.Empty;
            }

            var verInfo = FileVersionInfo.GetVersionInfo(asmLocation);
            return verInfo.ProductVersion;
        }

        
    }
}
