using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    public delegate void BeaconHandler(string MAC, int RSSI, double RealDistanceForTest);
}
