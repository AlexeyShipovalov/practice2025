using System;
using System.Reflection;
namespace task07
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
    public class VersionAttribute : Attribute
    {
        public int Major { get; }
        public int Minor { get; }

        public VersionAttribute(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }
    }
    [DisplayName("Пример класса")]
    [Version(1, 0)]
    public class SampleClass
    {
        [DisplayName("Числовое свойство")]
        public int Number { get; set; }

        [DisplayName("Тестовый метод")]
        public void TestMethod()
        {
            Console.WriteLine("Метод выполнен");
        }
    }
    public static class ReflectionHelper
    {
        public static string GetClassInfo(Type type)
        {
            string result = "";
            var displayName = type.GetCustomAttribute<DisplayNameAttribute>();
            if (displayName != null)
            {
                result += $"Название класса: {displayName.DisplayName}\n";
            }
            var version = type.GetCustomAttribute<VersionAttribute>();
            if (version != null)
            {
                result += $"Версия: {version.Major}.{version.Minor}\n";
            }
            result += "\nСвойства:\n";
            foreach (var property in type.GetProperties())
            {
                var propName = property.GetCustomAttribute<DisplayNameAttribute>();
                result += propName != null 
                    ? $"{property.Name} ({propName.DisplayName})\n" 
                    : $"{property.Name} (без описания)\n";
            }
            result += "\nМетоды:\n";
            foreach (var method in type.GetMethods())
            {
                if (method.DeclaringType == typeof(object)) continue;
                
                var methodName = method.GetCustomAttribute<DisplayNameAttribute>();
                result += methodName != null 
                    ? $"{method.Name} ({methodName.DisplayName})\n" 
                    : $"{method.Name} (без описания)\n";
            }
            return result;
        }
    }
}

