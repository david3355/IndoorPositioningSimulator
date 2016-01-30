using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    class InspectTwoIntersectionStrategy : CommonPointStrategy
    {
        /// <summary>
        /// Ez a módszer nem vizsgálja meg, csak az első két metszet pontjait. 
        /// Azon a feltevésen alapszik, hogy minden metszet két pontja közül az egyik közös.
        /// Így elég, ha két metszés megvizsgálunk, és megkeressük a közös pontot.
        /// </summary>
        public override Point CalculateCommonPoint(List<Intersection> intersections)
        {
            List<Point> mainIntersectPoints = intersections[0].Points;
            List<Point> comparsionIntersectPoints = intersections[1].Points;
            
            double mindist = LocationCalculator.Distance(mainIntersectPoints[0], comparsionIntersectPoints[0]);
            Point commonPoint = mainIntersectPoints[0];

            for (int i = 0; i < mainIntersectPoints.Count; i++)
            {
                for (int j = 0; j < comparsionIntersectPoints.Count; j++)
                {
                    double dist = LocationCalculator.Distance(mainIntersectPoints[i], comparsionIntersectPoints[j]);
                    if (dist < mindist)
                    {
                        mindist = dist;
                        commonPoint = mainIntersectPoints[i];
                    }
                }
            }

            return commonPoint;
        }
    }
}
