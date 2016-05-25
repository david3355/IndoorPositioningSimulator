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
    delegate void ViewOptionChange(ViewOption View);
    delegate void ValueOptionChange(int Value);

    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options(OptionChangeEventHandler Handler)
        {
            InitializeComponent();
            Init();
            eventHandler = Handler;
        }

        private OptionChangeEventHandler eventHandler;

        private void Init()
        {
            check_maximumscope.IsChecked = OptionSave.TagMaximumScopeVisible;
            check_distanceline.IsChecked = OptionSave.TagDistanceLineVisible;
            check_distancescope.IsChecked = OptionSave.TagDistanceScopeVisible;
            check_distancelabel.IsChecked = OptionSave.TagDistanceLabelVisible;
            check_devicepos.IsChecked = OptionSave.RealDevicePositionVisible;
            check_simulated_devicepos.IsChecked = OptionSave.SimulatedDevicePositionVisible;
            check_predictedDistancescope.IsChecked = OptionSave.TagPredictedDistanceScopeVisible;
            check_predictionlabel.IsChecked = OptionSave.TagPredictionLabelVisible;
            check_avgPredictedDistancescope.IsChecked = OptionSave.TagAveragePredictedDistanceScopeVisible;
            check_avgPredictionlabel.IsChecked = OptionSave.TagAveragePredictionLabelVisible;
            text_maxrange.Text = OptionSave.MaxRange.ToString();
            text_avginterval.Text = OptionSave.AveragingInterval.ToString();
            text_minbeacon.Text = OptionSave.BeaconsendingIntervalMinimum.ToString();
            text_maxbeacon.Text = OptionSave.BeaconsendingIntervalMaximum.ToString();

            e_maximumscope.Fill = BluetoothTagDisplay.lightBlue;
            e_distanceline.Fill = BluetoothTagDisplay.purple;
            e_devicepos.Fill = DeviceDisplay.green;
            e_simulated_devicepos.Fill = DeviceDisplay.red;
            e_distancescope.Fill = BluetoothTagDisplay.black;
            e_distancelabel.Fill = BluetoothTagDisplay.black;
            e_predictedDistancescope.Fill = BluetoothTagDisplay.orange;
            e_predictionlabel.Fill = BluetoothTagDisplay.orange;
            e_avgPredictedDistancescope.Fill = BluetoothTagDisplay.blue;
            e_avgPredictionlabel.Fill = BluetoothTagDisplay.blue;
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            saveMaxRange();
            saveBeaconSendingInterval();
            saveAveragingInterval();
        }

        private void ValueChangeHandler(TextBox TextBox, ValueOptionChange ChangeHandler, ref int OptionSave)
        {
            int new_value;
            if (int.TryParse(TextBox.Text, out new_value))
            {
                ChangeHandler(new_value);
                OptionSave = new_value;
            }
        }

        private void saveMaxRange()
        {
            if (eventHandler != null) ValueChangeHandler(text_maxrange, eventHandler.MaxRangeChanged, ref OptionSave.MaxRange);
        }

        private void saveBeaconSendingInterval()
        {
            if (eventHandler != null)
            {
                ValueChangeHandler(text_minbeacon, eventHandler.BeaconsendingIntervalMinimumChanged, ref OptionSave.BeaconsendingIntervalMinimum);
                ValueChangeHandler(text_maxbeacon, eventHandler.BeaconsendingIntervalMaximumChanged, ref OptionSave.BeaconsendingIntervalMaximum);
            }
        }

        private void saveAveragingInterval()
        {
            if (eventHandler != null) ValueChangeHandler(text_avginterval, eventHandler.AveragingIntervalChanged, ref OptionSave.AveragingInterval);
        }


        private void checkHandler(object sender, ViewOptionChange ChangeHandler, ref bool OptionSave)
        {
            CheckBox checkbox = sender as CheckBox;
            if (checkbox.IsChecked != null) OptionSave = (bool)checkbox.IsChecked;
            if (checkbox.IsChecked == true) ChangeHandler(ViewOption.Visible);
            else ChangeHandler(ViewOption.Invisible);
        }

        private void check_maximumscope_Change(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            checkHandler(sender, eventHandler.TagMaximumScopeVisibilityChange, ref OptionSave.TagMaximumScopeVisible);
        }

        private void check_distancescope_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            checkHandler(sender, eventHandler.TagDistanceScopeVisibilityChange, ref OptionSave.TagDistanceScopeVisible);
        }

        private void check_distanceline_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            checkHandler(sender, eventHandler.TagDistanceLineVisibilityChange, ref OptionSave.TagDistanceLineVisible);
        }

        private void check_distancelabel_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            checkHandler(sender, eventHandler.TagDistanceLabelVisibilityChange, ref OptionSave.TagDistanceLabelVisible);
        }

        private void check_devicepos_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            checkHandler(sender, eventHandler.RealDevicePositionVisibilityChange, ref OptionSave.RealDevicePositionVisible);
        }

        private void check_simulated_devicepos_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            checkHandler(sender, eventHandler.SimulatedDevicePositionVisibilityChange, ref OptionSave.SimulatedDevicePositionVisible);
        }

        private void check_predictedDistancescope_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            checkHandler(sender, eventHandler.TagPredictedDistanceScopeVisibilityChange, ref OptionSave.TagPredictedDistanceScopeVisible);
        }

        private void check_predictionlabel_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            checkHandler(sender, eventHandler.TagPredictionLabelVisibilityChange, ref OptionSave.TagPredictionLabelVisible);
        }

        private void check_avgPredictedDistancescope_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            checkHandler(sender, eventHandler.TagAveragePredictedDistanceScopeVisibilityChange, ref OptionSave.TagAveragePredictedDistanceScopeVisible);
        }

        private void check_avgPredictionlabel_Checked(object sender, RoutedEventArgs e)
        {
            if (eventHandler == null) return;
            checkHandler(sender, eventHandler.TagAveragePredictionLabelVisibilityChange, ref OptionSave.TagAveragePredictionLabelVisible);
        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                switch (e.Key)
                {
                    case Key.D0: check_maximumscope.IsChecked = !OptionSave.TagMaximumScopeVisible;
                        break;
                    case Key.D1: check_distancescope.IsChecked = !OptionSave.TagDistanceScopeVisible;
                        break;
                    case Key.D2: check_predictedDistancescope.IsChecked = !OptionSave.TagPredictedDistanceScopeVisible;
                        break;
                    case Key.D3: check_avgPredictedDistancescope.IsChecked = !OptionSave.TagAveragePredictedDistanceScopeVisible;
                        break;
                }
            }
        }

    }
}
