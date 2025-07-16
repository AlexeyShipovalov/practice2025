using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ScottPlot;
namespace task14
{
    public static class Integral
    {
        public static double SolveSingleThread(double a, double b, Func<double, double> f, double step)
        {
            return DefiniteIntegral.Solve(a, b, f, step, 1);
        }
        public static void Main()
        {
            const double a = -100;
            const double b = 100;
            Func<double, double> function = Math.Sin;
            double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
            int[] threadCounts = { 1, 2, 4, 8, 16, 32 };
            double optimalStep = 0;
            foreach (var step in steps)
            {
                double result = SolveSingleThread(a, b, function, step);
                if (Math.Abs(result) < 1e-4)
                {
                    optimalStep = step;
                    break;
                }
            }
            if (optimalStep == 0)
            {
                Console.WriteLine("Не удалось достичь точности 1e-4.");
                return;
            }
            var timeByThreads = new Dictionary<int, double>();
            double singleTime = MeasureAverageTime(() => SolveSingleThread(a, b, function, optimalStep));
            foreach (var threads in threadCounts)
            {
                timeByThreads[threads] = MeasureAverageTime(() => DefiniteIntegral.Solve(a, b, function, optimalStep, threads));
            }
            var best = timeByThreads.OrderBy(kv => kv.Value).First();
            double speedup = (timeByThreads[1] - best.Value) / timeByThreads[1] * 100;
            SaveResults(optimalStep, best.Key, singleTime, best.Value, speedup);
            SaveGraphData(timeByThreads);
            GeneratePlot(timeByThreads);
            Console.WriteLine($"Оптимальный шаг: {optimalStep:E1}");
            Console.WriteLine($"Оптимальное количество потоков: {best.Key}");
            Console.WriteLine($"Ускорение: {speedup:F1}%");
        }
        private static double MeasureAverageTime(Func<double> action)
        {
            var sw = new Stopwatch();
            double total = 0;
            const int runs = 5;
            for (int i = 0; i < runs; i++)
            {
                sw.Restart();
                action();
                sw.Stop();
                total += sw.Elapsed.TotalMilliseconds;
            }
            return total / runs;
        }
        private static void SaveResults(double step, int bestThreads, double singleTime, double multiTime, double speedup)
        {
            Directory.CreateDirectory("task15");
            string path = Path.Combine("task15", "results.txt");
            File.WriteAllText(path, $@"Результаты анализа
            =================
            Оптимальный шаг: {step:E1}
            Оптимальное количество потоков: {bestThreads}
            Однопоточное время: {singleTime:F2} мс
            Многопоточное время: {multiTime:F2} мс
            Ускорение: {speedup:F1}%
            ");
        }
        private static void SaveGraphData(Dictionary<int, double> timings)
        {
            Directory.CreateDirectory("task15");
            string path = Path.Combine("task15", "graph_data.csv");
            var lines = timings.Select(kv => $"{kv.Key},{kv.Value:F2}");
            File.WriteAllLines(path, lines);
        }
        private static void GeneratePlot(Dictionary<int, double> timings)
        {
            var plt = new Plot(800, 600);
            plt.AddScatter(
                timings.Keys.Select(k => (double)k).ToArray(),
                timings.Values.ToArray(),
                color: System.Drawing.Color.Blue,
                lineWidth: 2,
                markerSize: 5
            );
            plt.XLabel("Количество потоков");
            plt.YLabel("Время выполнения (мс)");
            plt.Title("Производительность вычисления интеграла");
            plt.Grid(enable: true);
            string plotPath = Path.Combine("task15", "performance_plot.png");
            plt.SaveFig(plotPath);
        }
    }
}
