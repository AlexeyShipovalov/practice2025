using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
namespace task05
{
    public class ClassAnalyzer
    {
        private readonly Type _type;
        public ClassAnalyzer(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            _type = type;
        }
        public IEnumerable<string> GetPublicMethods()
        {
            return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Select(m => m.Name);
        }
        public IEnumerable<string> GetMethodParams(string methodName)
        {
            var method = _type.GetMethod(methodName);
            if (method == null)
            {
                return Enumerable.Empty<string>();
            }
            return method.GetParameters()
                    .Select(p => $"{p.ParameterType.Name} {p.Name}");
        }
        public IEnumerable<string> GetAllFields()
        {
            return _type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Select(f => f.Name);
        }
        public IEnumerable<string> GetProperties()
        {
            return _type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => p.Name);
        }
        public bool HasAttribute<T>() where T : Attribute
        {
            return _type.GetCustomAttributes(typeof(T), false).Length > 0;
        }
    }
}
