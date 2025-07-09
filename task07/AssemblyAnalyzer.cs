using System;
using System.Reflection;
using System.Linq;

namespace task07
{
    public static class AssemblyAnalyzer
    {
        public static void AnalyzeAndPrint(string assemblyPath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                Console.WriteLine($"Анализ сборки: {assembly.FullName}\n");

                foreach (var type in assembly.GetTypes().Where(t => t.IsClass))
                {
                    PrintTypeInfo(type);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при анализе сборки: {ex.Message}");
            }
        }
        private static void PrintTypeInfo(Type type)
        {
            Console.WriteLine($"\nКЛАСС: {type.Name}");
            PrintAttributes("Атрибуты класса:", type.GetCustomAttributes(false));
            var constructors = type.GetConstructors();
            if (constructors.Any())
            {
                Console.WriteLine("\nКонструкторы:");
                foreach (var ctor in constructors)
                {
                    Console.WriteLine($"- {ctor.Name}");
                    PrintParameters(ctor.GetParameters());
                }
            }
            var properties = type.GetProperties();
            if (properties.Any())
            {
                Console.WriteLine("\nСвойства:");
                foreach (var prop in properties)
                {
                    Console.Write($"- {prop.PropertyType.Name} {prop.Name}");
                    PrintAttributes(" (атрибуты свойства)", prop.GetCustomAttributes(false));
                }
            }
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                            .Where(m => !m.IsSpecialName); 
            if (methods.Any())
            {
                Console.WriteLine("\nМетоды:");
                foreach (var method in methods)
                {
                    Console.Write($"- {method.ReturnType.Name} {method.Name}");
                    PrintParameters(method.GetParameters());
                    PrintAttributes("  (атрибуты метода)", method.GetCustomAttributes(false));
                }
            }
            Console.WriteLine("\n" + new string('=', 60));
        }
        private static void PrintAttributes(string title, object[] attributes)
        {
            if (attributes.Length == 0) return;
            Console.Write(title);
            foreach (var attr in attributes)
            {
                Console.Write($" [{attr.GetType().Name}]");
            }
            Console.WriteLine();
        }
        private static void PrintParameters(ParameterInfo[] parameters)
        {
            if (parameters.Length == 0) return;
            Console.Write("(");
            Console.Write(string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}")));
            Console.WriteLine(")");
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Укажите путь к сборке в аргументах командной строки");
                Console.WriteLine("Пример: dotnet run -- /home/dappi/practice2025/practice2025/task07/bin/Debug/net8.0/task07.dll");
                return;
            }
            AssemblyAnalyzer.AnalyzeAndPrint(args[0]);
        }
    }
}
