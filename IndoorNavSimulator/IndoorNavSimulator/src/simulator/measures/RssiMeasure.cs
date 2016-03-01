using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    class RssiMeasure
    {
        public RssiMeasure(int RSSI, double DistanceAverage, double DistanceStdDev)
        {
            this.Rssi = RSSI;
            this.DistanceAverage = DistanceAverage;
            this.DistanceStdDev = DistanceStdDev;
        }

        public int Rssi { get; set; }
        public double DistanceAverage { get; set; }
        public double DistanceStdDev { get; set; }
    }
}
