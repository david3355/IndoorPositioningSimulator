using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    class PolyRegressionModelDistanceCalculator : DistanceCalculator
    {
        /// <summary>
        /// The two constant values of this equation is acquired from the polynomial regression model based on the rssi measures taken from known distances
        /// </summary>
        public override double GetDistanceByRSSI(int RSSI)
        {
            return -0.00000000002457955958 * Math.Pow(RSSI, 7.0) + 0.17043942467948633000;
        }
    }
}
