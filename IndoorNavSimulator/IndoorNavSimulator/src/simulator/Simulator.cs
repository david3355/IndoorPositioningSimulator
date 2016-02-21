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
        public Simulator(Canvas Background)
        {
            backgr = Background;
            InitSimulator();
        }

        private Canvas backgr;

        private Map map;
        private List<BluetoothTagDisplay> tags;
        private List<DeviceDistance> deviceDistances;
        private RealDevice realDev;
        private SimulatedDevice simDev;
        private CalculatorStrategy calculator_strategy;

        private void InitSimulator()
        {
            tags = new List<BluetoothTagDisplay>();
            deviceDistances = new List<DeviceDistance>();
            realDev = new RealDevice(backgr);
            simDev = new SimulatedDevice(backgr);
            CommonPointStrategy inspect_two_point_strategy = new InspectTwoIntersectionStrategy();
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
            BluetoothTagDisplay tag = new BluetoothTagDisplay(backgr, position, this);
            tags.Add(tag);
        }

        public void ClearSimulator()
        {
            //TODO: javítani
            tags.Clear();
            deviceDistances.Clear();
            backgr.Children.Clear();
            realDev.Display();
            simDev.Display();
            simDev.SetSimulationDisplay(Precision.NoTag, new Point(0, 0));
        }

        #region ContactEventHandler implementation

        public void DeviceAppear(Point Origo, double Distance)
        {
            DeviceDistance dist = new DeviceDistance(Origo, Distance);
            deviceDistances.Add(dist);
        }

        public void DeviceLeft(Point Origo)
        {
            int index = deviceDistances.FindIndex(p => p.Origo.Equals(Origo));
            deviceDistances.RemoveAt(index);
        }

        #endregion

        #region LocationEvents implementation

        public void DeviceMove(Point position)
        {
            if (position.X < 0 || position.Y < 0) return;
            realDev.MoveDevice(position);
            double d;
            DeviceDistance dd;
            foreach (BluetoothTagDisplay tag in tags)
            {
                d = tag.DeviceMotion(position);
                dd = deviceDistances.Find(ddist => ddist.Origo.Equals(tag.Origo));
                if (dd != null) dd.SetDistance(d);
            }


            LocationResult result = LocationCalculator.CalculateCommonPoint(deviceDistances, null, calculator_strategy);
            if (result.Precision != Precision.NoTag)
            {
                switch (result.Precision)
                {
                    case Precision.OneTag: simDev.SetSimulationDisplay(Precision.OneTag, result.SimulatedLocation, result.Radius); break;
                    case Precision.TwoTag: simDev.SetSimulationDisplay(Precision.TwoTag, result.SimulatedLocation, result.Radius); break;
                    case Precision.ThreeOrMoreTag: simDev.SetSimulationDisplay(Precision.ThreeOrMoreTag, result.SimulatedLocation); break;
                }
                Point simulatedDevicePos = result.SimulatedLocation;
                simDev.MoveDevice(simulatedDevicePos);
            }
            else simDev.SetSimulationDisplay(Precision.NoTag, new Point(-1, -1));
        }

        public void BeaconReceived()
        {
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

        #endregion
    }
}
