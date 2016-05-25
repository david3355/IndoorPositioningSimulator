using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    abstract class DistanceCalculator
    {
        public abstract double GetDistanceByRSSI(int RSSI);
    }
}
