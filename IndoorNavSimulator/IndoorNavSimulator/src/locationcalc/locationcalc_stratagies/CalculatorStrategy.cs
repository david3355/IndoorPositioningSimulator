﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    abstract class CalculatorStrategy
    {
        private const double BIAS = 0.1;

        public LocationResult CalculateLocation(List<NearbyBluetoothTag> NearbyDistances, LocationResult LastLocation)
        {
            List<NearbyBluetoothTag> Distances = NearbyDistances.ToList<NearbyBluetoothTag>();
            Distances.RemoveAll(tag => tag.AverageRSSI == 0.0);
            if (Distances.Count == 0) return new LocationResult(new Point(-1, -1), Precision.NoTag);
            if (Distances.Count == 1) return new LocationResult(Distances[0].Origo, Precision.OneTag, Distances[0].AveragePredictedDistanceFromTag); // 1 vagy semennyi kör esetén nem tudunk pozíciót meghatározni

            if (Distances.Count == 2) // 2 kör esetén a metszet két pontja közti felezőpont kell
            {
                ForceAllIntersections(Distances);
                Intersection points = Intersection.CalculateIntersection(Distances[0].Origo, Distances[0].AveragePredictedDistanceFromTag, Distances[1].Origo, Distances[1].AveragePredictedDistanceFromTag);
                return new LocationResult(LocationCalculator.Midpoint(points.Points[0], points.Points[1]), Precision.TwoTag, LocationCalculator.Distance(points.Points[0], points.Points[1]) / 2);
            }

            return CalculateCommonPoint(Distances, LastLocation);
        }

        protected abstract LocationResult CalculateCommonPoint(List<NearbyBluetoothTag> Distances, LocationResult LastLocation);

        /// <summary>
        /// Kiszámolja a megadott távolságoknak megfelelő körök metszéspontjait
        /// </summary>
        /// <param name="Distances">A közeli bluetooth tag-ek eszköztől mért távolságait tartalmazzák, melyek köröknek felelnek meg</param>
        /// <returns>A visszaadott Intersection lista minden eleme 2 kör metszéspontait tartalmazza</returns>
        protected List<Intersection> GetIntersections(List<NearbyBluetoothTag> Distances)
        {
            List<Intersection> intersectionPoints = new List<Intersection>();
            for (int i = 0; i < Distances.Count; i++)
            {
                for (int j = i + 1; j < Distances.Count; j++)
                {
                    if (i != j) intersectionPoints.Add(Intersection.CalculateIntersection(Distances[i].Origo, Distances[i].AveragePredictedDistanceFromTag, Distances[j].Origo, Distances[j].AveragePredictedDistanceFromTag));
                }
            }
            return intersectionPoints;
        }

        /// <summary>
        /// Ha egyszer is módosítani kellett valahol, akkor újra kell ellenőrizni az egészet, hogy mindenhol van-e metszéspont továbbra is
        /// </summary>
        protected bool IntersectionCheck(List<NearbyBluetoothTag> LeastDistances)
        {
            bool requiredModification = false;
            for (int i = 0; i < LeastDistances.Count; i++)
            {
                for (int j = 0; j < LeastDistances.Count; j++)
                {
                    if (i != j)
                    {
                        requiredModification |= IntersectionCheck(LeastDistances[i], LeastDistances[j]);
                    }
                }
            }
            return requiredModification;
        }

        protected void ForceAllIntersections(List<NearbyBluetoothTag> LeastDistances)
        {
            int checkNum = 0;
            const int MAXITERATIONS = 100;

            bool requiredModification = false;
            do
            {
                requiredModification = IntersectionCheck(LeastDistances);
                checkNum++;
            }
            while(requiredModification && checkNum < MAXITERATIONS);
        }


        /// <summary>
        /// Leellenőrzi azokat az eseteket, amikor nincs metszéspont, és beállítja a távolságokat úgy, hogy legyen
        /// </summary>
        /// <returns>
        /// true, ha módosítani kellett, mert nem volt metszéspont
        /// false, ha volt metszéspont
        /// </returns>
        protected bool IntersectionCheck(NearbyBluetoothTag t0, NearbyBluetoothTag t1)
        {
            // Első eset: a két távolság által jelzett kör messze van egymástól:
            // Megoldás: mindkettőt arányosan növeljük
            bool requiredModification = CheckFarDistances(t0, t1);

            // Második eset: az egyik kör a másikon belül helyezkedik el:
            // Megoldás: a nagyobb kört arányosan csökkentjük, a kisebb kört pedig arányosan növeljük
            if (!requiredModification) requiredModification = CheckInclude(t0, t1);
            return requiredModification;
        }

        /// <summary>
        /// Első eset: a két távolság által jelzett kör messze van egymástól:
        /// Megoldás: mindkettőt arányosan növeljük
        /// </summary>
        protected bool CheckFarDistances(NearbyBluetoothTag tag1, NearbyBluetoothTag tag2)
        {
            if (LocationCalculator.Distance(tag1.Origo, tag2.Origo) > tag1.AveragePredictedDistanceFromTag + tag2.AveragePredictedDistanceFromTag)
            {
                double deviance = LocationCalculator.Distance(tag1.Origo, tag2.Origo) - (tag1.AveragePredictedDistanceFromTag + tag2.AveragePredictedDistanceFromTag);
                deviance += BIAS;  // Az eltérés torzítása, így biztosan lesz metszéspont
                double max = tag1.AveragePredictedDistanceFromTag + tag2.AveragePredictedDistanceFromTag;
                double rate1 = tag1.AveragePredictedDistanceFromTag / max;
                double rate2 = tag2.AveragePredictedDistanceFromTag / max;
                tag1.SetAveragePredictedDistance(tag1.AveragePredictedDistanceFromTag + deviance * rate1);
                tag2.SetAveragePredictedDistance(tag2.AveragePredictedDistanceFromTag + deviance * rate2);


                //TEST
                double dist = LocationCalculator.Distance(tag1.Origo, tag2.Origo) - (tag1.AveragePredictedDistanceFromTag + tag2.AveragePredictedDistanceFromTag);
                if (dist > 0.0)
                    dist = 0; /* throw new Exception("Calculation error: distance must be 0.0"); */
                //TEST

                return true;
            }
            else return false;
        }

        /// <summary>
        /// Második eset: az egyik kör a másikon belül helyezkedik el:
        /// Megoldás: a nagyobb kört arányosan csökkentjük, a kisebb kört pedig arányosan növeljük
        /// </summary>
        protected bool CheckInclude(NearbyBluetoothTag tag1, NearbyBluetoothTag tag2)
        {
            NearbyBluetoothTag bigger, smaller;
            if (tag1.Includes(tag2))
            {
                bigger = tag1;
                smaller = tag2;
            }
            else if (tag2.Includes(tag1))
            {
                bigger = tag2;
                smaller = tag1;
            }
            else return false;

            double deviance = bigger.AveragePredictedDistanceFromTag - (LocationCalculator.Distance(bigger.Origo, smaller.Origo) + smaller.AveragePredictedDistanceFromTag);
            deviance += BIAS;   // Az eltérés torzítása, így biztosan lesz metszéspont
            double max = bigger.AveragePredictedDistanceFromTag + smaller.AveragePredictedDistanceFromTag;
            double brate = bigger.AveragePredictedDistanceFromTag / max;
            double srate = smaller.AveragePredictedDistanceFromTag / max;
            bigger.SetAveragePredictedDistance(bigger.AveragePredictedDistanceFromTag - deviance * brate);
            smaller.SetAveragePredictedDistance(smaller.AveragePredictedDistanceFromTag + deviance * srate);


            //TEST
            double dist = bigger.AveragePredictedDistanceFromTag - (LocationCalculator.Distance(bigger.Origo, smaller.Origo) + smaller.AveragePredictedDistanceFromTag);
            if (dist > 0.0)
                dist = 0; /* throw new Exception("Calculation error: distance must be 0.0"); */
            //TEST

            return true;
        }


    }
}
