namespace Task1;


public static class RandomVariateGenerator
{
    private static readonly Random _rnd = new();

    public static double[] GenerateExponential(double lambda, int count)
    {
        var samples = new double[count];
        for (int i = 0; i < count; i++)
        {
            double xi = 1.0 - _rnd.NextDouble();
            samples[i] = -1.0 / lambda * Math.Log(xi);
        }
        return samples;
    }
    
    public static double[] GenerateLcgUniform(
        int count, 
        long a,
        long c,
        long seed = 123456789L)
    {
        long z = seed;
        var samples = new double[count];

        for (int i = 0; i < count; i++)
        {
            z = (a * z) % c;
            samples[i] = (double)z / c;
        }

        return samples;
    }
}