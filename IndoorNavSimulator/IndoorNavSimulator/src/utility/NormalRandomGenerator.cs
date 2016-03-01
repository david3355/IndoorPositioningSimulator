using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    class NormalRandomGenerator
    {
        private static Random rnd = new Random();

        public static double GetUniform()
        {
            return rnd.NextDouble();
        }

        // Get normal (Gaussian) random sample with mean 0 and standard deviation 1
        public static double GetStandardNormal()
        {
            // Use Box-Muller algorithm
            double u1 = GetUniform();
            double u2 = GetUniform();
            double r = Math.Sqrt(-2.0 * Math.Log(u1));
            double theta = 2.0 * Math.PI * u2;
            return r * Math.Sin(theta);
        }

        // Get normal (Gaussian) random sample with specified mean and standard deviation
        public static double GetNormal(double mean, double standardDeviation)
        {
            if (standardDeviation <= 0.0)
            {
                string msg = string.Format("Shape must be positive. Received {0}.", standardDeviation);
                throw new ArgumentOutOfRangeException(msg);
            }
            return mean + standardDeviation * GetStandardNormal();
        }
    }
}
