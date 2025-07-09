using Xunit;
using System;
using System.IO;
using PluginLoader;
namespace task10tests
{
    public class PluginLoadingTests : IDisposable
    {
        private readonly string _pluginsDir = Path.Combine(Directory.GetCurrentDirectory(), "test_plugins");
        public PluginLoadingTests()
        {
            Directory.CreateDirectory(_pluginsDir);
            string lib1Path = "/home/dappi/practice2025/practice2025/PluginLibrary1/bin/Debug/net8.0/PluginLibrary1.dll";
            string lib2Path = "/home/dappi/practice2025/practice2025/PluginLibrary2/bin/Debug/net8.0/PluginLibrary2.dll";
            if (!File.Exists(lib1Path))
                throw new FileNotFoundException($"Файл не найден: {lib1Path}");
            if (!File.Exists(lib2Path))
                throw new FileNotFoundException($"Файл не найден: {lib2Path}");
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
