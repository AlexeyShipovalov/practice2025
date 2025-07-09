using PluginAttributes;
using PluginLoader;
namespace PluginLibrary1
{
    [PluginLoad("PluginA")]
    [DependsOn("PluginB")]
    public class PluginA : IPlugin
    {
        public string Name => "PluginA";
        public void Execute() => Console.WriteLine("Plugin A executed");
    }
    [PluginLoad("PluginB")]
    public class PluginB : IPlugin
    {
        public string Name => "PluginB";
        public void Execute() => Console.WriteLine("Plugin B executed");
    }
}
