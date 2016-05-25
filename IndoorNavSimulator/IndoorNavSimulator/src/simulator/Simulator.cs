using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace IndoorNavSimulator
{
    class Simulator : LocationEvents, ContactEventHandler, OptionChangeEventHandler
    {
        public Simulator(Canvas Background, MainWindow MainWindow)
        {
            backgr = Background;
            mainWindow = MainWindow;
            InitSimulator();
        }

        private Canvas backgr;
        private MainWindow mainWindow;

        private Map map;
        private List<BluetoothTagDisplay> tags;
        private List<NearbyBluetoothTag> nearbyTags;
        private RealDevice realDev;
        private SimulatedDevice simDev;
        private CalculatorStrategy calculator_strategy;
        //private DistanceMeasureHelper distanceMeasureHelper;
        private DistanceCalculator distcalc;

        private void InitSimulator()
        {
            //distanceMeasureHelper = DistanceMeasureHelper.GetInstance;
            distcalc = new PolyRegressionModelDistanceCalculator();
            tags = new List<BluetoothTagDisplay>();
            nearbyTags = new List<NearbyBluetoothTag>();
            realDev = new RealDevice(backgr);
            simDev = new SimulatedDevice(backgr);
            CommonPointStrategy inspect_two_point_strategy = new ClosestPointsStrategy();
            calculator_strategy = new ClosestDistanceLocationCalculator(inspect_two_point_strategy);
            //closestdistance_strategy = new AverageLocationCalculator();   // EZ A RÉGI VERZIÓ
        }

        public void SetMap(Map Map)
        {
            map = Map;
            backgr.Background = map.MapImage;
        }

        public void AddNewBluetoothTag(Point position)
        {
            String mac = MacGenerator.GenerateRandomMAC();
            BluetoothTagDisplay tag = new BluetoothTagDisplay(backgr, mainWindow, mac, position, this);
            tags.Add(tag);
        }

        public void ClearSimulator()
        {
            StopAllBeaconSending();
            tags.Clear();
            nearbyTags.Clear();
            backgr.Children.Clear();
            realDev.Display();
            simDev.Display();
            simDev.SetSimulationDisplay(Precision.NoTag, new Point(0, 0));
        }

        public void StopAllBeaconSending()
        {
            foreach (NearbyBluetoothTag tag in nearbyTags)
            {
                tag.StopSendingBeacons();
            }
        }

        #region ContactEventHandler implementation

        public void DeviceAppear(Point Origo, double Distance, String MAC)
        {
            NearbyBluetoothTag dist = new NearbyBluetoothTag(Origo, MAC, Distance, BeaconReceived);
            nearbyTags.Add(dist);
        }

        public void DeviceLeft(Point Origo)
        {
            int index = nearbyTags.FindIndex(p => p.Origo.Equals(Origo));
            nearbyTags[index].StopSendingBeacons();
            nearbyTags.RemoveAt(index);
        }

        #endregion

        #region LocationEvents implementation

        public void DeviceMove(Point position)
        {
            if (position.X < 0 || position.Y < 0) return;
            realDev.MoveDevice(position);
            double d;
            NearbyBluetoothTag dd;
            foreach (BluetoothTagDisplay tag in tags)
            {
                d = tag.DeviceMotion(position);
                dd = nearbyTags.Find(ddist => ddist.MAC.Equals(tag.MAC));
                if (dd != null) dd.SetExactDistance(d);
            }

            /*
            LocationResult result = LocationCalculator.CalculateCommonPoint(nearbyTags, null, calculator_strategy);
            DrawSimulatedPosition(result);
             * */
        }

        public void BeaconReceived(NearbyBluetoothTag Sender, int RSSI)
        {
            /*
            RssiMeasure rssi = distanceMeasureHelper.GetRSSI(RSSI);
            double distance = rssi.DistanceAverage;

            RssiMeasure avgrssi = distanceMeasureHelper.GetRSSI(AverageRSSI);
            double avgdistance = avgrssi.DistanceAverage;
            */
            double distance = distcalc.GetDistanceByRSSI(RSSI);
            double avgdistance = distcalc.GetDistanceByRSSI(Sender.AverageRSSI);
            Sender.SetAveragePredictedDistance(avgdistance);

            foreach (BluetoothTagDisplay tag in tags)
            {
                if (tag.MAC.Equals(Sender.MAC))
                {
                    tag.BeaconSent(RSSI, distance, Sender.AverageRSSI, avgdistance);
                }
            }
                        
            LocationResult location = PredictPosition();
            mainWindow.Dispatcher.Invoke((Action)(() =>
            {
                DrawSimulatedPosition(location);
            }));
        }

        private void DrawSimulatedPosition(LocationResult Position)
        {
            if (Position.Precision != Precision.NoTag)
            {
                switch (Position.Precision)
                {
                    case Precision.OneTag: simDev.SetSimulationDisplay(Precision.OneTag, Position.SimulatedLocation, Position.Radius); break;
                    case Precision.TwoTag: simDev.SetSimulationDisplay(Precision.TwoTag, Position.SimulatedLocation, Position.Radius); break;
                    case Precision.ThreeOrMoreTag: simDev.SetSimulationDisplay(Precision.ThreeOrMoreTag, Position.SimulatedLocation); break;
                }
                Point simulatedDevicePos = Position.SimulatedLocation;
                simDev.MoveDevice(simulatedDevicePos);
            }
            else simDev.SetSimulationDisplay(Precision.NoTag, new Point(-1, -1));
        }

        private LocationResult PredictPosition()
        {
            // Amíg a pozíciót számoljuk, ne lehessen megváltoztatni az average értékeket!
            lock (nearbyTags)
            {
                CommonPointStrategy cps = new ClosestPointsStrategy();  //InspectAllPointsStrategy ez mégsem jó
                CalculatorStrategy calculator = new AverageClosestDistanceLocationCalculator(cps);
                LocationResult location = calculator.CalculateLocation(nearbyTags, null);
                return location;
            }
        }

        #endregion

        #region Option change handlers

        public void TagMaximumScopeVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTagDisplay tag in tags)
            {
                tag.SetMaximumDistanceScopeVisibility(View);
            }
        }

        public void TagDistanceScopeVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTagDisplay tag in tags)
            {
                tag.SetDistanceScopeVisibility(View);
            }
        }

        public void TagDistanceLineVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTagDisplay tag in tags)
            {
                tag.SetDistanceLineVisibility(View);
            }
        }

        public void TagDistanceLabelVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTagDisplay tag in tags)
            {
                tag.SetDistanceLabelVisibility(View);
            }
        }

        public void RealDevicePositionVisibilityChange(ViewOption View)
        {
            realDev.SetPositionLabelVisibility(View);
        }

        public void SimulatedDevicePositionVisibilityChange(ViewOption View)
        {
            simDev.SetPositionLabelVisibility(View);
        }

        public void TagPredictedDistanceScopeVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTagDisplay tag in tags)
            {
                tag.SetPredictedDistanceScopeVisibility(View);
            }
        }

        public void TagPredictionLabelVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTagDisplay tag in tags)
            {
                tag.SetPredictionLabelVisibility(View);
            }
        }

        public void TagAveragePredictedDistanceScopeVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTagDisplay tag in tags)
            {
                tag.SetAveragePredictedDistanceScopeVisibility(View);
            }
        }

        public void TagAveragePredictionLabelVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTagDisplay tag in tags)
            {
                tag.SetAveragePredictionLabelVisibility(View);
            }
        }

        public void MaxRangeChanged(int NewValue)
        {
            foreach (BluetoothTagDisplay tag in tags)
            {
                tag.ZoneRadius = NewValue;
            }
        }

        public void BeaconsendingIntervalMinimumChanged(int NewValue)
        {
            foreach (NearbyBluetoothTag tag in nearbyTags)
            {
                tag.SetBeaconSendingIntervalMinimum(NewValue);
            }
        }

        public void BeaconsendingIntervalMaximumChanged(int NewValue)
        {
            foreach (NearbyBluetoothTag tag in nearbyTags)
            {
                tag.SetBeaconSendingIntervalMaximum(NewValue);
            }
        }

        public void AveragingIntervalChanged(int NewValue)
        {
            foreach (NearbyBluetoothTag tag in nearbyTags)
            {
                tag.SetAveragingInterval(NewValue);
            }
        }

        #endregion






    }
}
