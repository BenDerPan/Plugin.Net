using PluginDotNet.Abstractions;
using PluginDotNet.Contexts;
using PluginDotNet.Locators;
using PluginDotNet.Metas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginDotNet.Providers
{
    public class AssemblyPluginSourceProvider : IPluginSource
    {
        readonly string _assemblyPath;
        Assembly _assembly;

        private readonly AssemblyPluginSourceOptions _options;
        private PluginAssemblyLoadContext _pluginAssemblyLoadContext;
        private List<TypePluginSourceProvider> _plugins = null;

        public bool IsInitialized { get; private set; }

        public AssemblyPluginSourceProvider(string assemblyPath = null, Assembly assembly = null, Predicate<Type> filter = null,
            Dictionary<string, Predicate<Type>> taggedFilters = null, Action<TypeDataInfoBuilder> configureFinder = null,
            TypeDataInfo info = null, AssemblyPluginSourceOptions options = null)
        {
            if (assembly != null)
            {
                _assembly = assembly;
                _assemblyPath = _assembly.Location;
            }
            else if (!string.IsNullOrWhiteSpace(assemblyPath))
            {
                _assemblyPath = assemblyPath;
            }
            else
            {
                throw new ArgumentNullException($"{nameof(assembly)} or {nameof(assemblyPath)} must be set.");
            }

            _options = options ?? new AssemblyPluginSourceOptions();

            SetFilters(filter, taggedFilters, info, configureFinder);
        }
        private void SetFilters(Predicate<Type> filter, Dictionary<string, Predicate<Type>> taggedFilters, TypeDataInfo info,
            Action<TypeDataInfoBuilder> configureBuilder)
        {
            if (_options.TypeLocatorOptions == null)
            {
                _options.TypeLocatorOptions = new TypeLocatorOptions();
            }

            if (_options.TypeLocatorOptions.TypeInfos == null)
            {
                _options.TypeLocatorOptions.TypeInfos = new List<TypeDataInfo>();
            }

            if (filter != null)
            {
                var filterCriteria = new TypeDataInfo { Query = (context, type) => filter(type) };
                filterCriteria.Tags.Add(string.Empty);

                _options.TypeLocatorOptions.TypeInfos.Add(filterCriteria);
            }

            if (taggedFilters?.Any() == true)
            {
                foreach (var taggedFilter in taggedFilters)
                {
                    var taggedCriteria = new TypeDataInfo { Query = (context, type) => taggedFilter.Value(type) };
                    taggedCriteria.Tags.Add(taggedFilter.Key);

                    _options.TypeLocatorOptions.TypeInfos.Add(taggedCriteria);
                }
            }

            if (configureBuilder != null)
            {
                var builder = new TypeDataInfoBuilder();
                configureBuilder(builder);

                var configuredCriteria = builder.Build();

                _options.TypeLocatorOptions.TypeInfos.Add(configuredCriteria);
            }

            if (info != null)
            {
                _options.TypeLocatorOptions.TypeInfos.Add(info);
            }

  

            if (_options.TypeLocatorOptions.TypeInfos.Any() != true)
            {
                var findAll = TypeDataInfoBuilder
                    .Create()
                    .Tag(string.Empty)
                    .Build();

                _options.TypeLocatorOptions.TypeInfos.Add(findAll);
            }
        }


        public Plugin GetPlugin(string name, Version version)
        {
            foreach (var pluginSource in _plugins)
            {
                var foundPlugin = pluginSource.GetPlugin(name, version);

                if (foundPlugin == null)
                {
                    continue;
                }

                return foundPlugin;
            }

            return null;
        }

        public List<Plugin> GetPlugins()
        {
            return _plugins.SelectMany(x => x.GetPlugins()).ToList();
        }

        public void Initialize()
        {
            if (!string.IsNullOrWhiteSpace(_assemblyPath) && _assembly == null)
            {
                if (!File.Exists(_assemblyPath))
                {
                    throw new ArgumentException($"Assembly in path {_assemblyPath} does not exist.");
                }
            }

            if (_assembly == null && File.Exists(_assemblyPath) || File.Exists(_assemblyPath) && _pluginAssemblyLoadContext == null)
            {
                _pluginAssemblyLoadContext = new PluginAssemblyLoadContext(_assemblyPath, _options.PluginLoadContextOptions);
                _assembly = _pluginAssemblyLoadContext.Load();
            }

            _plugins = new List<TypePluginSourceProvider>();

            var finder = new TypeLocator();

            var handledPluginTypes = new List<Type>();
            foreach (var info in _options.TypeLocatorOptions.TypeInfos)
            {
                var pluginTypes = finder.Find(info, _assembly, _pluginAssemblyLoadContext);

                foreach (var type in pluginTypes)
                {
                    if (handledPluginTypes.Contains(type))
                    {
                        // Make sure to create only a single type plugin catalog for each plugin type. 
                        // The type catalog will add all the matching tags
                        continue;
                    }

                    var typePluginSource = new TypePluginSourceProvider(type,
                        new DefaultPluginSourceMetaDataLoader()
                        {
                            PluginMetaDataLoader = _options.PluginNameOptions,
                            TypeLocatorContext = _pluginAssemblyLoadContext,
                            TypeLocatorOptions = _options.TypeLocatorOptions
                        });

                    typePluginSource.Initialize();

                    _plugins.Add(typePluginSource);

                    handledPluginTypes.Add(type);
                }
            }

            IsInitialized = true;
        }

        public async Task InitializeAsync()
        {
            if (!string.IsNullOrWhiteSpace(_assemblyPath) && _assembly == null)
            {
                if (!File.Exists(_assemblyPath))
                {
                    throw new ArgumentException($"Assembly in path {_assemblyPath} does not exist.");
                }
            }

            if (_assembly == null && File.Exists(_assemblyPath) || File.Exists(_assemblyPath) && _pluginAssemblyLoadContext == null)
            {
                _pluginAssemblyLoadContext = new PluginAssemblyLoadContext(_assemblyPath, _options.PluginLoadContextOptions);
                _assembly = _pluginAssemblyLoadContext.Load();
            }

            _plugins = new List<TypePluginSourceProvider>();

            var finder = new TypeLocator();

            var handledPluginTypes = new List<Type>();
            foreach (var info in _options.TypeLocatorOptions.TypeInfos)
            {
                var pluginTypes = finder.Find(info, _assembly, _pluginAssemblyLoadContext);

                foreach (var type in pluginTypes)
                {
                    if (handledPluginTypes.Contains(type))
                    {
                        continue;
                    }

                    var typePluginSource = new TypePluginSourceProvider(type,
                        new DefaultPluginSourceMetaDataLoader()
                        {
                            PluginMetaDataLoader = _options.PluginNameOptions,
                            TypeLocatorContext = _pluginAssemblyLoadContext,
                            TypeLocatorOptions = _options.TypeLocatorOptions
                        });

                    await typePluginSource.InitializeAsync();

                    _plugins.Add(typePluginSource);

                    handledPluginTypes.Add(type);
                }
            }

            IsInitialized = true;
        }
    }
}
