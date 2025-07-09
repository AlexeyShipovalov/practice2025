using PluginAttributes;
using PluginLoader;
namespace PluginLibrary2
{
    [PluginLoad("PluginC")]
    [DependsOn("PluginA")]
    public class PluginC : IPlugin
    {
        public string Name => "PluginC";
        public void Execute() => Console.WriteLine("Plugin C executed");
    }
}
