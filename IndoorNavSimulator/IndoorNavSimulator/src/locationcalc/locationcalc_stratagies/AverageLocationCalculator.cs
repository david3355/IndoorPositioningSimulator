using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    class AverageLocationCalculator : CalculatorStrategy
    {
        protected override LocationResult CalculateCommonPoint(List<DeviceDistance> Distances, LocationResult LastLocation)
        {
            List<Point> adjacentPoints = new List<Point>(); // ebben lesznek az egyes metszetek legközelebbi pontjai után kiválogatott szomszédos pontok
            List<Intersection> intersectionPoints = new List<Intersection>(); // ebben lesznek két-két kör metszéspontjai

            // kiszámoljuk minden kör, minden másik körhöz mért metszéspontját
            for (int i = 0; i < Distances.Count; i++)
            {
                for (int j = i + 1; j < Distances.Count; j++)
                {
                    if (i == j) continue;
                    intersectionPoints.Add(Intersection.CalculateIntersection(Distances[i].Origo, Distances[i].DistanceFromTag, Distances[j].Origo, Distances[j].DistanceFromTag));
                }
            }
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
