using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17
{
    public interface ICommand
    {
        void Execute();
    }
    public class ServerThread : IDisposable
    {
        private readonly Thread _thread;
        private readonly BlockingCollection<ICommand> _commandQueue = new BlockingCollection<ICommand>();
        private volatile bool _isRunning = true;
        private volatile bool _softStopRequested = false;
        public ServerThread()
        {
            _thread = new Thread(Run);
            _thread.Start();
        }
        public void EnqueueCommand(ICommand command)
        {
            if (!_isRunning || _softStopRequested)
                throw new InvalidOperationException("Cannot enqueue command after stop request");
            _commandQueue.Add(command);
        }
        private void Run()
        {
            while (_isRunning)
            {
                try
                {
                    var command = _commandQueue.Take();
                    command.Execute();
                    if (_softStopRequested && _commandQueue.Count == 0)
                        break;
                }
                catch (InvalidOperationException)
                {
                    break;
                }
            }
            _isRunning = false;
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
            RequestHardStop();
            if (!_thread.Join(TimeSpan.FromSeconds(1)))
                _thread.Interrupt();
        }
        private class DCommand : ICommand
        {
            public void Execute() { }
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
}
