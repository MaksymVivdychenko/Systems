using MathNet.Numerics.Distributions;

namespace Task1;

public static class GoodnessOfFitTest
{
    public record ChiSquareResult(
        int MergedIntervals,
        int DegreesOfFreedom,
        double ChiSquareObserved,
        double ChiSquareCritical,
        bool IsAccepted);

    public record EvaluatedBin(double Left, double Right, int Observed, double Expected)
    {
        public int Observed { get; set; } = Observed;
        public double Expected { get; set; } = Expected;
        public double Right { get; set; } = Right;
    }

    public static ChiSquareResult Verify(
        List<HistogramBin> rawBins,
        int sampleSize,
        int estimatedParamsCount,
        Func<double, double, double> theoreticalProbFunc,
        double alpha = 0.05)
    {
        var bins = rawBins.Select(b => new EvaluatedBin(
            b.Left,
            b.Right,
            b.Count,
            sampleSize * theoreticalProbFunc(b.Left, b.Right)
        )).ToList();

        // Merge intervals with small frequencies (n_j < 5 or E_j < 5)
        for (int i = bins.Count - 1; i > 0; i--)
        {
            if (bins[i].Observed < 5 || bins[i].Expected < 5.0)
            {
                bins[i - 1].Observed += bins[i].Observed;
                bins[i - 1].Expected += bins[i].Expected;
                bins[i - 1].Right = bins[i].Right;
                bins.RemoveAt(i);
            }
        }

        double chiSquare = bins.Sum(b => Math.Pow(b.Observed - b.Expected, 2) / b.Expected);
        int df = Math.Max(1, bins.Count - 1 - estimatedParamsCount);
        double chiSquareCrit = ApproximateChiSquareCritical(df, alpha);

        return new ChiSquareResult(
            bins.Count,
            df,
            chiSquare,
            chiSquareCrit,
            chiSquare < chiSquareCrit
        );
    }

    private static double ApproximateChiSquareCritical(int df, double alpha)
    {
        return ChiSquared.InvCDF(df, 1.0 - alpha);
    }
}