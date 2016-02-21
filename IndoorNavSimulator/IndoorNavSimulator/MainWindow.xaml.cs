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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private Simulator simulator;

        private void Init()
        {
            simulator = new Simulator(backgr);
            SetDefaultBackground();
        }

        private void SetDefaultBackground()
        {
            Uri picture = new Uri(Environment.CurrentDirectory + @"\\deik.jpg", UriKind.Absolute);
            simulator.SetMap(GetMapByURI(picture));
        }

        private Map GetMapByURI(Uri URI)
        {
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(URI);
            return new Map(ib, 1);
        }

        #region Event Handlers

        private void backgr_MouseMove(object sender, MouseEventArgs e)
        {
            simulator.DeviceMove(e.GetPosition(backgr));
        }

        private void backgr_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            simulator.AddNewBluetoothTag(e.GetPosition(backgr));
        }

        private void backgr_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            simulator.ClearSimulator();
        }

        private void mi_setbackground_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                Uri picture = new Uri(ofd.FileName, UriKind.Absolute);
                simulator.SetMap(GetMapByURI(picture));
            }
        }

        private void sim_settings_Click(object sender, RoutedEventArgs e)
        {
            Options opt = new Options(simulator);
            opt.Show();
        }
       
        #endregion

    }
}
