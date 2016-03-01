using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndoorNavSimulator
{
    class DistanceMeasureHelper
    {
        private DistanceMeasureHelper(string MeasureDataFile)
        {
            dataPath = MeasureDataFile;
            rssis = ReadData();
        }

        private string dataPath;
        private List<RssiMeasure> rssis;

        private static DistanceMeasureHelper self;

        public static DistanceMeasureHelper GetInstance
        {
            get
            {
                if (self == null) self = new DistanceMeasureHelper("rssi.txt");
                return self;
            }
        }

        /// <summary>
        /// RSSI data is ordered
        /// </summary>
        public List<RssiMeasure> ReadData()
        {
            List<RssiMeasure> measures = new List<RssiMeasure>();
            using (StreamReader str = new StreamReader(dataPath, Encoding.Default))
            {
                while (!str.EndOfStream)
                {
                    string[] tags = str.ReadLine().Split('\t');
                    int rssi = int.Parse(tags[0]);
                    double dist = double.Parse(tags[1].Replace('.', ','));
                    double stddev = double.Parse(tags[2].Replace('.', ','));
                    measures.Add(new RssiMeasure(rssi, dist, stddev));
                }
            }
            return measures;
        }

        /// <summary>
        /// Minden RSSI érték pontosan egyszer fordul elő, így lehet pontos ellenőrzést végezni
        /// </summary>
        public RssiMeasure GetRSSI(double RSSI)
        {
            foreach (RssiMeasure m in rssis)
            {
                if (m.Rssi == RSSI) return m;
            }
            if (RSSI < rssis[0].Rssi) return rssis[0];
            return rssis[rssis.Count - 1];
        }
    }
}
