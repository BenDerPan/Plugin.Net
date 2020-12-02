using PluginDotNet.Providers;
using Simple1;
using System;

namespace FolderExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var folderSource = new FolderPluginSourceProvider(folderPath: ".", configureLocator: cfg => {
                cfg.AssignableTo(typeof(IScanerPlugin));
                cfg.IsInterface(false);
            }, null, null);
            folderSource.Initialize();
            var folderPlugins = folderSource.GetPlugins();

            foreach (var plugin in folderPlugins)
            {
                var instance = plugin.NewInstance<IScanerPlugin>();
                var result = instance.DoScan("twitter.com");
                Console.WriteLine(result);
            }


            Console.ReadKey();
        }
    }
}
