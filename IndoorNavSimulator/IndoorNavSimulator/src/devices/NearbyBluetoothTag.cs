using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;

namespace IndoorNavSimulator
{
    /// <summary>
    /// Egy, az adott helymeghatározó eszköztől való távolságot reprezentáló objektum
    /// Az Origo a referenciapont (bluetooth tag) helyzete, a DistanceFromTag pedig a távolsága a telefontól
    /// </summary>
    public class NearbyBluetoothTag
    {
        public NearbyBluetoothTag(Point Origo, String MAC, double DistanceFromTag, BeaconHandler BeaconHandler)
        {
            origo = Origo;
            mac = MAC;
            distanceFromTag = DistanceFromTag;
            sendBeacon = BeaconHandler;
            lastBeacons = new List<Beacon>();
            rssiMeasureHelper = RssiMeasureHelper.GetInstance;
            beaconThread = new Thread(sendBeacons);
            beaconThread.Start();
        }

        private Point origo;
        private String mac;
        private double distanceFromTag;
        private BeaconHandler sendBeacon;
        private static RssiMeasureHelper rssiMeasureHelper;

        private Thread beaconThread;
        private bool sendingBeacons;
        private int MINSLEEP = 1000;
        private int MAXSLEEP = 3000;
        private int DIFFERENCE = 20;
        private List<Beacon> lastBeacons;
        private int avgRSSI;
        private double avgPredictedDistance;

        private static Random rnd = new Random();

        public Point Origo
        {
            get { return origo; }
        }

        public double ExactDistanceFromTag
        {
            get { return distanceFromTag; }
        }

        public double AveragePredictedDistanceFromTag
        {
            get { return avgPredictedDistance; }
        }

        public int AverageRSSI
        {
            get { return avgRSSI; }
        }

        public String MAC
        {
            get { return mac; }
        }

        public void SetExactDistance(double NewDistance)
        {
            distanceFromTag = NewDistance;
        }

        public void SetAveragePredictedDistance(double NewAveragePredictedDistance)
        {
            avgPredictedDistance = NewAveragePredictedDistance;
        }

        public void StopSendingBeacons()
        {
            sendingBeacons = false;
            if (beaconThread.IsAlive) beaconThread.Abort();
        }

        public void SetBeaconSendingInterval(int Minimum, int Maximum)
        {
            MINSLEEP = Minimum;
            MAXSLEEP = Maximum;
        }

        public void SetBeaconSendingIntervalMinimum(int Minimum)
        {
            MINSLEEP = Minimum;
        }

        public void SetBeaconSendingIntervalMaximum(int Maximum)
        {
            MAXSLEEP = Maximum;
        }

        public void SetAveragingInterval(int Value)
        {
            if (Value >= 0) DIFFERENCE = Value;
            else throw new ArgumentException("Value must be greater than zero!");
        }

        /// <summary>
        /// Az egyik tag távolságából felírható kör tartalmazza-e a másik tag távolsága alapján felírható kört (average predicted distance alapján)
        /// </summary>
        public bool Includes(NearbyBluetoothTag tag)
        {
            if (this.avgPredictedDistance > LocationCalculator.Distance(this.origo, tag.origo) + tag.avgPredictedDistance) return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            NearbyBluetoothTag dist = obj as NearbyBluetoothTag;
            if (dist == null) return false;
            return this.Origo.X == dist.Origo.X && this.Origo.Y == dist.Origo.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        

        private void sendBeacons()
        {
            sendingBeacons = true;
            while (sendingBeacons)
            {
                if (MINSLEEP != 0 || MAXSLEEP != 0) Thread.Sleep(rnd.Next(MINSLEEP, MAXSLEEP));
                Beacon generatedRSSI = new Beacon(getRandomRSSIByDistance(), DateTime.Now);
                avgRSSI = GetAverageRSSI(generatedRSSI);
                sendBeacon(this, generatedRSSI.RSSI);
            }
        }

        private int GetAverageRSSI(Beacon LastRSSI)
        {
            DateTime now = DateTime.Now;
            lastBeacons.RemoveAll(beacon => (now - beacon.Timestamp).TotalSeconds > DIFFERENCE);
            lastBeacons.Add(LastRSSI);
            return GetAverageRSSI();
        }

        private int GetAverageRSSI()
        {
            avgRSSI = (int)Math.Round(lastBeacons.Average(beacon => beacon.RSSI));
            return avgRSSI;
        }

        private int getRandomRSSIByDistance()
        {
            DistanceMeasure measure = rssiMeasureHelper.GetDistance(distanceFromTag);
            double mean = measure.RssiAverage;
            double stddev = measure.RssiStandardDeviance;
            if (stddev == 0.0) return (int)mean;
            else return (int)NormalRandomGenerator.GetNormal(mean, stddev);
        }

        public override string ToString()
        {
            StringBuilder stb = new StringBuilder();
            foreach (Beacon rssi in lastBeacons)
            {
                stb.Append(rssi.RSSI + " ");
            }
            return String.Format("AvgDist: {6}, ExactDist: {0}, Origo ({1} ; {2}) - {3} > {4}  AvgRSSI: [{5}]", Math.Round(distanceFromTag, 4), Math.Round(Origo.X, 2), Math.Round(Origo.Y, 2), mac, stb.ToString(), avgRSSI, avgPredictedDistance);
        }
    }
}
