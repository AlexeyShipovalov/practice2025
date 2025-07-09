using Xunit;
using System;
using System.IO;
using PluginLoader;
using System.Reflection;
namespace task10tests
{
    public class PluginLoadingTests : IDisposable
    {
        private readonly string _pluginsDir;

        public PluginLoadingTests()
        {
            _pluginsDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test_plugins");
            Directory.CreateDirectory(_pluginsDir);
            var solutionDir = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                "../../../../"));
            var lib1Path = Path.Combine(solutionDir, "PluginLibrary1/bin/Debug/net8.0/PluginLibrary1.dll");
            var lib2Path = Path.Combine(solutionDir, "PluginLibrary2/bin/Debug/net8.0/PluginLibrary2.dll");
            if (!File.Exists(lib1Path))
                throw new FileNotFoundException($"PluginLibrary1.dll не найден в: {lib1Path}");
            if (!File.Exists(lib2Path))
                throw new FileNotFoundException($"PluginLibrary2.dll не найден в : {lib2Path}");
            File.Copy(lib1Path, Path.Combine(_pluginsDir, "PluginLibrary1.dll"), true);
            File.Copy(lib2Path, Path.Combine(_pluginsDir, "PluginLibrary2.dll"), true);
        }
        public void Dispose()
        {
            if (Directory.Exists(_pluginsDir))
            {
                Directory.Delete(_pluginsDir, true);
            }
        }
        [Fact]
        public void LoadPlugins_ShouldReturnCorrectCount()
        {
            var plugins = PluginCore.LoadFromDirectory(_pluginsDir);
            Assert.Equal(3, plugins.Count);
        }
        [Fact]
        public void PluginsShouldExecuteInCorrectOrder()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            var plugins = PluginCore.LoadFromDirectory(_pluginsDir);
            plugins.ExecuteAll();
            var result = output.ToString();
            Assert.Contains("Plugin B executed", result);
            Assert.Contains("Plugin A executed", result); 
            Assert.Contains("Plugin C executed", result);
        }
    }
}
