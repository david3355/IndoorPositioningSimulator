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
    interface ContactEventHandler
    {
        void DeviceAppear(Point Origo, double Distance);
        void DeviceLeft(Point Origo);
    }

    /// <summary>
    /// Egy, az adott helymeghatározó eszköztől való távolságot reprezentáló objektum
    /// Az Origo a referenciapont (bluetooth tag) helyzete, a DistanceFromTag pedig a távolsága a telefontól
    /// </summary>
    class DeviceDistance
    {
        public DeviceDistance(Point Origo, double DistanceFromTag)
        {
            origo = Origo;
            distanceFromTag = DistanceFromTag;
        }

        private Point origo;
        private double distanceFromTag;

        public Point Origo
        {
            get { return origo; }
        }

        public double DistanceFromTag
        {
            get { return distanceFromTag; }
        }

        public void SetDistance(double NewDistance)
        {
            distanceFromTag = NewDistance;
        }

        public override bool Equals(object obj)
        {
            DeviceDistance dist = obj as DeviceDistance;
            if (dist == null) return false;
            return this.Origo.X == dist.Origo.X && this.Origo.Y == dist.Origo.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

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
            tagDiameter = 20;
        }

        private Point origo;
        private Ellipse tagDisplay, zoneDistance, deviceDistance;
        private Line radius;
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
            radius = new Line();
            label_distance = new Label();

            //deviceDistance.Visibility = Visibility.Hidden;

            tagDisplay.Stroke = blue;
            zoneDistance.Stroke = gray;
            deviceDistance.Stroke = black;
            radius.Stroke = purple;

            tagDisplay.Width = tagDiameter;
            tagDisplay.Height = tagDiameter;
            tagDisplay.Fill = blue;

            zoneDistance.Width = zoneRadius * 2;
            zoneDistance.Height = zoneRadius * 2;

            background.Children.Add(tagDisplay);
            background.Children.Add(zoneDistance);
            background.Children.Add(label_distance);
            
            label_distance.Foreground = new SolidColorBrush(Colors.Red);
            label_distance.FontSize = 9;

            Canvas.SetLeft(tagDisplay, origo.X - tagDiameter / 2);
            Canvas.SetTop(tagDisplay, origo.Y - tagDiameter / 2);
            Canvas.SetLeft(zoneDistance, origo.X - zoneRadius);
            Canvas.SetTop(zoneDistance, origo.Y - zoneRadius);
            Canvas.SetLeft(label_distance, origo.X + tagDiameter / 2);
            Canvas.SetTop(label_distance, origo.Y - tagDiameter);
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
                    background.Children.Remove(radius);
                    hasDevice = false;
                    contactEventHandler.DeviceLeft(origo);
                }
            }
            else if (!hasDevice)
            {
                background.Children.Add(deviceDistance);
                background.Children.Add(radius);
                hasDevice = true;
                contactEventHandler.DeviceAppear(origo, distanceFromTag);
            }
            Canvas.SetLeft(deviceDistance, origo.X - distanceFromTag);
            Canvas.SetTop(deviceDistance, origo.Y - distanceFromTag);
            deviceDistance.Width = distanceFromTag * 2;
            deviceDistance.Height = distanceFromTag * 2;
            radius.X1 = origo.X;
            radius.Y1 = origo.Y;
            radius.X2 = DevicePosition.X;
            radius.Y2 = DevicePosition.Y;

            return distanceFromTag;
        }

    }
}
