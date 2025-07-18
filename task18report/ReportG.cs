using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScottPlot;
using task18;
namespace task18report
{
    public static class ReportGenerator
    {
        private const string OutputDir = "output";
        private static readonly List<int> WorkUnits = new List<int>();
        public static async Task Run()
        {
            Directory.CreateDirectory(OutputDir);
            var perfResults = await TestPerformanceAsync();
            await ExecuteTasksAsync();
            GenerateMainReport();
            GeneratePlot();
            Console.WriteLine($"Отчеты сгенерированы в {Path.GetFullPath(OutputDir)}");
        }
        private static async Task<List<(int Tasks, double SingleThread, double MultiThread)>> TestPerformanceAsync()
        {
            var results = new List<(int, double, double)>();
            var testCases = new[] { 1, 2, 3 };
            foreach (int taskCount in testCases)
            {
                var sw = Stopwatch.StartNew();
                var scheduler = new RoundRobinScheduler();
                var syncTasks = Enumerable.Range(0, taskCount)
                    .Select(_ => new LongRunningCommand(scheduler, 5))
                    .ToList();
                foreach (var cmd in syncTasks)
                {
                    while (!cmd.IsComplete) cmd.Execute();
                }
                sw.Stop();
                double singleTime = sw.Elapsed.TotalMilliseconds;
                sw.Restart();
                using (var scheduler2 = new RoundRobinScheduler())
                using (var server = new ServerThread(scheduler2))
                {
                    var tasks = Enumerable.Range(0, taskCount)
                        .Select(_ => new LongRunningCommand(scheduler2, 5))
                        .ToList();

                    foreach (var cmd in tasks)
                    {
                        server.EnqueueCommand(cmd);
                    }
                    await Task.Delay(50);
                }
                sw.Stop();
                results.Add((taskCount, singleTime, sw.Elapsed.TotalMilliseconds));
            }
            return results;
        }
        private static async Task ExecuteTasksAsync()
        {
            using (var scheduler = new RoundRobinScheduler())
            using (var server = new ServerThread(scheduler))
            {
                var random = new Random();
                var tasks = Enumerable.Range(0, 5)
                    .Select(_ => 
                    {
                        var units = random.Next(3, 6);
                        WorkUnits.Add(units);
                        return new LongRunningCommand(scheduler, units);
                    })
                    .ToList();
                foreach (var cmd in tasks)
                {
                    server.EnqueueCommand(cmd);
                }
                await Task.Delay(100);
            }
        }
        private static void GenerateMainReport()
        {
            string path = Path.Combine(OutputDir, "task_report.txt");
            File.WriteAllText(path, 
                $"=== Основной отчет ===\n" +
                $"Всего задач: {WorkUnits.Count}\n" +
                $"Общий объем: {WorkUnits.Sum()}\n" +
                $"Средний объем: {WorkUnits.Average():F1}\n" +
                $"Максимальный: {WorkUnits.Max()}\n" +
                $"Минимальный: {WorkUnits.Min()}"
            );
        }
        private static void GeneratePlot()
        {
            if (WorkUnits.Count == 0) return;
            var plot = new Plot(600, 400);
            plot.Title("Распределение задач");
            plot.XLabel("Номер задачи");
            plot.YLabel("Объем работы");
            double[] xs = Enumerable.Range(1, WorkUnits.Count).Select(x => (double)x).ToArray();
            double[] ys = WorkUnits.Select(x => (double)x).ToArray();
            plot.AddScatter(xs, ys);
            plot.SaveFig(Path.Combine(OutputDir, "tasks18.png"));
        }
    }
}
