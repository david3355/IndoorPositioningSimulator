using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    interface ContactEventHandler
    {
        void DeviceAppear(Point Origo, double Distance, String MAC);
        void DeviceLeft(Point Origo);
    }
}
