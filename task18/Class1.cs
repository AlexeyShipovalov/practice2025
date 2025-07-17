using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
namespace task18
{
    public interface ICommand
    {
        void Execute();
    }
    public interface IScheduler
    {
        bool HasCommand();
        ICommand Select();
        void Add(ICommand cmd);
    }
    public class RoundRobinScheduler : IScheduler, IDisposable
    {
        private readonly Queue<ICommand> _commands = new Queue<ICommand>();
        private readonly object _lock = new object();
        private bool _disposed;
        public bool HasCommand()
        {
            lock (_lock)
            {
                return _commands.Count > 0 && !_disposed;
            }
        }
        public ICommand Select()
        {
            lock (_lock)
            {
                if (_disposed) throw new ObjectDisposedException(nameof(RoundRobinScheduler));
                if (_commands.Count == 0)
                    throw new InvalidOperationException("Нет доступных команд");
                var cmd = _commands.Dequeue();
                _commands.Enqueue(cmd);
                return cmd;
            }
        }
        public void Add(ICommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd));
            lock (_lock)
            {
                if (_disposed) return;
                _commands.Enqueue(cmd);
            }
        }
        public void Dispose()
        {
            lock (_lock)
            {
                _commands.Clear();
                _disposed = true;
            }
        }
    }
    public class ServerThread : IDisposable
    {
        private readonly Thread _thread;
        private readonly BlockingCollection<ICommand> _commandQueue = new BlockingCollection<ICommand>();
        private readonly IScheduler _scheduler;
        private volatile bool _isRunning = true;
        private volatile bool _softStopRequested = false;
        private readonly ManualResetEvent _stoppedEvent = new ManualResetEvent(false);
        private bool _disposed;
        public ServerThread(IScheduler? scheduler = null)
        {
            _scheduler = scheduler ?? new RoundRobinScheduler();
            _thread = new Thread(Run)
            {
                IsBackground = true,
                Name = "ServerThread Worker"
            };
            _thread.Start();
        }
        public void EnqueueCommand(ICommand command)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ServerThread));
            if (!_isRunning || _softStopRequested)
                throw new InvalidOperationException("Невозможно поставить команду в очередь после запроса на остановку");
            _commandQueue.Add(command);
        }
        private void Run()
        {
            try
            {
                while (_isRunning && !_disposed)
                {
                    try
                    {
                        if (_scheduler.HasCommand())
                        {
                            var command = _scheduler.Select();
                            command.Execute();
                            Thread.Sleep(1);
                            continue;
                        }
                        if (_commandQueue.TryTake(out ICommand? newCommand, 100))
                        {
                            newCommand.Execute();
                        }
                        else if (_softStopRequested && _commandQueue.Count == 0 && !_scheduler.HasCommand())
                        {
                            break;
                        }
                    }
                    catch (Exception ex) when (ex is InvalidOperationException || ex is ThreadInterruptedException)
                    {
                        break;
                    }
                }
            }
            finally
            {
                _isRunning = false;
                try
                {
                    _stoppedEvent.Set();
                }
                catch (ObjectDisposedException)
                {
                   
                }
            }
        }

        public void RequestSoftStop()
        {
            _softStopRequested = true;
        }
        public void RequestHardStop()
        {
            _isRunning = false;
            _commandQueue.CompleteAdding();
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            RequestHardStop();
            try
            {
                if (!_stoppedEvent.WaitOne(1000))
                {
                    _thread.Interrupt();
                }
            }
            finally
            {
                _stoppedEvent.Dispose();
                _commandQueue.Dispose();
                if (_scheduler is IDisposable disposableScheduler)
                {
                    disposableScheduler.Dispose();
                }
            }
        }
    }
    public class HardStop : ICommand
    {
        private readonly ServerThread _targetThread;
        public HardStop(ServerThread targetThread)
        {
            _targetThread = targetThread ?? throw new ArgumentNullException(nameof(targetThread));
        }
        public void Execute()
        {
            _targetThread.RequestHardStop();
        }
    }
    public class SoftStop : ICommand
    {
        private readonly ServerThread _targetThread;
        public SoftStop(ServerThread targetThread)
        {
            _targetThread = targetThread ?? throw new ArgumentNullException(nameof(targetThread));
        }
        public void Execute()
        {
            _targetThread.RequestSoftStop();
        }
    }
    public class LongRunningCommand : ICommand
    {
        private readonly IScheduler _scheduler;
        private int _remainingWork;
        private readonly object _lock = new object();
        public LongRunningCommand(IScheduler scheduler, int workUnits)
        {
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _remainingWork = workUnits;
        }
        public void Execute()
        {
            lock (_lock)
            {
                if (_remainingWork <= 0) return;
                _remainingWork--;
                if (_remainingWork > 0)
                {
                    _scheduler.Add(this);
                }
            }
            Thread.Sleep(10);
        }
        public bool IsComplete => _remainingWork <= 0;
    }
}
