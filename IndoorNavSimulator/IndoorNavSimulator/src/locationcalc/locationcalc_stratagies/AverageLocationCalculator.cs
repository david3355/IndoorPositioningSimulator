using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    class AverageLocationCalculator : CalculatorStrategy
    {
        protected override LocationResult CalculateCommonPoint(List<NearbyBluetoothTag> Distances, LocationResult LastLocation)
        {
            List<Point> adjacentPoints = new List<Point>(); // ebben lesznek az egyes metszetek legközelebbi pontjai után kiválogatott szomszédos pontok

            // kiszámoljuk minden kör, minden másik körhöz mért metszéspontját
            List<Intersection> intersectionPoints = GetIntersections(Distances); // ebben lesznek két-két kör metszéspontjai
           
            // eddig jó
            for (int i = 0; i < intersectionPoints.Count; i++)
            {
                for (int j = i + 1; j < intersectionPoints.Count; j++)
                {
                    if (i == j) continue;
                    adjacentPoints.Add(LocationCalculator.ClosestPoint(intersectionPoints[i], intersectionPoints[j]));
                }
            }

            return new LocationResult(LocationCalculator.PointAverage(adjacentPoints), Precision.ThreeOrMoreTag);
        }
    }
}
