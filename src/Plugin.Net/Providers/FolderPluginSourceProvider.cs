using PluginDotNet.Abstractions;
using PluginDotNet.Contexts;
using PluginDotNet.Locators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace PluginDotNet.Providers
{
    public class FolderPluginSourceProvider : IPluginSource
    {
        readonly string _folderPath;
        readonly FolderPluginSourceOptions _options;
        readonly List<AssemblyPluginSourceProvider> _sources = new List<AssemblyPluginSourceProvider>();

        public bool IsInitialized { get; private set; }

        public FolderPluginSourceProvider(string folderPath, Action<TypeDataInfoBuilder> configureFinder, TypeDataInfo info,
            FolderPluginSourceOptions options)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            _folderPath = folderPath;
            _options = options ?? new FolderPluginSourceOptions();

            if (_options.TypeLocatorOptions == null)
            {
                _options.TypeLocatorOptions = new TypeLocatorOptions();
            }

            if (_options.TypeLocatorOptions.TypeInfos == null)
            {
                _options.TypeLocatorOptions.TypeInfos = new List<TypeDataInfo>();
            }

            if (configureFinder != null)
            {
                var builder = new TypeDataInfoBuilder();
                configureFinder(builder);

                var criteria = builder.Build();

                _options.TypeLocatorOptions.TypeInfos.Add(criteria);
            }

            if (info != null)
            {
                _options.TypeLocatorOptions.TypeInfos.Add(info);
            }

        }


        public Plugin GetPlugin(string name, Version version)
        {
            foreach (var asmSource in _sources)
            {
                var plugin = asmSource.GetPlugin(name, version);

                if (plugin == null)
                {
                    continue;
                }

                return plugin;
            }

            return null;
        }

        public List<Plugin> GetPlugins()
        {
            return _sources.SelectMany(x => x.GetPlugins()).ToList();
        }

        public void Initialize()
        {
            var foundFiles = new List<string>();

            foreach (var searchPattern in _options.SearchPatterns)
            {
                var dllFiles = Directory.GetFiles(_folderPath, searchPattern,
                    _options.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                foundFiles.AddRange(dllFiles);
            }

            foundFiles = foundFiles.Distinct().ToList();

            foreach (var assemblyPath in foundFiles)
            {
                // Assemblies are treated as readonly as long as possible
                var isPluginAssembly = IsPluginAssembly(assemblyPath);

                if (isPluginAssembly == false)
                {
                    continue;
                }

                var assemblyCatalogOptions = new AssemblyPluginSourceOptions
                {
                    PluginLoadContextOptions = _options.PluginLoadContextOptions,
                    TypeLocatorOptions = _options.TypeLocatorOptions,
                    PluginNameOptions = _options.PluginMetaDataLoader
                };

                // We are actually just delegating the responsibility from FolderPluginCatalog to AssemblyPluginCatalog. 
                var assemblyCatalog = new AssemblyPluginSourceProvider(assemblyPath,options: assemblyCatalogOptions);
                assemblyCatalog.Initialize();

                _sources.Add(assemblyCatalog);
            }

            IsInitialized = true;
        }

        public async Task InitializeAsync()
        {
            var foundFiles = new List<string>();

            foreach (var searchPattern in _options.SearchPatterns)
            {
                var dllFiles = Directory.GetFiles(_folderPath, searchPattern,
                    _options.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                foundFiles.AddRange(dllFiles);
            }

            foundFiles = foundFiles.Distinct().ToList();

            foreach (var assemblyPath in foundFiles)
            {
                // Assemblies are treated as readonly as long as possible
                var isPluginAssembly = IsPluginAssembly(assemblyPath);

                if (isPluginAssembly == false)
                {
                    continue;
                }

                var assemblyCatalogOptions = new AssemblyPluginSourceOptions
                {
                    PluginLoadContextOptions = _options.PluginLoadContextOptions,
                    TypeLocatorOptions = _options.TypeLocatorOptions,
                    PluginNameOptions = _options.PluginMetaDataLoader
                };

                // We are actually just delegating the responsibility from FolderPluginCatalog to AssemblyPluginCatalog. 
                var assemblyCatalog = new AssemblyPluginSourceProvider(assemblyPath, options: assemblyCatalogOptions);
               await assemblyCatalog.InitializeAsync();

                _sources.Add(assemblyCatalog);
            }

            IsInitialized = true;
        }

        private bool IsPluginAssembly(string assemblyPath)
        {
            using (Stream stream = File.OpenRead(assemblyPath))
            using (var reader = new PEReader(stream))
            {
                if (!reader.HasMetadata)
                {
                    return false;
                }

                if (_options.TypeLocatorOptions.TypeInfos?.Any() != true)
                {
                    // If there are no resolvers, assume that each DLL is a plugin
                    return true;
                }

                var runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
                var runtimeAssemblies = Directory.GetFiles(runtimeDirectory, "*.dll");
                var paths = new List<string>(runtimeAssemblies) { assemblyPath };

                if (_options.PluginLoadContextOptions.AdditionalRuntimePaths?.Any() == true)
                {
                    foreach (var additionalRuntimePath in _options.PluginLoadContextOptions.AdditionalRuntimePaths)
                    {
                        var dlls = Directory.GetFiles(additionalRuntimePath, "*.dll");
                        paths.AddRange(dlls);
                    }
                }

                if (_options.PluginLoadContextOptions.UseHostApplicationAssemblies == UseHostApplicationAssembliesEnum.Always)
                {
                    var hostApplicationPath = Environment.CurrentDirectory;
                    var hostDlls = Directory.GetFiles(hostApplicationPath, "*.dll", SearchOption.AllDirectories);

                    paths.AddRange(hostDlls);

                    AddSharedFrameworkDlls(hostApplicationPath, runtimeDirectory, paths);
                }
                else if (_options.PluginLoadContextOptions.UseHostApplicationAssemblies == UseHostApplicationAssembliesEnum.Never)
                {
                    var pluginPath = Path.GetDirectoryName(assemblyPath);
                    var dllsInPluginPath = Directory.GetFiles(pluginPath, "*.dll", SearchOption.AllDirectories);

                    paths.AddRange(dllsInPluginPath);
                }
                else if (_options.PluginLoadContextOptions.UseHostApplicationAssemblies == UseHostApplicationAssembliesEnum.Selected)
                {
                    foreach (var hostApplicationAssembly in _options.PluginLoadContextOptions.HostApplicationAssemblies)
                    {
                        var assembly = Assembly.Load(hostApplicationAssembly);
                        paths.Add(assembly.Location);
                    }
                }

                paths = paths.Distinct().ToList();

                var resolver = new PathAssemblyResolver(paths);

                // We use the metadata (readonly) versions of the assemblies before loading them
                using (var metadataContext = new MetadataLoadContext(resolver))
                {
                    var metadataPluginLoadContext = new MetadataTypeLocatorContext(metadataContext);
                    var readonlyAssembly = metadataContext.LoadFromAssemblyPath(assemblyPath);

                    var typeFinder = new TypeLocator();

                    foreach (var info in _options.TypeLocatorOptions.TypeInfos)
                    {
                        var typesFound = typeFinder.Find(info, readonlyAssembly, metadataPluginLoadContext);

                        if (typesFound?.Any() == true)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void AddSharedFrameworkDlls(string hostApplicationPath, string runtimeDirectory, List<string> paths)
        {
            // Fixing #23. If the main application references a shared framework (for example WinForms), we want to add these dlls also
            var defaultAssemblies = AssemblyLoadContext.Default.Assemblies.ToList();

            var defaultAssemblyDirectories = defaultAssemblies.Where(x => x.IsDynamic == false).Where(x => string.IsNullOrWhiteSpace(x.Location) == false)
                .GroupBy(x => Path.GetDirectoryName(x.Location)).Select(x => x.Key).ToList();

            foreach (var assemblyDirectory in defaultAssemblyDirectories)
            {
                if (string.Equals(assemblyDirectory.TrimEnd('\\').TrimEnd('/'), hostApplicationPath.TrimEnd('\\').TrimEnd('/')))
                {
                    continue;
                }

                if (string.Equals(assemblyDirectory.TrimEnd('\\').TrimEnd('/'), runtimeDirectory.TrimEnd('\\').TrimEnd('/')))
                {
                    continue;
                }

                if (_options.PluginLoadContextOptions.AdditionalRuntimePaths == null)
                {
                    _options.PluginLoadContextOptions.AdditionalRuntimePaths = new List<string>();
                }

                if (_options.PluginLoadContextOptions.AdditionalRuntimePaths.Contains(assemblyDirectory) == false)
                {
                    _options.PluginLoadContextOptions.AdditionalRuntimePaths.Add(assemblyDirectory);
                }

                var dlls = Directory.GetFiles(assemblyDirectory, "*.dll");
                paths.AddRange(dlls);
            }
        }
    }
}
