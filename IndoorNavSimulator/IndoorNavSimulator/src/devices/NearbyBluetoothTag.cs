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
    class NearbyBluetoothTag
    {
        public NearbyBluetoothTag(Point Origo, String MAC, double DistanceFromTag, BeaconHandler BeaconHandler)
        {
            origo = Origo;
            mac = MAC;
            distanceFromTag = DistanceFromTag;
            sendBeacon = BeaconHandler;
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

        private static Random rnd = new Random();

        public Point Origo
        {
            get { return origo; }
        }

        public double DistanceFromTag
        {
            get { return distanceFromTag; }
        }

        public String MAC
        {
            get { return mac; }
        }

        public void SetDistance(double NewDistance)
        {
            distanceFromTag = NewDistance;
        }

        public void StopSendingBeacons()
        {
            sendingBeacons = false;
            if (beaconThread.IsAlive) beaconThread.Abort();
        }

        public void SetBeaconSendingPeriodInterval(int Minimum, int Maximum)
        {
            MINSLEEP = Minimum;
            MAXSLEEP = Maximum;
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

        public override string ToString()
        {
            return String.Format("{0} ({1} ; {2}) - {3}", Math.Round(distanceFromTag, 4), Math.Round(Origo.X, 2), Math.Round(Origo.Y, 2), mac);
        }

        private void sendBeacons()
        {
            sendingBeacons = true;
            while (sendingBeacons)
            {
                if (MINSLEEP != 0 || MAXSLEEP != 0) Thread.Sleep(rnd.Next(MINSLEEP, MAXSLEEP));
                sendBeacon(mac, getRandomRSSIByDistance(), distanceFromTag);
            }
        }

        private int getRandomRSSIByDistance()
        {
            DistanceMeasure measure = rssiMeasureHelper.GetDistance(distanceFromTag);
            double mean = measure.RssiAverage;
            double stddev = measure.RssiStandardDeviance;
            if (stddev == 0.0) return (int)mean;
            else return (int)NormalRandomGenerator.GetNormal(mean, stddev);
        }

    }
}
