using System;
using System.Reflection;
using CommandLib;
namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly assembly = Assembly.LoadFrom(args[0]);
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface)
                {
                    ICommand command = (ICommand)Activator.CreateInstance(type, new object[] { args[1] });
                    command.Execute();
                }
            }
        }
    }
}
