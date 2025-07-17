using Xunit;
using System;
using System.Threading;
using System.Collections.Generic;
using task18;
namespace task18tests
{
    public class ServerThreadTests : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                try { disposable.Dispose(); } catch { }
            }
            _disposables.Clear();
        }
        private ServerThread CreateServer(IScheduler? scheduler = null)
        {
            var server = new ServerThread(scheduler);
            _disposables.Add(server);
            if (scheduler is IDisposable disposableScheduler)
            {
                _disposables.Add(disposableScheduler);
            }
            return server;
        }
        [Fact]
        public void ServerThread_ExecutesCommandsFromQueue()
        {
            var executed = false;
            var command = new SimpleCommand(() => executed = true);
            var server = CreateServer();
            server.EnqueueCommand(command);
            WaitFor(() => executed, timeoutMs: 500);
            Assert.True(executed);
        }
        [Fact]
        public void ServerThread_HandlesLongWithScheduler()
        {
            var scheduler = new RoundRobinScheduler();
            var server = CreateServer(scheduler);
            var longCommand = new LongRunningCommand(scheduler, workUnits: 3);
            server.EnqueueCommand(longCommand);
            WaitFor(() => longCommand.IsComplete, timeoutMs: 2000);
            Assert.False(longCommand.IsComplete);
        }
        [Fact]
        public void ServerThread_SoftStop_CompletesAllCommands()
        {
            var scheduler = new RoundRobinScheduler();
            var server = CreateServer(scheduler);
            var cmd = new LongRunningCommand(scheduler, workUnits: 2);
            server.EnqueueCommand(cmd);
            server.RequestSoftStop();
            WaitFor(() => cmd.IsComplete, timeoutMs: 2000);
            Assert.True(cmd.IsComplete);
        }
        [Fact]
        public void ServerThread_HardStop_StopsImmediately()
        {
            var scheduler = new RoundRobinScheduler();
            var server = CreateServer(scheduler);
            var cmd = new LongRunningCommand(scheduler, workUnits: 10);
            scheduler.Add(cmd);
            server.RequestHardStop();
            Thread.Sleep(100);
            Assert.False(cmd.IsComplete);
        }
        [Fact]
        public void Dispose_StopsThreadCleanly()
        {
            var server = CreateServer();
            var threadId = 0;
            var command = new SimpleCommand(() => threadId = Thread.CurrentThread.ManagedThreadId);
            server.EnqueueCommand(command);
            WaitFor(() => threadId != 0, 500);
            server.Dispose();
            Thread.Sleep(100);
            Assert.False(server.IsThreadAlive());
        }
        private static void WaitFor(Func<bool> condition, int timeoutMs)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (!condition() && sw.ElapsedMilliseconds < timeoutMs)
            {
                Thread.Sleep(10);
            }
        }
        private class SimpleCommand : ICommand
        {
            private readonly Action _action;
            public SimpleCommand(Action action) => _action = action;
            public void Execute() => _action();
        }
        private class TrackingCommand : ICommand
        {
            private readonly int _id;
            private readonly List<int> _results;
            private readonly IScheduler _scheduler;
            private int _remaining;
            public TrackingCommand(int id, List<int> results, IScheduler scheduler, int remaining)
            {
                _id = id;
                _results = results;
                _scheduler = scheduler;
                _remaining = remaining;
            }
            public void Execute()
            {
                _results.Add(_id);
                _remaining--;
                if (_remaining > 0)
                {
                    _scheduler.Add(this);
                }
            }
        }
    }
    public static class ServerThreadExtensions
    {
        public static bool IsThreadAlive(this ServerThread server)
        {
            if (server == null) return false;
            var field = typeof(ServerThread).GetField(
                "_thread",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field?.GetValue(server) is Thread thread)
            {
                return thread.IsAlive;
            }
            return false;
        }
    }
}
