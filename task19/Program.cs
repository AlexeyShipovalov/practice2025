using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ScottPlot;
namespace task19
{
    public interface ICommand
    {
        void Execute();
    }
    public class TestCommand(int id) : ICommand
    {
        int counter = 0;
        public void Execute()
        {
            Console.WriteLine($"Поток {id} вызов {++counter}");
        }
        public int Id => id;
        public int Counter => counter;
    }
    public sealed class HardStop : ICommand
    {
        private readonly ServerThread _target;
        public HardStop(ServerThread target) => _target = target;
        public void Execute() => _target.RequestHardStop();
    }
    public sealed class ServerThread : IDisposable
    {
        private readonly Thread _thread;
        private readonly BlockingCollection<ICommand> _queue = new();
        private volatile bool _isRunning = true;
        private readonly ManualResetEvent _stopped = new(false);
        private bool _disposed;
        public ServerThread()
        {
            _thread = new Thread(Run) { IsBackground = true, Name = "Работник" };
            _thread.Start();
        }
        public void Enqueue(ICommand cmd)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ServerThread));
            if (!_isRunning) throw new InvalidOperationException("Поток останавливается");
            _queue.Add(cmd);
        }
        private void Run()
        {
            try
            {
                foreach (var cmd in _queue.GetConsumingEnumerable())
                {
                    if (!_isRunning) break;
                    cmd.Execute();
                }
            }
            finally { _isRunning = false; _stopped.Set(); }
        }
        public void RequestHardStop()
        {
            _isRunning = false;
            _queue.CompleteAdding();
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            RequestHardStop();
            _stopped.WaitOne(1000);
            _stopped.Dispose();
            _queue.Dispose();
        }
    }
    static class Program
    {
        private static readonly List<(double T, int Id, int Call)> Points = new();
        private static readonly DateTime T0 = DateTime.UtcNow;
        static void Main()
        {
            File.WriteAllText("report.txt", string.Empty);
            var server = new ServerThread();
            var commands = Enumerable.Range(1, 5)
                                    .Select(id => new TestCommand(id))
                                    .ToList();
            foreach (var cmd in commands)
                for (int i = 0; i < 3; i++)
                    server.Enqueue(new Wrapper(cmd, (c) => Log(c)));
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(1000);
                server.Enqueue(new HardStop(server));
            });
            Thread.Sleep(2000);
            server.Dispose();
            RenderCharts();
            Console.WriteLine("Готово. report.txt, graph.png");
        }
        private static void Log(TestCommand cmd)
        {
            var ts = DateTime.UtcNow.ToString("HH:mm:ss.fff");
            Console.WriteLine($"[{ts}] TestCommand {cmd.Id} вызов {cmd.Counter}");
            File.AppendAllText("report.txt", $"[{ts}] TestCommand {cmd.Id} вызов {cmd.Counter}{Environment.NewLine}");
            lock (Points)
                Points.Add(((DateTime.UtcNow - T0).TotalSeconds, cmd.Id, cmd.Counter));
        }
        private static void RenderCharts()
        {
            var plt = new ScottPlot.Plot();
            double[] xs = Points.Select(p => p.T).ToArray();
            double[] ys = Points.Select(p => (double)p.Id).ToArray();
            plt.Add.Scatter(xs, ys);
            plt.Title("Graph: время выполнения команд");
            plt.XLabel("Время, сек");
            plt.YLabel("ID команды");
            plt.SavePng("graph.png", 1000, 400);
        }
        private sealed class Wrapper : ICommand
        {
            private readonly TestCommand _cmd;
            private readonly Action<TestCommand> _log;
            public Wrapper(TestCommand cmd, Action<TestCommand> log)
            {
                _cmd = cmd;
                _log = log;
            }
            public void Execute()
            {
                _cmd.Execute();
                _log(_cmd);
            }
        }
    }
}
