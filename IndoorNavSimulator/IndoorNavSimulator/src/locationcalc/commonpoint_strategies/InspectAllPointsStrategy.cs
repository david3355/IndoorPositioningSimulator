using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    class InspectAllPointsStrategy : CommonPointStrategy
    {
        /// <summary>
        /// Minden metszet minden pontját megvizsgáljuk, és az elsőhöz viszonyítjuk.
        /// Feltesszük, hogy minden metszet tartalmaz egy pontot, amely közös a többivel, így az első is.
        /// Ezért azt keressük, hogy az első metszet első, vagy második pontja a közös pont.
        /// </summary>
        public override Point CalculateCommonPoint(List<Intersection> intersections)
        {
            double minDist = LocationCalculator.Distance(intersections[0].Points[0], intersections[1].Points[0]);
            Point commonPoint = intersections[0].Points[0];
            for (int pointIdx = 0; pointIdx < 2; pointIdx++)
            {
                Point ip1 = intersections[0].Points[pointIdx];
                for (int i = 1; i < intersections.Count; i++)
                {
                    for (int otherPointIdx = 0; otherPointIdx < 2; otherPointIdx++)
                    {
                        Point ip2 = intersections[i].Points[otherPointIdx];
                        double d = LocationCalculator.Distance(ip1, ip2);
                        if (d < minDist)
                        {
                            minDist = d;
                            commonPoint = ip1;
                            if (pointIdx == 1) return commonPoint; // Ha az első pont nem jó, és találtunk kisebb távolságot, akkor ez lesz a közös pont
                        }
                    }
                }
            }

            return commonPoint;
        }
    }
}
