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
    public partial class MainWindow : Window, ContactEventHandler
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

        /// <summary>
        /// TESZT
        /// </summary>
        private void Test2()
        {
            Point p1 = new Point(4, 4);
            Point p2 = new Point(7, 7);
            Point p3 = new Point(4,10);
            double r1, r2, r3;
            r1 = r2 = r3 = 3;
            Intersection i1 = new Intersection(p1, r1, p2, r2);
            Intersection i2 = new Intersection(p1, r1, p3, r3);
            Intersection i3 = new Intersection(p3, r3, p2, r2);
            Intersection[] intersections = { i1, i2, i3 };
            CommonPointStrategy commonPointStrategy = new InspectTwoIntersectionStrategy();
            LocationResult lr = new LocationResult(commonPointStrategy.CommonPointOfIntersections(new List<Intersection>(intersections)), Precision.ThreeOrMoreTag);
        }

        private void Test()
        {
            Point p1 = new Point(13, 9);
            Point p2 = new Point(27, 8);
            Point p3 = new Point(28, 22);
            double r1, r2, r3;
            r1 = 9.8;
            r2 = 7;
            r3 = 10.8;
            Intersection i1 = new Intersection(p1, r1, p2, r2);
            Intersection i2 = new Intersection(p1, r1, p3, r3);
            Intersection i3 = new Intersection(p3, r3, p2, r2);
            i3 = null;
            Intersection[] intersections = { i1, i2, i3 };
            CommonPointStrategy commonPointStrategy = new InspectTwoIntersectionStrategy();
            Point p = commonPointStrategy.CommonPointOfIntersections(new List<Intersection>(intersections));
            MessageBox.Show(p.ToString());
        }

        private void Init()
        {
            tags = new List<BluetoothTag>();
            deviceDistances = new List<DeviceDistance>();
            realDev = new RealDevice(backgr);
            simDev = new SimulatedDevice(backgr);
            CommonPointStrategy inspect_two_point_strategy = new InspectTwoIntersectionStrategy();
            calculator_strategy = new ClosestDistanceLocationCalculator(inspect_two_point_strategy);
            //closestdistance_strategy = new AverageLocationCalculator();   // EZ A RÉGI VERZIÓ, AMI JÓL MŰKÖDIK
            SetDefaultBackground();
        }

        private void SetDefaultBackground()
        {
            Uri picture = new Uri(Environment.CurrentDirectory + @"\\deik.jpg", UriKind.Absolute);
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(picture);
            backgr.Background = ib;
        }

        #region EventHandlers

        private void backgr_MouseMove(object sender, MouseEventArgs e)
        {
            realDev.MoveDevice(e.GetPosition(backgr));
            double d;
            DeviceDistance dd;
            foreach (BluetoothTag tag in tags)
            {
                d = tag.DeviceMotion(e.GetPosition(backgr));
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

        #endregion

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

        private void backgr_KeyDown(object sender, KeyEventArgs e)
        {
            MouseEventArgs me = new MouseEventArgs(Mouse.PrimaryDevice, 10000);
            backgr_MouseMove(this, me);
        }


    }
}
