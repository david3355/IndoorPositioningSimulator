using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    class AverageClosestDistanceLocationCalculator : ClosestDistanceLocationCalculator
    {
        public AverageClosestDistanceLocationCalculator(CommonPointStrategy CommonPointStrategy)
            : base(CommonPointStrategy)
        { }

        protected override LocationResult CalculateCommonPoint(List<NearbyBluetoothTag> Distances, LocationResult LastLocation)
        {
            List<NearbyBluetoothTag> leastDistances = CalculateClosestDistances(Distances, 3);

            // TODO: javítás: kivédeni a pontatlan, torzított eseteket úgy, hogy minden esetben legyen metszéspont! 
            ForceAllIntersections(leastDistances);
            // Innen már mindenhol biztosan lesz metszéspont

            List<Intersection> intersectionPoints = GetIntersections(leastDistances); // ebben lesznek két-két kör metszéspontjai
            intersectionPoints.RemoveAll(intersect => intersect.Points == null || intersect.Points.Count == 0);

            return new LocationResult(commonPointStrategy.CommonPointOfIntersections(intersectionPoints), Precision.ThreeOrMoreTag);
        }




    }
}
