using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    public enum Precision { NoTag, OneTag, TwoTag, ThreeOrMoreTag }

    abstract class LocationCalculator
    {
        public static double Distance(Point P0, Point P1)
        {
            return Math.Sqrt(Math.Pow(P0.X - P1.X, 2) + Math.Pow(P0.Y - P1.Y, 2));
        }

        /// <summary>
        /// A két pont közti felezőpontot számítja ki
        /// </summary>
        /// <returns>A felezőpont</returns>
        public static Point Midpoint(Point P0, Point P1)
        {
            return new Point((P0.X + P1.X) / 2, (P0.Y + P1.Y) / 2);
        }

        /// <summary>
        /// A paraméterként megadott pontok átlagát adja, vagyis az átlag középpontot (két pont esetében ez a felezőpont)
        /// </summary>
        public static Point PointAverage(List<Point> Points)
        {
            double sx = 0;
            double sy = 0;
            int n = Points.Count;
            foreach (Point p in Points)
            {
                sx += p.X;
                sy += p.Y;
            }
            return new Point(sx / n, sy / n);
        }

        /// <summary>
        /// Három, vagy több körnek számolja ki azt a pontját, amelyik ponton az összes kör metszi egymást
        /// </summary>
        /// <param name="OrigoPoints">A körök középpontjai</param>
        /// <param name="Radiuses">A körök sugarai</param>
        /// <param name="CalculatorStrategy">Különféle stratégiák megadásával különféle módokon lehet pozíciót számolni</param>
        /// <returns>Azt a pontot adja vissza, ahol az összes kör metszi egymást</returns>
        public static LocationResult CalculateCommonPoint(List<DeviceDistance> Distances, LocationResult LastLocation, CalculatorStrategy CalculatorStrategy)
        {
            return CalculatorStrategy.CalculateLocation(Distances, LastLocation);
        }

        /// <summary>
        /// Elavult, ezt használjuk: InspectTwoIntersectionStrategy -> CalculateCommonPoint
        /// Visszaadja két metszet pontjai közül a közös pontot
        /// </summary>
        public static Point ClosestPoint(Intersection Solution1, Intersection Solution2)
        {
            List<Point> intersectPoints1 = Solution1.Points;
            List<Point> intersectPoints2 = Solution2.Points;
            Point Pmin1 = intersectPoints1[0];
            Point Pmin2 = intersectPoints2[0];
            double mindist = Distance(Pmin1, Pmin2);
            double dist;
            for (int i = 0; i < intersectPoints1.Count; i++)
            {
                for (int j = 0; j < intersectPoints2.Count; j++)
                {
                    dist = Distance(intersectPoints1[i], intersectPoints2[j]);
                    if (dist < mindist)
                    {
                        Pmin1 = intersectPoints1[i];
                        Pmin2 = intersectPoints2[j];
                        mindist = dist;
                    }
                }
            }
            return Midpoint(Pmin1, Pmin2);
        }

    }
}
