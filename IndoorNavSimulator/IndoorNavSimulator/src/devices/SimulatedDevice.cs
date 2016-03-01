using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace IndoorNavSimulator
{
    class SimulatedDevice : DeviceDisplay
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
            label_devicepos.Foreground = red.Clone();   // Még az előtt kell klónozni, hogy az opacity-t beállítanánk!
            label_devicepos.FontSize = 9;
            background.Children.Add(label_devicepos);
            deviceDisplay.Visibility = Visibility.Hidden;
            label_devicepos.Visibility = Visibility.Hidden;
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

            label_devicepos.Content = String.Format("({0} ; {1})", Math.Round(Origo.X, 2), Math.Round(Origo.Y, 2));
            Canvas.SetLeft(label_devicepos, Origo.X + 10);
            Canvas.SetTop(label_devicepos, Origo.Y - 20);
        }

        
    }
}
