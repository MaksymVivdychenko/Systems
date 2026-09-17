using System.Diagnostics;

namespace Task2;

public static class SortingBenchmark
{
    public static double[] CollectSortExecutionTimes(int repetitions = 150, int collectionSize = 1_000_000)
    {
        var timesMs = new double[repetitions];
        var rnd = new Random();
        var buffer = new int[collectionSize];

        // JIT warm-up
        for (int i = 0; i < 50_000; i++) buffer[i] = rnd.Next();
        Array.Sort(buffer, 0, 50_000);

        var stopwatch = new Stopwatch();

        for (int run = 0; run < repetitions; run++)
        {
            // unsorted coll generation
            for (int i = 0; i < collectionSize; i++)
            {
                buffer[i] = rnd.Next();
            }

            // time measurement
            stopwatch.Restart();
            Array.Sort(buffer);
            stopwatch.Stop();

            timesMs[run] = stopwatch.Elapsed.TotalMilliseconds;
        }

        return timesMs;
    }
}