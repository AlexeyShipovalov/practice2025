using Xunit;
using task14;
namespace task14tests
{
    public class PerformanceTests
    {
        [Fact]
        public void TestIntegralOfSin()
        {
            var SIN = (double x) => Math.Sin(x);
            double result = DefiniteIntegral.Solve(-100, 100, SIN, 1e-4, 8);
            Assert.InRange(result, -1e-4, 1e-4);
        }
    }
}
