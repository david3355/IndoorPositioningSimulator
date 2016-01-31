using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace IndoorNavSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ContactEventHandler, OptionChangeEventHandler
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private List<BluetoothTag> tags;
        private List<DeviceDistance> deviceDistances;
        private RealDevice realDev;
        private SimulatedDevice simDev;
        private CalculatorStrategy calculator_strategy;

        private void Init()
        {
            tags = new List<BluetoothTag>();
            deviceDistances = new List<DeviceDistance>();
            realDev = new RealDevice(backgr);
            simDev = new SimulatedDevice(backgr);
            CommonPointStrategy inspect_two_point_strategy = new InspectTwoIntersectionStrategy();
            calculator_strategy = new ClosestDistanceLocationCalculator(inspect_two_point_strategy);
            //closestdistance_strategy = new AverageLocationCalculator();   // EZ A RÉGI VERZIÓ
            SetDefaultBackground();
        }

        private void SetDefaultBackground()
        {
            Uri picture = new Uri(Environment.CurrentDirectory + @"\\deik.jpg", UriKind.Absolute);
            //Uri picture2 = new Uri(Environment.CurrentDirectory + @"\\IK_Földszint.png", UriKind.Absolute);
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(picture);
            backgr.Background = ib;
        }

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


        #region Option change handlers

        public void TagMaximumScopeVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTag tag in tags)
            {
                tag.SetMaximumDistanceScopeVisibility(View);
            }
        }

        public void TagDistanceScopeVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTag tag in tags)
            {
                tag.SetDistanceScopeVisibility(View);
            }
        }

        public void TagDistanceLineVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTag tag in tags)
            {
                tag.SetDistanceLineVisibility(View);
            }
        }

        public void TagDistanceLabelVisibilityChange(ViewOption View)
        {
            foreach (BluetoothTag tag in tags)
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

        #region Event Handlers

        private void backgr_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousepos = e.GetPosition(backgr);
            if (mousepos.X < 0 || mousepos.Y < 0) return;
            realDev.MoveDevice(mousepos);
            double d;
            DeviceDistance dd;
            foreach (BluetoothTag tag in tags)
            {
                d = tag.DeviceMotion(mousepos);
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

        private void backgr_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BluetoothTag tag = new BluetoothTag(backgr, e.GetPosition(backgr), this);
            tags.Add(tag);
        }

        private void backgr_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            tags.Clear();
            deviceDistances.Clear();
            backgr.Children.Clear();
            realDev.Display();
            simDev.Display();
        }


        private void mi_setbackground_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                Uri picture = new Uri(ofd.FileName, UriKind.Absolute);
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(picture);
                backgr.Background = ib;
            }
        }

        private void sim_settings_Click(object sender, RoutedEventArgs e)
        {
            Options opt = new Options(this);
            opt.Show();
        }
       
        #endregion


        
    }
}
