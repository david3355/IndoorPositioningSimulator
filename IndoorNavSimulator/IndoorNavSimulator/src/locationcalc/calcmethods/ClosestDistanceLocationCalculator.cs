using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    class ClosestDistanceLocationCalculator : LocationCalculator
    {
        public override LocationResult CalculateCommonPoint(List<DeviceDistance> Distances)
        {
            if (Distances.Count == 0) return new LocationResult(new Point(-1, -1), Precision.NoTag);
            if (Distances.Count == 1) return new LocationResult(Distances[0].Origo, Precision.OneTag, Distances[0].DistanceFromTag); // 1 vagy semennyi kör esetén nem tudunk pozíciót meghatározni

            if (Distances.Count == 2) // 2 kör esetén a metszet két pontja közti felezőpont kell
            {
                Intersection points = CalculateIntersection(Distances[0].Origo, Distances[1].Origo, Distances[0].DistanceFromTag, Distances[1].DistanceFromTag);
                return new LocationResult(Midpoint(points.Points[0], points.Points[1]), Precision.TwoTag, Distance(points.Points[0], points.Points[1]) / 2);
            }

            // Válasszuk ki a 3 legkisebb távolságot:
            List<DeviceDistance> leastDistances = new List<DeviceDistance>();
            for (int i = 0; i < 3; i++)
            {

            }

            List<Point> adjacentPoints = new List<Point>(); // ebben lesznek az egyes metszetek legközelebbi pontjai után kiválogatott szomszédos pontok
            List<Intersection> intersectionPoints = new List<Intersection>(); // ebben lesznek két-két kör metszéspontjai

            // kiszámoljuk minden kör, minden másik körhöz mért metszéspontját
            for (int i = 0; i < Distances.Count; i++)
            {
                for (int j = i + 1; j < Distances.Count; j++)
                {
                    if (i == j) continue;
                    intersectionPoints.Add(CalculateIntersection(Distances[i].Origo, Distances[j].Origo, Distances[i].DistanceFromTag, Distances[j].DistanceFromTag));
                }
            }
            // eddig jó
            for (int i = 0; i < intersectionPoints.Count; i++)
            {
                for (int j = i + 1; j < intersectionPoints.Count; j++)
                {
                    if (i == j) continue;
                    adjacentPoints.Add(ClosestPoint(intersectionPoints[i].Points, intersectionPoints[j].Points));
                }
            }

            return new LocationResult(PointAverage(adjacentPoints), Precision.ThreeOrMoreTag);
        }
    }
}
