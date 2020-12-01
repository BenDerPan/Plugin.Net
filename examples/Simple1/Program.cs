using PluginDotNet.Providers;
using PluginDotNet.Metas;
using System;

namespace Simple1
{
    class Program
    {
        static void Main(string[] args)
        {
            var typePluginSourceProvider = new TypePluginSourceProvider(typeof(ScanerPlugin), new DefaultPluginSourceMetaDataLoader());
            typePluginSourceProvider.Initialize();
            var plugins = typePluginSourceProvider.GetPlugins();
            foreach (var plugin in plugins)
            {
                var instance = plugin.NewInstance<IScanerPlugin>();
                var result=instance.DoScan("google.com");
                Console.WriteLine(result);
            }

            var asmSource = new AssemblyPluginSourceProvider(assembly:typeof(Program).Assembly, filter: type => typeof(IScanerPlugin).IsAssignableFrom(type)&&!type.IsInterface);
            asmSource.Initialize();
            var asmPlugins = asmSource.GetPlugins();

            foreach (var plugin in asmPlugins)
            {
                var instance = plugin.NewInstance<IScanerPlugin>();
                var result = instance.DoScan("facebook.com");
                Console.WriteLine(result);
            }

           

            Console.ReadKey();
        }
    }
}
