﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PluginDotNet.Contexts
{
    public class PluginLoadContextOptions
    {
        /// <summary>
        /// Gets or sets if the plugin should by default to use the assemblies referenced by the plugin or by the host application. Useful in situations where it is important that the host application
        /// and the plugin use the same version of the assembly, even if they reference different versions.
        /// </summary>
        public UseHostApplicationAssembliesEnum UseHostApplicationAssemblies { get; set; } = Defaults.UseHostApplicationAssemblies;

        /// <summary>
        /// Gets or sets the assemblies which the plugin should use if UseHostApplicationAssemblies is set to Selected. These assemblies are used
        /// even if the plugin itself references an another version of the same assembly.
        /// </summary>
        public List<AssemblyName> HostApplicationAssemblies { get; set; } = Defaults.HostApplicationAssemblies;

        /// <summary>
        /// Gets or sets the additional runtime paths which are used when locating plugin assemblies  
        /// </summary>
        public List<string> AdditionalRuntimePaths { get; set; } = Defaults.AdditionalRuntimePaths;

        public static class Defaults
        {
            /// <summary>
            /// Gets or sets if the plugin should by default to use the assemblies referenced by the plugin or by the host application. Default = Always. Useful in situations where it is important that the host application
            /// and the plugin use the same version of the assembly, even if they reference different versions. 
            /// </summary>
            public static UseHostApplicationAssembliesEnum UseHostApplicationAssemblies { get; set; } = UseHostApplicationAssembliesEnum.Always;

            /// <summary>
            /// Gets or sets the assemblies which the plugin should use if UseHostApplicationAssemblies is set to Selected. These assemblies are used
            /// even if the plugin itself references an another version of the same assembly.
            /// </summary>
            public static List<AssemblyName> HostApplicationAssemblies { get; set; } = new List<AssemblyName>();

            /// <summary>
            /// Gets or sets the additional runtime paths which are used when locating plugin assemblies  
            /// </summary>
            public static List<string> AdditionalRuntimePaths { get; set; } = new List<string>();
        }
    }
}
