using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PluginDotNet.Locators
{
    public class TypeLocatorOptions
    {
        public List<TypeDataInfo> TypeInfos { get; set; } = new List<TypeDataInfo>(Defaults.GetDefaultTypeInfos());

        public static class Defaults
        {
            public static List<TypeDataInfo> TypeInfos { get; set; } = new List<TypeDataInfo>();

            public static ReadOnlyCollection<TypeDataInfo> GetDefaultTypeInfos()
            {
                return TypeInfos.AsReadOnly();
            }
        }
    }
}
