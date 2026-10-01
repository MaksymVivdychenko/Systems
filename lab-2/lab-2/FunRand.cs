using System;

namespace lab_2
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
    }
}
