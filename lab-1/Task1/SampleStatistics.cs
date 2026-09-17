namespace Task1;

public class SampleStatistics
{
    public double[] Data { get; }
    public double Mean { get; }
    public double Variance { get; }
    public double StandardDeviation => Math.Sqrt(Variance);
    public double Min { get; }
    public double Max { get; }

    public SampleStatistics(double[] data)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        Min = data.Min();
        Max = data.Max();
        Mean = data.Average();
        Variance = data.Select(x => Math.Pow(x - Mean, 2)).Sum() / (data.Length - 1);
    }

    public List<HistogramBin> BuildHistogram(int intervalsCount)
    {
        double h = (Max - Min) / intervalsCount;
        var bins = new List<HistogramBin>();

        for (int i = 0; i < intervalsCount; i++)
        {
            double left = Min + i * h;
            double right = (i == intervalsCount - 1) ? Max : left + h;

            int count = (i == intervalsCount - 1)
                ? Data.Count(x => x >= left && x <= right)
                : Data.Count(x => x >= left && x < right);

            bins.Add(new HistogramBin(left, right, count));
        }

        return bins;
    }
}