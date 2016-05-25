using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    class OptionSave
    {
        public static bool TagMaximumScopeVisible = false;
        public static bool TagDistanceLineVisible = true;
        public static bool TagDistanceScopeVisible = false;
        public static bool TagDistanceLabelVisible = true;
        public static bool RealDevicePositionVisible = true;
        public static bool SimulatedDevicePositionVisible = true;
        public static bool TagPredictedDistanceScopeVisible = false;
        public static bool TagPredictionLabelVisible = true;
        public static bool TagAveragePredictedDistanceScopeVisible = true;
        public static bool TagAveragePredictionLabelVisible = true;

        public static int MaxRange = 1500;
        public static int BeaconsendingIntervalMinimum = 200;
        public static int BeaconsendingIntervalMaximum = 3000;
        public static int AveragingInterval = 20;
    }
}
