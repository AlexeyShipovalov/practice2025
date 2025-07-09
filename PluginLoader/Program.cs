using System;
using PluginLoader; 
namespace PluginLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Укажите путь к папке с плагинами");
                return;
            }
            var plugins = PluginCore.LoadFromDirectory(args[0]);
            plugins.ExecuteAll();
        }
    }
}
