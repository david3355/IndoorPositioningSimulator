using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace IndoorNavSimulator
{
    class SimulatedDevice : Device
    {
        public SimulatedDevice(Canvas Background)
            : base(Background)
        {
            precision = Precision.NoTag;
        }

        private Precision precision;


        public Precision Precision
        {
            get { return precision; }
        }

        protected override void Init()
        {
            deviceDisplay.Visibility = Visibility.Hidden;
            radius = commonRadius;
            ApplyRadius();
            deviceDisplay.Stroke = red;
            deviceDisplay.StrokeThickness = 2;
            deviceDisplay.Fill = red;
            deviceDisplay.Fill.Opacity = 0.3;
            Canvas.SetZIndex(deviceDisplay, 5);
        }

        public void SetSimulationDisplay(Precision Precision, Point Origo, double Radius = -1)
        {
            if (this.precision != Precision) precision = Precision;
            this.origo = Origo;
            if (Radius < commonRadius) Radius = commonRadius;

            switch (Precision)
            {
                case Precision.NoTag:
                    Hide();
                    break;
                case Precision.OneTag:
                    Visible();
                    this.radius = Radius;
                    break;
                case Precision.TwoTag:
                    Visible();
                    this.radius = Radius;
                    break;
                case Precision.ThreeOrMoreTag:
                    Visible();
                    this.radius = commonRadius;
                    break;
            }

            ApplyRadius();
        }
    }
}
