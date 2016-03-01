using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndoorNavSimulator
{
    class RssiMeasureHelper
    {
        private RssiMeasureHelper(string DistanceDataFile)
        {
            dataPath = DistanceDataFile;
            distances = ReadData();
        }

        private string dataPath;
        private List<DistanceMeasure> distances;

        private static RssiMeasureHelper self;

        public static RssiMeasureHelper GetInstance
        {
            get
            {
                if (self == null) self = new RssiMeasureHelper("distances.txt");
                return self;
            }
        }

        /// <summary>
        /// Distance data is ordered
        /// </summary>
        public List<DistanceMeasure> ReadData()
        {
            double lastd = -1;
            DistanceMeasure m = null;
            List<DistanceMeasure> measures = new List<DistanceMeasure>();
            using (StreamReader str = new StreamReader(dataPath, Encoding.Default))
            {
                while (!str.EndOfStream)
                {
                    string[] tags = str.ReadLine().Split('\t');
                    double dist = double.Parse(tags[0].Replace('.', ','));
                    int rssi = int.Parse(tags[1]);
                    if (dist != lastd)
                    {
                        m = new DistanceMeasure(dist);
                        measures.Add(m);
                        lastd = dist;
                    }
                    m.AddRssiData(rssi);
                }
            }
            return measures;
        }


        public DistanceMeasure GetDistance(double Distance)
        {
            for (int i = 0; i < distances.Count -1; i++)
            {
                DistanceMeasure m = distances[i];
                DistanceMeasure m_next = distances[i+1];
                if (m.Distance <= Distance && Distance < m_next.Distance) return m;
            }
            if (Distance < distances[0].Distance) return distances[0];
            return distances[distances.Count - 1];
        }
    }
}
