using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    abstract class CalculatorStrategy
    {
        public LocationResult CalculateLocation(List<NearbyBluetoothTag> Distances, LocationResult LastLocation)
        {
            if (Distances.Count == 0) return new LocationResult(new Point(-1, -1), Precision.NoTag);
            if (Distances.Count == 1) return new LocationResult(Distances[0].Origo, Precision.OneTag, Distances[0].DistanceFromTag); // 1 vagy semennyi kör esetén nem tudunk pozíciót meghatározni

            if (Distances.Count == 2) // 2 kör esetén a metszet két pontja közti felezőpont kell
            {
                Intersection points = Intersection.CalculateIntersection(Distances[0].Origo, Distances[0].DistanceFromTag, Distances[1].Origo, Distances[1].DistanceFromTag);
                return new LocationResult(LocationCalculator.Midpoint(points.Points[0], points.Points[1]), Precision.TwoTag, LocationCalculator.Distance(points.Points[0], points.Points[1]) / 2);
            }

            return CalculateCommonPoint(Distances, LastLocation);
        }

        protected abstract LocationResult CalculateCommonPoint(List<NearbyBluetoothTag> Distances, LocationResult LastLocation);
    }
}
