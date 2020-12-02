using PluginDotNet.Locators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace PluginDotNet.Contexts
{
    public class PluginAssemblyLoadContext : AssemblyLoadContext, ITypeLocatorContext
    {
        private readonly string _pluginPath;
        private readonly AssemblyDependencyResolver _resolver;
        
        private readonly PluginLoadContextOptions _options;

        public PluginAssemblyLoadContext(Assembly assembly, PluginLoadContextOptions options = null) : this(assembly.Location, options)
        {
        }

        public PluginAssemblyLoadContext(string pluginPath, PluginLoadContextOptions options = null) : base(true)
        {
            _pluginPath = pluginPath;
            _resolver = new AssemblyDependencyResolver(pluginPath);
            _options = options ?? new PluginLoadContextOptions();
        }

        public Assembly Load()
        {
            var assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(_pluginPath));
            Assembly result = null;
            try
            {
                Console.WriteLine($"Load Assembly:  Path={_pluginPath}, AssemblyName={assemblyName}");
                result = LoadFromAssemblyName(assemblyName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Load Error: Path={_pluginPath}, AssemblyName={assemblyName}, Error:{e.Message}");
            }

            return result;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (TryUseHostApplicationAssembly(assemblyName))
            {
                try
                {
                    var defaultAssembly = Default.LoadFromAssemblyName(assemblyName);


                    return null;
                }
                catch
                {
                }
            }

            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            if (assemblyPath != null)
            {
                
                var result = LoadFromAssemblyPath(assemblyPath);
                return result;
            }

            if (_options.AdditionalRuntimePaths?.Any() != true)
            {
                
                return null;
            }

            // Solving issue 23. The project doesn't reference WinForms but the plugin does.
            // Try to locate the required dll using AdditionalRuntimePaths
            foreach (var runtimePath in _options.AdditionalRuntimePaths)
            {
                var fileName = assemblyName.Name + ".dll";
                var filePath = Directory.GetFiles(runtimePath, fileName, SearchOption.AllDirectories).FirstOrDefault();

                if (filePath != null)
                {
                  
                    return LoadFromAssemblyPath(filePath);
                }
            }

        
            return null;
        }

        private bool TryUseHostApplicationAssembly(AssemblyName assemblyName)
        {
       
            if (_options.UseHostApplicationAssemblies == UseHostApplicationAssembliesEnum.Never)
            {
             
                return false;
            }

            if (_options.UseHostApplicationAssemblies == UseHostApplicationAssembliesEnum.Always)
            {
               

                return true;
            }

            var name = assemblyName.Name;

            var result = _options.HostApplicationAssemblies?.Any(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase)) == true;

            return result;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }

        public Assembly FindAssembly(string assemblyName)
        {
            return Load(new AssemblyName(assemblyName));
        }

        public Type FindType(Type type)
        {
            var assemblyName = type.Assembly.GetName();
            var assembly = Load(assemblyName);

            if (assembly == null)
            {
                assembly = Assembly.Load(assemblyName);
            }

            var result = assembly.GetType(type.FullName);

            return result;
        }
    }
}
