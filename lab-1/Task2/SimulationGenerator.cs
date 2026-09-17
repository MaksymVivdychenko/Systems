namespace Task2;

public static class SimulationGenerator
{
    private static readonly Random _rnd = new();
    
    // r = sigma * (sum(zeta_i, i=1..12) - 6) + mu
    public static double GenerateNormalTime(double mu, double sigma)
    {
        double sum = 0.0;
        for (int i = 0; i < 12; i++)
        {
            sum += _rnd.NextDouble();
        }

        return sigma * (sum - 6.0) + mu;
    }

    public static double[] GenerateBatch(double mu, double sigma, int count)
    {
        var data = new double[count];
        for (int i = 0; i < count; i++)
        {
            data[i] = GenerateNormalTime(mu, sigma);
        }
        return data;
    }
}