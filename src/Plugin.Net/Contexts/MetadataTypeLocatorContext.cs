using PluginDotNet.Locators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginDotNet.Contexts
{
    public class MetadataTypeLocatorContext : ITypeLocatorContext
    {
        readonly MetadataLoadContext _metadataLoadContext;

        public MetadataTypeLocatorContext(MetadataLoadContext metadataLoadContext)
        {
            _metadataLoadContext = metadataLoadContext;
        }

        public Assembly FindAssembly(string assemblyName)
        {
            var result = _metadataLoadContext.LoadFromAssemblyName(assemblyName);

            return result;
        }

        public Type FindType(Type type)
        {
            var assemblyName = type.Assembly.GetName();
            var assemblies = _metadataLoadContext.GetAssemblies();
            var assembly = assemblies.FirstOrDefault(x => string.Equals(x.FullName, assemblyName.FullName));


            try
            {
                if (assembly == null)
                {
                    assembly = _metadataLoadContext.LoadFromAssemblyName(assemblyName);
                }

                return assembly.GetType(type.FullName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
