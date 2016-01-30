using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace IndoorNavSimulator
{
    class RealDevice : Device
    {
        public RealDevice(Canvas Background)
            : base(Background)
        {

        }

        protected override void Init()
        {
            radius = commonRadius - 2;
            ApplyRadius();
            deviceDisplay.Fill = green;
            Canvas.SetZIndex(deviceDisplay, 4);
        }

    }

}
