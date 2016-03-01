using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    class ClosestDistanceLocationCalculator : CalculatorStrategy
    {
        public ClosestDistanceLocationCalculator(CommonPointStrategy CommonPointStrategy)
        {
            commonPointStrategy = CommonPointStrategy;
        }

        private CommonPointStrategy commonPointStrategy;

        protected override LocationResult CalculateCommonPoint(List<NearbyBluetoothTag> Distances, LocationResult LastLocation)
        {
            // Válasszuk ki a 3 legkisebb távolságot:
            List<NearbyBluetoothTag> distancesCopy = Distances.ToList<NearbyBluetoothTag>();
            List<NearbyBluetoothTag> leastDistances = new List<NearbyBluetoothTag>();
            int mini;
            for (int k = 0; k < 3; k++)
            {
                mini = 0;
                for (int i = 1; i < distancesCopy.Count; i++)
                {
                    if (distancesCopy[i].DistanceFromTag < distancesCopy[mini].DistanceFromTag) mini = i;
                }
                leastDistances.Add(distancesCopy[mini]);
                distancesCopy.RemoveAt(mini);
            }

            List<Intersection> intersectionPoints = new List<Intersection>(); // ebben lesznek két-két kör metszéspontjai

            NearbyBluetoothTag d0 = leastDistances[0];
            NearbyBluetoothTag d1 = leastDistances[1];
            NearbyBluetoothTag d2 = leastDistances[2];

            intersectionPoints.Add(Intersection.CalculateIntersection(d0.Origo, d0.DistanceFromTag, d1.Origo, d1.DistanceFromTag));
            intersectionPoints.Add(Intersection.CalculateIntersection(d0.Origo, d0.DistanceFromTag, d2.Origo, d2.DistanceFromTag));
            intersectionPoints.Add(Intersection.CalculateIntersection(d2.Origo, d2.DistanceFromTag, d1.Origo, d1.DistanceFromTag));

            return new LocationResult(commonPointStrategy.CommonPointOfIntersections(intersectionPoints), Precision.ThreeOrMoreTag);
        }
    }
}
