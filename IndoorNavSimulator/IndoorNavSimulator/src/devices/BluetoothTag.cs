using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace IndoorNavSimulator
{
    public enum ViewOption { Visible, Invisible }

    class BluetoothTag
    {
        public BluetoothTag(Canvas Background, Point CenterPosition, ContactEventHandler EventHandler, double ZoneRadius)
        {
            zoneRadius = ZoneRadius;
            origo = CenterPosition;
            background = Background;
            contactEventHandler = EventHandler;
            Init();
        }

        public BluetoothTag(Canvas Background, Point CenterPosition, ContactEventHandler EventHandler)
            : this(Background, CenterPosition, EventHandler, DEFAULTZONERAD)
        {
        }

        static BluetoothTag()
        {
            blue = new SolidColorBrush(Colors.Blue);
            gray = new SolidColorBrush(Colors.LightBlue);
            black = new SolidColorBrush(Colors.Black);
            purple = new SolidColorBrush(Colors.Purple);
            tagDiameter = 10;
        }

        private Point origo;
        private Ellipse tagDisplay, zoneDistance, deviceDistance;
        private Line line_radius;
        private double zoneRadius;
        private Canvas background;
        private bool hasDevice;
        private ContactEventHandler contactEventHandler;
        private Label label_distance;

        private static SolidColorBrush blue, gray, black, purple;
        private static double tagDiameter;
        private const double DEFAULTZONERAD = 200;

        public Point Origo
        {
            get { return origo; }
        }

        private void Init()
        {
            hasDevice = false;
            tagDisplay = new Ellipse();
            zoneDistance = new Ellipse();
            deviceDistance = new Ellipse();
            line_radius = new Line();
            label_distance = new Label();

            //deviceDistance.Visibility = Visibility.Hidden;

            tagDisplay.Stroke = blue;
            zoneDistance.Stroke = gray;
            deviceDistance.Stroke = black;
            line_radius.Stroke = purple;

            tagDisplay.Width = tagDiameter;
            tagDisplay.Height = tagDiameter;
            tagDisplay.Fill = blue;

            zoneDistance.Width = zoneRadius * 2;
            zoneDistance.Height = zoneRadius * 2;

            background.Children.Add(tagDisplay);
            background.Children.Add(zoneDistance);
            background.Children.Add(label_distance);

            label_distance.Foreground = blue;
            label_distance.FontSize = 9;

            Canvas.SetLeft(tagDisplay, origo.X - tagDiameter / 2);
            Canvas.SetTop(tagDisplay, origo.Y - tagDiameter / 2);
            Canvas.SetLeft(zoneDistance, origo.X - zoneRadius);
            Canvas.SetTop(zoneDistance, origo.Y - zoneRadius);
            Canvas.SetLeft(label_distance, origo.X + tagDiameter / 2);
            Canvas.SetTop(label_distance, origo.Y - tagDiameter);
            Canvas.SetZIndex(tagDisplay, 0);
        }

        public void SetMaximumDistanceScopeVisibility(ViewOption View)
        {
            SetVisibility(zoneDistance, View);
        }

        public void SetDistanceScopeVisibility(ViewOption View)
        {
            SetVisibility(deviceDistance, View);
        }

        public void SetDistanceLineVisibility(ViewOption View)
        {
            SetVisibility(line_radius, View);
        }

        public void SetDistanceLabelVisibility(ViewOption View)
        {
            SetVisibility(label_distance, View);
        }

        private void SetVisibility(UIElement Element, ViewOption View)
        {
            switch (View)
            {
                case ViewOption.Visible: Element.Visibility = Visibility.Visible;
                    break;
                case ViewOption.Invisible: Element.Visibility = Visibility.Hidden;
                    break;
            }
        }

        public double DeviceMotion(Point DevicePosition)
        {
            double distanceFromTag = LocationCalculator.Distance(origo, DevicePosition);
            label_distance.Content = Math.Round(distanceFromTag, 2);
            if (distanceFromTag > zoneRadius)
            {
                if (hasDevice)
                {
                    background.Children.Remove(deviceDistance);
                    background.Children.Remove(line_radius);
                    hasDevice = false;
                    contactEventHandler.DeviceLeft(origo);
                }
            }
            else if (!hasDevice)
            {
                background.Children.Add(deviceDistance);
                background.Children.Add(line_radius);
                hasDevice = true;
                contactEventHandler.DeviceAppear(origo, distanceFromTag);
            }
            Canvas.SetLeft(deviceDistance, origo.X - distanceFromTag);
            Canvas.SetTop(deviceDistance, origo.Y - distanceFromTag);
            deviceDistance.Width = distanceFromTag * 2;
            deviceDistance.Height = distanceFromTag * 2;
            line_radius.X1 = origo.X;
            line_radius.Y1 = origo.Y;
            line_radius.X2 = DevicePosition.X;
            line_radius.Y2 = DevicePosition.Y;

            return distanceFromTag;
        }

    }
}
