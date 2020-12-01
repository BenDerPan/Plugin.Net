using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PluginDotNet.Abstractions
{
    public class Plugin
    {
        public string Name { get; }

        public string Description { get; }

        public string ProductVersion { get; }

        public Version Version { get; }

        public Type Type { get; }

        public Assembly Assembly { get; }

        public IPluginSource Source { get; }

        public string Mark => Marks.FirstOrDefault();

        public List<string> Marks { get; }


        public Plugin(Assembly assembly, Type type, string name, Version version, IPluginSource source, string description = "", string productVersion = "",
            string mark = "", List<string> marks = null)
        {
            Assembly = assembly;
            Type = type;
            Name = name;
            Version = version;
            Source = source;
            Description = description;
            ProductVersion = productVersion;
            Marks = marks;

            if (Marks == null)
            {
                Marks = new List<string>();
            }

            if (!string.IsNullOrWhiteSpace(mark))
            {
                Marks.Add(mark);
            }
            if (marks!=null)
            {
                Marks = Marks.Union(marks).ToList();
            }
        }

        public static implicit operator Type(Plugin plugin)
        {
            return plugin.Type;
        }

        public T NewInstance<T>()
        {
            return (T)Activator.CreateInstance(this.Type);
        }

        public bool TryNewInstance<T>(out T instance)
        {
            try
            {
                instance = NewInstance<T>();
                return true;
            }
            catch
            {
                instance = default(T);
            }

            return false;
        }

        public override string ToString()
        {
            return $"{Name}: {Version}";
        }
    }
}
