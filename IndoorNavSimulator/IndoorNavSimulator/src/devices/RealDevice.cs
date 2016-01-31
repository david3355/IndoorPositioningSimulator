using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

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
            label_devicepos.Foreground = green;
            label_devicepos.FontSize = 9;
            background.Children.Add(label_devicepos);
            Canvas.SetZIndex(label_devicepos, 6);
        }

        protected override void AdditionalMoving(Point Center)
        {
            label_devicepos.Content = String.Format("({0};{1})", Math.Round(Center.X, 0), Math.Round(Center.Y, 0));
            Canvas.SetLeft(label_devicepos, origo.X + radius);
            Canvas.SetTop(label_devicepos, origo.Y - radius);
        }

    }

}
