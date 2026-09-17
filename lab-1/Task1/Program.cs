namespace Task1;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        int sampleSize = 10000;
        int initialBins = 20;

        // Task 1.1
        double[] testLambdas = { 0.5, 1.0, 2.5 };

        Console.WriteLine("=== ЗАВДАННЯ 1.1: ЕКСПЕРИМЕНТИ З ЕКСПОНЕНЦІЙНИМ РОЗПОДІЛОМ ===");
        foreach (double lambda in testLambdas)
        {
            Console.WriteLine($"\n--- Проведення експерименту для lambda = {lambda} ---");

            double[] expData = RandomVariateGenerator.GenerateExponential(lambda, sampleSize);
            var expStats = new SampleStatistics(expData);
            var expBins = expStats.BuildHistogram(initialBins);

            PrintHistogram(expBins.ToArray(), $"Гістограма експоненційного розподілу (lambda = {lambda})");

            Func<double, double, double> expProb = (left, right) =>
                Math.Exp(-lambda * left) - Math.Exp(-lambda * right);

            var expResult = GoodnessOfFitTest.Verify(
                expBins,
                sampleSize,
                estimatedParamsCount: 1,
                expProb
            );

            Console.WriteLine($"Теоретичні:  Середнє = {1.0 / lambda:F4}, Дисперсія = {1.0 / (lambda * lambda):F4}");
            Console.WriteLine($"Емпіричні:   Середнє = {expStats.Mean:F4}, Дисперсія = {expStats.Variance:F4}");
            Console.WriteLine($"Ступені вільності (df): {expResult.DegreesOfFreedom} (m = {expResult.MergedIntervals})");
            Console.WriteLine($"Хі2 розрах: {expResult.ChiSquareObserved:F4} | Хі2 крит: {expResult.ChiSquareCritical:F4} | Гіпотезу прийнято: {expResult.IsAccepted}");
        }

        // Завдання 1.2: Рівномірний розподіл (0, 1) (ЛКГ) для різних (a, c)
        Console.WriteLine("\n=== ЗАВДАННЯ 1.2: ЕКСПЕРИМЕНТИ З РІВНОМІРНИМ РОЗПОДІЛОМ (ЛКГ) ===");

        var lcgConfigs = new (long a, long c, string Description)[]
        {
            (1220703125L, 2147483648L, "Завдання з методички: a = 5^13, c = 2^31"),
            (16807L,      2147483647L, "Park-Miller (MINSTD): a = 7^5, c = 2^31 - 1"),
            (48271L,      2147483647L, "Оновлений Park-Miller: a = 48271, c = 2^31 - 1"),
            (1664525L,    4294967296L, "Numerical Recipes: a = 1664525, c = 2^32")
        };
  
        foreach (var (a, c, description) in lcgConfigs)
        {
            Console.WriteLine($"\n--- Дослідження конфігурації: {description} ---");
            Console.WriteLine($"Параметри: a = {a}, c = {c}");

            double[] uniformData = RandomVariateGenerator.GenerateLcgUniform(sampleSize, a, c);
            var uniStats = new SampleStatistics(uniformData);
            var uniBins = uniStats.BuildHistogram(initialBins);

            PrintHistogram(uniBins.ToArray(), $"Гістограма ЛКГ: a={a}, c={c}");

            Func<double, double, double> uniformProb = (left, right) => right - left;

            var uniResult = GoodnessOfFitTest.Verify(
                uniBins,
                sampleSize,
                estimatedParamsCount: 0,
                uniformProb
            );

            Console.WriteLine($"Теоретичні:  Середнє = 0.5000, Дисперсія = {1.0 / 12.0:F4}");
            Console.WriteLine($"Емпіричні:   Середнє = {uniStats.Mean:F4}, Дисперсія = {uniStats.Variance:F4}");
            Console.WriteLine($"Ступені вільності (df): {uniResult.DegreesOfFreedom} (m = {uniResult.MergedIntervals})");
            Console.WriteLine($"Хі2 розрах: {uniResult.ChiSquareObserved:F4} | Хі2 крит: {uniResult.ChiSquareCritical:F4} | Гіпотезу прийнято: {uniResult.IsAccepted}");
        }
    }
    
    public static void PrintHistogram(HistogramBin[] bins, string? title = null)
    {
        HistogramPrinter.PrintHistogram(bins, title);
    }
}