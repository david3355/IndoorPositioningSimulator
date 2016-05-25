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
            lightBlue = new SolidColorBrush(Colors.LightBlue);
            black = new SolidColorBrush(Colors.Black);
            purple = new SolidColorBrush(Colors.Purple);
            orange = new SolidColorBrush(Colors.Orange);
            tagDiameter = 10;
        }

        private Point origo;
        private String mac;

        private Ellipse tagDisplay, zoneDistance, deviceDistance, predictedDistance, avgPredictedDistance;
        private Label label_distance, label_prediction, label_avg_prediction;
        private Line line_radius;
        private double zoneRadius;
        private Canvas background;
        private MainWindow mainWindow;
        private bool hasDevice;
        private ContactEventHandler contactEventHandler;

        public static SolidColorBrush blue, lightBlue, black, purple, orange;
        private static double tagDiameter;
        private const double DEFAULTZONERAD = 500;

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
            hasDevice = false;
            tagDisplay = new Ellipse();

            zoneDistance = new Ellipse();
            line_radius = new Line();

            deviceDistance = new Ellipse();
            label_distance = new Label();

            predictedDistance = new Ellipse();
            label_prediction = new Label();

            avgPredictedDistance = new Ellipse();
            label_avg_prediction = new Label();

            zoneDistance.Visibility = GetVisibilityValue(OptionSave.TagMaximumScopeVisible);
            line_radius.Visibility = GetVisibilityValue(OptionSave.TagDistanceLineVisible);
            deviceDistance.Visibility = GetVisibilityValue(OptionSave.TagDistanceScopeVisible);
            label_distance.Visibility = GetVisibilityValue(OptionSave.TagDistanceLabelVisible);
            predictedDistance.Visibility = GetVisibilityValue(OptionSave.TagPredictedDistanceScopeVisible);
            label_prediction.Visibility = GetVisibilityValue(OptionSave.TagPredictionLabelVisible);
            avgPredictedDistance.Visibility = GetVisibilityValue(OptionSave.TagAveragePredictedDistanceScopeVisible);
            label_avg_prediction.Visibility = GetVisibilityValue(OptionSave.TagAveragePredictionLabelVisible);

            tagDisplay.Stroke = blue;
            zoneDistance.Stroke = lightBlue;
            line_radius.Stroke = purple;
            deviceDistance.Stroke = black;
            predictedDistance.Stroke = orange;
            avgPredictedDistance.Stroke = blue;

            tagDisplay.Width = tagDiameter;
            tagDisplay.Height = tagDiameter;
            tagDisplay.Fill = blue;

            zoneDistance.Width = zoneRadius * 2;
            zoneDistance.Height = zoneRadius * 2;

            label_distance.Foreground = black;
            label_distance.FontSize = 9;
            label_prediction.Foreground = orange;
            label_prediction.FontSize = 9;
            label_avg_prediction.Foreground = blue;
            label_avg_prediction.FontSize = 9;

            tagDisplay.ToolTip = mac;

            background.Children.Add(tagDisplay);
            background.Children.Add(zoneDistance);
            background.Children.Add(label_distance);

            Canvas.SetLeft(tagDisplay, origo.X - tagDiameter / 2);
            Canvas.SetTop(tagDisplay, origo.Y - tagDiameter / 2);
            Canvas.SetLeft(zoneDistance, origo.X - zoneRadius);
            Canvas.SetTop(zoneDistance, origo.Y - zoneRadius);
            Canvas.SetLeft(label_distance, origo.X + tagDiameter / 2);
            Canvas.SetTop(label_distance, origo.Y - tagDiameter);
            Canvas.SetLeft(label_prediction, origo.X + tagDiameter / 2);
            Canvas.SetTop(label_prediction, origo.Y - tagDiameter * 2);
            Canvas.SetLeft(label_avg_prediction, origo.X + tagDiameter / 2);
            Canvas.SetTop(label_avg_prediction, origo.Y + tagDiameter * 2);
            Canvas.SetZIndex(tagDisplay, 10);
        }

        public double ZoneRadius
        {
            get { return zoneRadius; }
            set
            {
                if (value > 0) zoneRadius = value;
                else throw new ArgumentException("Value must be greater than zero!");
            }
        }

        private Visibility GetVisibilityValue(bool Visible)
        {
            return Visible ? Visibility.Visible : Visibility.Hidden;
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

        public void SetAveragePredictedDistanceScopeVisibility(ViewOption View)
        {
            SetVisibility(avgPredictedDistance, View);
        }

        public void SetAveragePredictionLabelVisibility(ViewOption View)
        {
            SetVisibility(label_avg_prediction, View);
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
                    background.Children.Remove(avgPredictedDistance);
                    background.Children.Remove(label_avg_prediction);
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
                background.Children.Add(avgPredictedDistance);
                background.Children.Add(label_avg_prediction);
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

        public void BeaconSent(int RSSI, double PredictedDistance, int AvgRSSI, double PredictedAvgDistance)
        {
            mainWindow.Dispatcher.Invoke((Action)(() =>
            {
                if (predictedDistance.Visibility == Visibility.Hidden && OptionSave.TagPredictedDistanceScopeVisible == true) predictedDistance.Visibility = Visibility.Visible;
                if (label_prediction.Visibility == Visibility.Hidden && OptionSave.TagPredictionLabelVisible == true) label_prediction.Visibility = Visibility.Visible;
                Canvas.SetLeft(predictedDistance, origo.X - PredictedDistance);
                Canvas.SetTop(predictedDistance, origo.Y - PredictedDistance);
                Canvas.SetLeft(avgPredictedDistance, origo.X - PredictedAvgDistance);
                Canvas.SetTop(avgPredictedDistance, origo.Y - PredictedAvgDistance);
                predictedDistance.Width = PredictedDistance * 2;
                predictedDistance.Height = PredictedDistance * 2;
                label_prediction.Content = String.Format("{0} ({1})", Math.Round(PredictedDistance, 2), RSSI);
                avgPredictedDistance.Width = PredictedAvgDistance * 2;
                avgPredictedDistance.Height = PredictedAvgDistance * 2;
                label_avg_prediction.Content = String.Format("{0} ({1})", Math.Round(PredictedAvgDistance, 2), AvgRSSI);
            }));
        }

    }
}

