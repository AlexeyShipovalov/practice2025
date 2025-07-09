using System;
namespace PluginAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginLoadAttribute : Attribute
    {
        public string Name { get; }
        public PluginLoadAttribute(string name) => Name = name;
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute
    {
        public string PluginName { get; }
        public DependsOnAttribute(string pluginName) => PluginName = pluginName;
    }
}
