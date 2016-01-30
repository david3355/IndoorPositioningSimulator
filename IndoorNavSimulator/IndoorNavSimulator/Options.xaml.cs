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
            if (checkbox.IsChecked == true) eventHandler.TagMaximumScopeVisibilityChange(View.Visible);
            else eventHandler.TagMaximumScopeVisibilityChange(View.Invisible);
        }

        private void check_distancescope_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            CheckBox checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true) eventHandler.TagDistanceScopeVisibilityChange(View.Visible);
            else eventHandler.TagDistanceScopeVisibilityChange(View.Invisible);
        }

        private void check_distanceline_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            CheckBox checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true) eventHandler.TagDistanceLineVisibilityChange(View.Visible);
            else eventHandler.TagDistanceLineVisibilityChange(View.Invisible);
        }

        private void check_distancelabel_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            CheckBox checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true) eventHandler.TagDistanceLabelVisibilityChange(View.Visible);
            else eventHandler.TagDistanceLabelVisibilityChange(View.Invisible);
        }

    }
}
