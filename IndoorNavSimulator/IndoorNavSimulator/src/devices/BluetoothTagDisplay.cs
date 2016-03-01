using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;

namespace IndoorNavSimulator
{
    public enum ViewOption { Visible, Invisible }

    class BluetoothTagDisplay
    {
        public BluetoothTagDisplay(Canvas Background, MainWindow MainWindow, String MAC, Point CenterPosition, ContactEventHandler EventHandler, double ZoneRadius)
        {
            zoneRadius = ZoneRadius;
            mac = MAC;
            origo = CenterPosition;
            background = Background;
            mainWindow = MainWindow;
            contactEventHandler = EventHandler;
            Init();
        }

        public BluetoothTagDisplay(Canvas Background, MainWindow MainWindow, String MAC, Point CenterPosition, ContactEventHandler EventHandler)
            : this(Background, MainWindow, MAC, CenterPosition, EventHandler, DEFAULTZONERAD)
        {
        }

        static BluetoothTagDisplay()
        {
            blue = new SolidColorBrush(Colors.Blue);
            gray = new SolidColorBrush(Colors.LightBlue);
            black = new SolidColorBrush(Colors.Black);
            purple = new SolidColorBrush(Colors.Purple);
            orange = new SolidColorBrush(Colors.Orange);
            tagDiameter = 10;
        }

        private Point origo;
        private String mac;

        private Ellipse tagDisplay, zoneDistance, deviceDistance, predictedDistance;
        private Line line_radius;
        private double zoneRadius;
        private Canvas background;
        private MainWindow mainWindow;
        private bool hasDevice;
        private ContactEventHandler contactEventHandler;
        private Label label_distance, label_prediction;

        private static SolidColorBrush blue, gray, black, purple, orange;
        private static double tagDiameter;
        private const double DEFAULTZONERAD = 400;

        private bool predScopeVisible, predLabelVisible;

        public Point Origo
        {
            get { return origo; }
        }

        public String MAC
        {
            get { return mac; }
        }

        private void Init()
        {
            predScopeVisible = predLabelVisible = true;
            hasDevice = false;
            tagDisplay = new Ellipse();
            zoneDistance = new Ellipse();
            deviceDistance = new Ellipse();
            predictedDistance = new Ellipse();
            line_radius = new Line();
            label_distance = new Label();
            label_prediction = new Label();

            //deviceDistance.Visibility = Visibility.Hidden;

            tagDisplay.Stroke = blue;
            zoneDistance.Stroke = gray;
            deviceDistance.Stroke = black;
            predictedDistance.Stroke = orange;
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
            label_prediction.Foreground = orange;
            label_prediction.FontSize = 9;

            tagDisplay.ToolTip = mac;

            Canvas.SetLeft(tagDisplay, origo.X - tagDiameter / 2);
            Canvas.SetTop(tagDisplay, origo.Y - tagDiameter / 2);
            Canvas.SetLeft(zoneDistance, origo.X - zoneRadius);
            Canvas.SetTop(zoneDistance, origo.Y - zoneRadius);
            Canvas.SetLeft(label_distance, origo.X + tagDiameter / 2);
            Canvas.SetTop(label_distance, origo.Y - tagDiameter);
            Canvas.SetLeft(label_prediction, origo.X + tagDiameter / 2);
            Canvas.SetTop(label_prediction, origo.Y - tagDiameter * 2);
            Canvas.SetZIndex(tagDisplay, 10);
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

        public void SetPredictionLabelVisibility(ViewOption View)
        {
            SetVisibility(label_prediction, View);
        }

        public void SetPredictedDistanceScopeVisibility(ViewOption View)
        {
            SetVisibility(predictedDistance, View);
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
                    background.Children.Remove(predictedDistance);
                    background.Children.Remove(label_prediction);
                    hasDevice = false;
                    contactEventHandler.DeviceLeft(origo);
                }
            }
            else if (!hasDevice)
            {
                background.Children.Add(deviceDistance);
                background.Children.Add(line_radius);
                background.Children.Add(predictedDistance);
                background.Children.Add(label_prediction);
                predictedDistance.Visibility = Visibility.Hidden;
                label_prediction.Visibility = Visibility.Hidden;
                hasDevice = true;
                contactEventHandler.DeviceAppear(origo, distanceFromTag, mac);
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

        public void BeaconSent(int RSSI, double PredictedDistance)
        {
            mainWindow.Dispatcher.Invoke((Action)(() =>
            {
                if (predictedDistance.Visibility == Visibility.Hidden && OptionSave.PredictedDistanceScopeVisible == true) predictedDistance.Visibility = Visibility.Visible;
                if (label_prediction.Visibility == Visibility.Hidden && OptionSave.PredictedLabelVisible == true) label_prediction.Visibility = Visibility.Visible;
                Canvas.SetLeft(predictedDistance, origo.X - PredictedDistance);
                Canvas.SetTop(predictedDistance, origo.Y - PredictedDistance);
                predictedDistance.Width = PredictedDistance * 2;
                predictedDistance.Height = PredictedDistance * 2;
                label_prediction.Content = String.Format("{0} ({1})", PredictedDistance, RSSI);
            }));
        }

    }
}

