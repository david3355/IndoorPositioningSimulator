using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    class Intersection
    {
        public Intersection(List<Point> Points)
        {
            points = Points;
        }

        private List<Point> points;

        public List<Point> Points
        {
            get { return points; }
        }
    }

    public enum Precision { NoTag, OneTag, TwoTag, ThreeOrMoreTag }

    public class LocationResult
    {
        public LocationResult(Point SimulatedLocation, Precision Precision, double Radius)
        {
            this.SimulatedLocation = SimulatedLocation;
            this.Precision = Precision;
            this.Radius = Radius;
        }

        public LocationResult(Point SimulatedLocation, Precision Precision)
            : this(SimulatedLocation, Precision, -1)
        { }

        public LocationResult() { }

        public Point SimulatedLocation { get; set; }
        public Precision Precision { get; set; }
        public double Radius { get; set; }
    }


    class LocationCalculator
    {
        public static Intersection CalculateIntersection(Point P0, Point P1, double r0, double r1)
        {
            double d, a, h;
            Point P2, P31, P32;
            d = Distance(P0, P1);

            if (d > r0 + r1) return new Intersection(new List<Point>());    // Két külön körről van szó
            if (d < Math.Abs(r0 - r1)) return new Intersection(new List<Point>());   // Az egyik kör a másikon belül van
            if (d == 0 && r0 == r1) return new Intersection(null);      // A két kör éppen egybeesik
            //TODO: megvizsgálni, hogy két metszéspont lesz, egy, végtelen, vagy nem érintkeznek

            a = (Math.Pow(r0, 2) - Math.Pow(r1, 2) + Math.Pow(d, 2)) / (2 * d);
            h = Math.Sqrt(Math.Pow(r0, 2) - Math.Pow(a, 2));
            P2 = new Point(P0.X + a * (P1.X - P0.X) / d, P0.Y + a * (P1.Y - P0.Y) / d);

            P31 = new Point(P2.X + h * (P1.Y - P0.Y) / d, P2.Y - h * (P1.X - P0.X) / d);
            P32 = new Point(P2.X - h * (P1.Y - P0.Y) / d, P2.Y + h * (P1.X - P0.X) / d);

            List<Point> solutions = new List<Point>();
            solutions.Add(P31);
            solutions.Add(P32);
            return new Intersection(solutions);
        }

        public static double Distance(Point P0, Point P1)
        {
            return Math.Sqrt(Math.Pow(P0.X - P1.X, 2) + Math.Pow(P0.Y - P1.Y, 2));
        }

        public static Point Midpoint(Point P0, Point P1)
        {
            return new Point((P0.X + P1.X) / 2, (P0.Y + P1.Y) / 2);
        }

        private static Point PointAverage(List<Point> Points)
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
        /// <returns>Azt a pontot adja vissza, ahol az összes kör metszi egymást</returns>
        public static LocationResult CalculateCommonPoint(List<DeviceDistance> Distances)
        {
            if (Distances.Count == 0) return new LocationResult(new Point(-1, -1), Precision.NoTag);
            if (Distances.Count == 1) return new LocationResult(Distances[0].Origo, Precision.OneTag, Distances[0].DistanceFromTag); // 1 vagy semennyi kör esetén nem tudunk pozíciót meghatározni

            if (Distances.Count == 2) // 2 kör esetén a metszet két pontja közti felezőpont kell
            {
                Intersection points = CalculateIntersection(Distances[0].Origo, Distances[1].Origo, Distances[0].DistanceFromTag, Distances[1].DistanceFromTag);
                return new LocationResult(Midpoint(points.Points[0], points.Points[1]), Precision.TwoTag, Distance(points.Points[0], points.Points[1]) / 2);
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

        public static Point ClosestPoint(List<Point> Solution1, List<Point> Solution2)
        {
            Point Pmin1 = Solution1[0], Pmin2 = Solution2[0];
            double mindist = Distance(Pmin1, Pmin2);
            double dist;
            for (int i = 0; i < Solution1.Count; i++)
            {
                for (int j = 0; j < Solution2.Count; j++)
                {
                    dist = Distance(Solution1[i], Solution2[j]);
                    if (dist < mindist)
                    {
                        Pmin1 = Solution1[i];
                        Pmin2 = Solution2[j];
                        mindist = dist;
                    }
                }
            }
            return Midpoint(Pmin1, Pmin2);
        }
    }
}
