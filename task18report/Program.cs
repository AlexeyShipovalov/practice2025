using System;
using System.Threading.Tasks;
using task18report;
class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            await ReportGenerator.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}
