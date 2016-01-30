using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    /// <summary>
    /// Egy, az adott helymeghatározó eszköztől való távolságot reprezentáló objektum
    /// Az Origo a referenciapont (bluetooth tag) helyzete, a DistanceFromTag pedig a távolsága a telefontól
    /// </summary>
    class DeviceDistance
    {
        public DeviceDistance(Point Origo, double DistanceFromTag)
        {
            origo = Origo;
            distanceFromTag = DistanceFromTag;
        }

        private Point origo;
        private double distanceFromTag;

        public Point Origo
        {
            get { return origo; }
        }

        public double DistanceFromTag
        {
            get { return distanceFromTag; }
        }

        public void SetDistance(double NewDistance)
        {
            distanceFromTag = NewDistance;
        }

        public override bool Equals(object obj)
        {
            DeviceDistance dist = obj as DeviceDistance;
            if (dist == null) return false;
            return this.Origo.X == dist.Origo.X && this.Origo.Y == dist.Origo.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} ({1} ; {2})", Math.Round(distanceFromTag, 4), Math.Round(Origo.X, 2), Math.Round(Origo.Y, 2));
        }

    }
}
