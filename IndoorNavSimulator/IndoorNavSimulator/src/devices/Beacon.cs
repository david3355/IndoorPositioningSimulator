using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    class Beacon
    {
        public Beacon(int RSSI, DateTime Timestamp)
        {
            this.RSSI = RSSI;
            this.Timestamp = Timestamp;
        }

        public int RSSI { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
}
