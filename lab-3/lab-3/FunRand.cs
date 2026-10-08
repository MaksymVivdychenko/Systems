using System;

namespace lab_3
{
    public class FunRand
    {
        private static Random r = new Random();

        public static double Exp(double timeMean)
        {
            double a = 0;
            while (a == 0)
            {
                a = r.NextDouble();
            }
            a = -timeMean * Math.Log(a);
            return a;
        }

        public static double Unif(double timeMin, double timeMax)
        {
            double a = 0;
            while (a == 0)
            {
                a = r.NextDouble();
            }
            a = timeMin + a * (timeMax - timeMin);
            return a;
        }

        public static double Norm(double timeMean, double timeDeviation)
        {
            double u1 = 1.0 - r.NextDouble();
            double u2 = 1.0 - r.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double a = timeMean + timeDeviation * randStdNormal;
            return a;
        }

        public static double Erlang(double mean, int k)
        {
            double sum = 0;
            double lambda = (double)k / mean;
            for (int i = 0; i < k; i++)
            {
                double u = 0;
                while (u == 0) u = r.NextDouble();
                sum += -Math.Log(u) / lambda;
            }
            return sum;
        }

        public static double GetNextDouble()
        {
            return r.NextDouble();
        }
    }
}
