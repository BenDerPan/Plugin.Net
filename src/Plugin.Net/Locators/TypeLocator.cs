using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PluginDotNet.Locators
{
    public class TypeLocator
    {
        public bool IsMatch(TypeDataInfo info,Type type,ITypeLocatorContext ctx)
        {
            if (info.Query!=null)
            {
                return info.Query(ctx, type);
            }

            if (info.IsAbstract!=null)
            {
                if (type.IsAbstract!=info.IsAbstract.GetValueOrDefault())
                {
                    return false;
                }
            }

            if (info.IsInterface!=null)
            {
                if (type.IsInterface!=info.IsInterface.GetValueOrDefault())
                {
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(info.Name))
            {
                var regExp = NameToRegex(info.Name);
                if (!regExp.IsMatch(type.FullName))
                {
                    var hasNameMatch = string.Equals(info.Name, type.Name, StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(info.Name, type.FullName, StringComparison.InvariantCultureIgnoreCase);

                    if (!hasNameMatch)
                    {
                        return false;
                    }
                }
            }

            if (info.Inherits!=null)
            {
                var inheriedType = ctx.FindType(info.Inherits);
                if (inheriedType == null || !inheriedType.IsAssignableFrom(type))
                {
                    return false;
                }
            }

            if (info.Implements!=null)
            {
                var interfaceType = ctx.FindType(info.Implements);
                if (interfaceType==null||!interfaceType.IsAssignableFrom(type))
                {
                    return false;
                }
            }

            if (info.AssignableTo!=null)
            {
                var assignType = ctx.FindType(info.AssignableTo);
                if (assignType==null||!assignType.IsAssignableFrom(type))
                {
                    return false;
                }
            }

            if (info.HasAttribute!=null)
            {
                var attrbutes = type.GetCustomAttributesData();
                var hasFound = false;

                foreach (var attrData in attrbutes)
                {
                    if (!string.Equals(attrData.AttributeType.FullName,info.HasAttribute.FullName,StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    hasFound = true;
                    break;
                }

                if (!hasFound)
                {
                    return false;
                }
            }

            return true;
        }

        public List<Type> Find(TypeDataInfo info,Assembly assembly,ITypeLocatorContext ctx)
        {
            if (info==null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            var retList = new List<Type>();

            if (assembly==null)
            {
                return retList;
            }

            var types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                var isMatch = IsMatch(info, type, ctx);
                if (isMatch)
                {
                    retList.Add(type);
                }
            }

            return retList;
        }

        private static Regex NameToRegex(string nameFilter)
        {
            // https://stackoverflow.com/a/30300521/66988
            var regex = "^" + Regex.Escape(nameFilter).Replace("\\?", ".").Replace("\\*", ".*") + "$";

            return new Regex(regex, RegexOptions.Compiled);
        }
    }
}
