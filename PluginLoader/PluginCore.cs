using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginAttributes;
namespace PluginLoader
{
    public interface IPlugin
    {
        string Name { get; }
        void Execute();
    }
    public static class PluginCore
    {
        public static List<IPlugin> LoadFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Неверное расположение: {directoryPath}");
            var assemblies = Directory.GetFiles(directoryPath, "*.dll")
                .Select(LoadAssemblySafe)
                .Where(a => a != null)
                .ToList();
            return LoadFromAssemblies(assemblies);
        }
        public static List<IPlugin> LoadFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            var pluginTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (IsValidPluginType(type))
                    {
                        pluginTypes.Add(type);
                    }
                }
            }
            return CreateInstances(pluginTypes);
        }
        public static void ExecuteAll(this IEnumerable<IPlugin> plugins)
        {
            foreach (var plugin in plugins)
            {
                Console.WriteLine($"выполнение плагина: {plugin.Name}");
                plugin.Execute();
            }
        }
        private static Assembly LoadAssemblySafe(string path)
        {
            try
            {
                return Assembly.LoadFrom(path);
            }
            catch
            {
                return null;
            }
        }
        private static bool IsValidPluginType(Type type)
        {
            return type.GetCustomAttribute<PluginLoadAttribute>() != null && 
                   typeof(IPlugin).IsAssignableFrom(type) &&
                   !type.IsAbstract &&
                   !type.IsInterface;
        }
        private static List<IPlugin> CreateInstances(List<Type> pluginTypes)
        {
            var graph = new Dictionary<string, List<string>>();
            var pluginInfo = new Dictionary<string, Type>();
            foreach (var type in pluginTypes)
            {
                var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                pluginInfo[attr.Name] = type;
                var dependencies = type.GetCustomAttributes<DependsOnAttribute>()
                    .Select(d => d.PluginName)
                    .ToList();

                graph[attr.Name] = dependencies;
            }
            var sortedPlugins = TopologicalSort(graph);
            return sortedPlugins
                .Where(name => pluginInfo.ContainsKey(name))
                .Select(name => (IPlugin)Activator.CreateInstance(pluginInfo[name]))
                .ToList();
        }
        private static List<string> TopologicalSort(Dictionary<string, List<string>> graph)
        {
            var visited = new HashSet<string>();
            var sorted = new List<string>();
            var temp = new HashSet<string>();
            foreach (var node in graph.Keys)
            {
                if (!visited.Contains(node))
                {
                    Visit(node, graph, visited, temp, sorted);
                }
            }
            return sorted;
        }
        private static void Visit(string node, Dictionary<string, List<string>> graph, 
            HashSet<string> visited, HashSet<string> temp, List<string> sorted)
        {
            if (temp.Contains(node))
                throw new InvalidOperationException($"Циклическая зависимость обнаружена при {node}");
            if (visited.Contains(node))
                return;
            temp.Add(node);
            foreach (var neighbor in graph[node])
            {
                if (!graph.ContainsKey(neighbor))
                    throw new InvalidOperationException($"Зависимость не найдена: {neighbor}");
                Visit(neighbor, graph, visited, temp, sorted);
            }
            temp.Remove(node);
            visited.Add(node);
            sorted.Add(node);
        }
    }
}
