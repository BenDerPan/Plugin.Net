using System;
using System.Collections.Generic;
using System.Text;

namespace PluginDotNet.Contexts
{
    public enum UseHostApplicationAssembliesEnum
    {
        /// <summary>
        /// Never use user host application's assemblies
        /// </summary>
        Never,

        /// <summary>
        /// Only use the listed hosted application assemblies
        /// </summary>
        Selected,

        /// <summary>
        /// Always try to use host application's assemblies
        /// </summary>
        Always
    }
}
