using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace IndoorNavSimulator
{
    public delegate void BeaconReceiver();

    class BeaconHandler
    {
        public BeaconHandler(List<DeviceDistance> DeviceDistances, BeaconReceiver BeaconReceiver)
        {
            deviceDistances = DeviceDistances;
            beaconReceiver = BeaconReceiver;
            beaconThreads = new List<Thread>();
            Init();
        }

        private List<DeviceDistance> deviceDistances;
        private List<Thread> beaconThreads;
        private BeaconReceiver beaconReceiver;

        private void Init()
        {
        }
    }
}
