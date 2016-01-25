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

        private void Init()
        {
            tags = new List<BluetoothTag>();
            deviceDistances = new List<DeviceDistance>();
            realDev = new RealDevice(backgr);
            simDev = new SimulatedDevice(backgr);
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

            LocationResult result = LocationCalculator.CalculateCommonPoint(deviceDistances);
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


    }
}
