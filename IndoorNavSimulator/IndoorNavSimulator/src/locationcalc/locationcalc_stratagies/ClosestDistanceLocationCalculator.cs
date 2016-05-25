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

        protected CommonPointStrategy commonPointStrategy;

        /// <summary>
        /// A megadott bluetooth tag-ek távolságaiból visszaadja a k legkisebb távolságú tag-et, vagyis azok tag-eket, amelyek legközelebb vannak az eszközhöz
        /// </summary>
        /// <param name="Distances">A közeli bluetooth tag-ek, melyek tárolják az eszköztől számított közelítő távoságot</param>
        /// <param name="kLeastDistances">Az a k érték amennyi bluetooth tag-et vissza akarunk kapni, melyek a legközelebb vannak az eszköztől</param>
        /// <returns></returns>
        protected List<NearbyBluetoothTag> CalculateClosestDistances(List<NearbyBluetoothTag> Distances, int kLeastDistances)
        {
            // Válasszuk ki a k legkisebb távolságot:
            List<NearbyBluetoothTag> distancesCopy = Distances.ToList<NearbyBluetoothTag>();
            List<NearbyBluetoothTag> leastDistances = new List<NearbyBluetoothTag>();
            int mini;
            for (int j = 0; j < kLeastDistances; j++)
            {
                mini = 0;
                for (int i = 1; i < distancesCopy.Count; i++)
                {
                    if (distancesCopy[i].AveragePredictedDistanceFromTag < distancesCopy[mini].AveragePredictedDistanceFromTag) mini = i;
                }
                leastDistances.Add(distancesCopy[mini]);
                distancesCopy.RemoveAt(mini);
            }
            return leastDistances;
        }

        protected override LocationResult CalculateCommonPoint(List<NearbyBluetoothTag> Distances, LocationResult LastLocation)
        {
            List<NearbyBluetoothTag> leastDistances = CalculateClosestDistances(Distances, 3);

            List<Intersection> intersectionPoints = GetIntersections(leastDistances); // ebben lesznek két-két kör metszéspontjai

            return new LocationResult(commonPointStrategy.CommonPointOfIntersections(intersectionPoints), Precision.ThreeOrMoreTag);
        }
    }
}
