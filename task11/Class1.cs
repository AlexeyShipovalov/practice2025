using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11
{
    public class DynamicClassCreator
    {
        public static object? CreateCalculator()
        {
            string code = 
                "using System;\n"+
                "public class Calculator\n" +
                "{\n" +
                "    public int Add(int a, int b) => a + b;\n" +
                "    public int Minus(int a, int b) => a - b;\n" +
                "    public int Mul(int a, int b) => a * b;\n" +
                "    public int Div(int a, int b)\n" +
                "    {\n" +
                "        if (b == 0) throw new DivideByZeroException();\n" +
                "        return a / b;\n" +
                "    }\n" +
                "}";
            var compilation = CSharpCompilation.Create("DynamicAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));
            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                foreach (var diagnostic in result.Diagnostics)
                    Console.WriteLine(diagnostic.ToString());
                return null;
            }
            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());
            return assembly.CreateInstance("Calculator");
        }
    }
}
