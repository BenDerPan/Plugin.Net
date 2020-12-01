using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PluginDotNet.Locators
{
    public interface ITypeLocatorContext
    {
        Assembly FindAssembly(string assemblyName);
        Type FindType(Type type);
    }
}
