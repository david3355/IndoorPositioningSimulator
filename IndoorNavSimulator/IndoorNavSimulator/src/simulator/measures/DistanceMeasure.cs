using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    class DistanceMeasure
    {
        public DistanceMeasure(double Distance)
            : this(Distance, new List<int>())
        { }

        public DistanceMeasure(double Distance, List<int> RssiData)
        {
            this.Distance = Distance;
            this.rssidata = RssiData;
        }

        public double Distance { get; set; }
        private List<int> rssidata;

        public double RssiAverage
        {
            get { return Avg(rssidata); }
        }

        public double RssiVariance
        {
            get { return Var(rssidata); }
        }

        public double RssiStandardDeviance
        {
            get { return StdDev(rssidata); }
        }

        public void AddRssiData(int RSSI)
        {
            rssidata.Add(RSSI);
        }

        private static double Avg(List<int> data)
        {
            return data.Average();
        }

        public static double Var(List<int> data)
        {
            double avg = Avg(data);
            double sum = 0;
            foreach (int d in data)
            {
                sum += Math.Pow(d - avg, 2);
            }
            return Math.Abs(sum / data.Count - 1);
        }

        public static double StdDev(List<int> data)
        {
            return Math.Sqrt(Var(data));
        }



    }
}
