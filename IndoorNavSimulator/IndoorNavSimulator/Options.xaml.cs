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
using System.Windows.Shapes;

namespace IndoorNavSimulator
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options(OptionChangeEventHandler Handler)
        {
            InitializeComponent();
            eventHandler = Handler;
        }

        private OptionChangeEventHandler eventHandler;

        private void check_maximumscope_Change(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            CheckBox checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true) eventHandler.TagMaximumScopeVisibilityChange(ViewOption.Visible);
            else eventHandler.TagMaximumScopeVisibilityChange(ViewOption.Invisible);
        }

        private void check_distancescope_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            CheckBox checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true) eventHandler.TagDistanceScopeVisibilityChange(ViewOption.Visible);
            else eventHandler.TagDistanceScopeVisibilityChange(ViewOption.Invisible);
        }

        private void check_distanceline_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            CheckBox checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true) eventHandler.TagDistanceLineVisibilityChange(ViewOption.Visible);
            else eventHandler.TagDistanceLineVisibilityChange(ViewOption.Invisible);
        }

        private void check_distancelabel_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            CheckBox checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true) eventHandler.TagDistanceLabelVisibilityChange(ViewOption.Visible);
            else eventHandler.TagDistanceLabelVisibilityChange(ViewOption.Invisible);
        }

        private void check_devicepos_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            CheckBox checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true) eventHandler.RealDevicePositionVisibilityChange(ViewOption.Visible);
            else eventHandler.RealDevicePositionVisibilityChange(ViewOption.Invisible);
        }

        private void check_simulated_devicepos_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            CheckBox checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true) eventHandler.SimulatedDevicePositionVisibilityChange(ViewOption.Visible);
            else eventHandler.SimulatedDevicePositionVisibilityChange(ViewOption.Invisible);
        }

    }
}
