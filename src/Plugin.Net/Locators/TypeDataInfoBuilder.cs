using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDotNet.Locators
{

    public class TypeDataInfoBuilder
    {
        private Type _inherits;
        private Type _implements;
        private Type _assignable;
        private bool? _isAbstract = false;
        private bool? _isInterface = false;
        private string _name;
        private Type _attribute;
        private List<string> _tags = new List<string>();

        public TypeDataInfo Build()
        {
            var res = new TypeDataInfo
            {
                IsInterface = _isInterface,
                Implements = _implements,
                Inherits = _inherits,
                AssignableTo = _assignable,
                Name = _name,
                IsAbstract = _isAbstract,
                HasAttribute = _attribute,
                Tags = _tags
            };

            return res;
        }

        public static implicit operator TypeDataInfo(TypeDataInfoBuilder criteriaBuilder)
        {
            return criteriaBuilder.Build();
        }

        public static TypeDataInfoBuilder Create()
        {
            return new TypeDataInfoBuilder();
        }

        public TypeDataInfoBuilder HasName(string name)
        {
            _name = name;

            return this;
        }

        public TypeDataInfoBuilder Implements<T>()
        {
            return Implements(typeof(T));
        }

        public TypeDataInfoBuilder Implements(Type t)
        {
            _implements = t;

            return this;
        }

        public TypeDataInfoBuilder Inherits<T>()
        {
            return Inherits(typeof(T));
        }

        public TypeDataInfoBuilder Inherits(Type t)
        {
            _inherits = t;

            return this;
        }

        public TypeDataInfoBuilder IsAbstract(bool? isAbstract)
        {
            _isAbstract = isAbstract;

            return this;
        }

        public TypeDataInfoBuilder IsInterface(bool? isInterface)
        {
            _isInterface = isInterface;

            return this;
        }

        public TypeDataInfoBuilder AssignableTo(Type assignableTo)
        {
            _assignable = assignableTo;

            return this;
        }

        public TypeDataInfoBuilder HasAttribute(Type attribute)
        {
            _attribute = attribute;

            return this;
        }

        public TypeDataInfoBuilder Tag(string tag)
        {
            if (_tags == null)
            {
                _tags = new List<string>();
            }

            if (_tags.Contains(tag))
            {
                return this;
            }

            _tags.Add(tag);

            return this;
        }

        public TypeDataInfoBuilder Tag(params string[] tags)
        {
            if (_tags == null)
            {
                _tags = new List<string>();
            }

            foreach (var tag in tags)
            {
                if (_tags.Contains(tag))
                {
                    continue;
                }

                _tags.Add(tag);
            }


            return this;
        }
    }
}
