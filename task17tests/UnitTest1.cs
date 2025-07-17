using System;
using System.Threading;
using Xunit;
using task17;
namespace task17tests
{
    public class ServerThreadTests
    {
        [Fact]
        public void TestHardStop_StopsImmediately()
        {
            var server = new ServerThread();
            int counter = 0;
            server.EnqueueCommand(new TestCommand(() => counter++));
            server.EnqueueCommand(new HardStop(server));
            server.EnqueueCommand(new TestCommand(() => counter++));
            server.Dispose();
            Assert.Equal(1, counter);
        }
        [Fact]
        public void TestSoftStop_StopsAfterQueueEmpty()
        {
            var server = new ServerThread();
            int counter = 0;
            server.EnqueueCommand(new TestCommand(() => counter++));
            server.EnqueueCommand(new SoftStop(server));
            server.EnqueueCommand(new TestCommand(() => counter++)); 
            server.Dispose();
            Assert.Equal(1, counter);
        }
        [Fact]
        public void TestCannotEnqueueAfterStop()
        {
            var server = new ServerThread();
            server.EnqueueCommand(new HardStop(server));
            Assert.Throws<InvalidOperationException>(() => 
                server.EnqueueCommand(new TestCommand(() => {})));
        }
        private class TestCommand : ICommand
        {
            private readonly Action _action;
            public TestCommand(Action action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
            }
            public void Execute()
            {
                _action();
            }
        }
    }
}
