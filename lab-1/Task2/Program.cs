using System.Text;
using MathNet.Numerics.Distributions;
using Task1;

namespace Task2;

class Program
{
    static void Main()
    {
        int repetitions = 50;
        int collectionSize = 1_000_000;
        int binsCount = 7;

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine($"=== ЕТАП 1. ЗБІР ЕКСПЕРИМЕНТАЛЬНИХ ДАНИХ (N = {repetitions}) ===");
        double[] realTimes = SortingBenchmark.CollectSortExecutionTimes(repetitions, collectionSize);
        
        var empiricalStats = new SampleStatistics(realTimes);
        Console.WriteLine($"Емпіричне середнє (mu~):   {empiricalStats.Mean:F3} ms");
        Console.WriteLine($"Емпіричний stdDev (sigma~): {empiricalStats.StandardDeviation:F3} ms");
        Console.WriteLine($"Діапазон часу:              [{empiricalStats.Min:F3}; {empiricalStats.Max:F3}] ms");
        
        var rawBins = empiricalStats.BuildHistogram(binsCount);
        HistogramPrinter.PrintHistogram(rawBins.ToArray());
        
        double mu = empiricalStats.Mean;
        double sigma = empiricalStats.StandardDeviation;
        var normalDist = new Normal(mu, sigma);

        Func<double, double, double> normalCdfDiff = (left, right) =>
        {
            return normalDist.CumulativeDistribution(right) - normalDist.CumulativeDistribution(left);
        };
        
        var testResult = GoodnessOfFitTest.Verify(
            rawBins,
            repetitions,
            estimatedParamsCount: 2,
            normalCdfDiff
        );

        Console.WriteLine("\n=== ЕТАП 2. ІДЕНТИФІКАЦІЯ ЗАКОНУ (ПЕРЕВІРКА ХІ-КВАДРАТ) ===");
        Console.WriteLine($"Інтервалів після злиття:    {testResult.MergedIntervals}");
        Console.WriteLine($"Ступенів вільності (df):     {testResult.DegreesOfFreedom}");
        Console.WriteLine($"chi^2 розраховане:           {testResult.ChiSquareObserved:F4}");
        Console.WriteLine($"chi^2 критичне:              {testResult.ChiSquareCritical:F4}");
        Console.WriteLine($"Результат узгодження:        {(testResult.IsAccepted ? "ГІПОТЕЗА ПРИЙНЯТА" : "ГІПОТЕЗА ВІДХИЛЕНА")}");
        
        Console.WriteLine("\n=== ЕТАП 3. ІМІТАЦІЙНИЙ АЛГОРИТМ ТА ОЦІНКА ТОЧНОСТІ ===");
        double[] simulatedTimes = SimulationGenerator.GenerateBatch(mu, sigma, repetitions);
        var simStats = new SampleStatistics(simulatedTimes);

        Console.WriteLine($"Імітоване середнє:           {simStats.Mean:F3} ms");
        Console.WriteLine($"Імітований stdDev:           {simStats.StandardDeviation:F3} ms");

        double meanError = Math.Abs(empiricalStats.Mean - simStats.Mean) / empiricalStats.Mean * 100.0;
        double stdError = Math.Abs(empiricalStats.StandardDeviation - simStats.StandardDeviation) / empiricalStats.StandardDeviation * 100.0;

        Console.WriteLine($"\nПохибка відтворення середнього: {meanError:F2}%");
        Console.WriteLine($"Похибка відтворення дисперсії:  {stdError:F2}%");
    }
}