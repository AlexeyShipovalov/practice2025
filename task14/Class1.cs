using System;
using System.Threading;
namespace task14
{
    public class DefiniteIntegral
    {
        private static object lockObj = new object();
        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
        {
            if (threadsNumber <= 0)
                throw new ArgumentException("Количество потоков должно быть положительным");
            double total = 0.0;
            double segmentLength = (b - a) / threadsNumber;
            var barrier = new Barrier(threadsNumber + 1);
            for (int i = 0; i < threadsNumber; i++)
            {
                double localA = a + i * segmentLength;
                double localB = localA + segmentLength;
                new Thread(() =>
                {
                    try
                    {
                        double partialSum = 0.0;
                        for (double x = localA; x < localB-step/2; x += step)
                        {
                            partialSum += (function(x) + function(x + step)) * step / 2;
                        }
                        
                        lock (lockObj)
                        {
                            total += partialSum;
                        }
                    }
                    finally
                    {
                        barrier.SignalAndWait();
                    }
                }).Start();
            }
            barrier.SignalAndWait();
            return total;
        }
    }
}
