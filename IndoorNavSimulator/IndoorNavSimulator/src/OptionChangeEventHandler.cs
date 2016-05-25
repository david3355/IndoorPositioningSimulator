using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    public interface OptionChangeEventHandler
    {
        void TagMaximumScopeVisibilityChange(ViewOption View);
        void TagDistanceLineVisibilityChange(ViewOption View);
        void TagDistanceScopeVisibilityChange(ViewOption View);
        void TagDistanceLabelVisibilityChange(ViewOption View);
        void RealDevicePositionVisibilityChange(ViewOption View);
        void SimulatedDevicePositionVisibilityChange(ViewOption View);
        void TagPredictedDistanceScopeVisibilityChange(ViewOption View);
        void TagPredictionLabelVisibilityChange(ViewOption View);
        void TagAveragePredictedDistanceScopeVisibilityChange(ViewOption View);
        void TagAveragePredictionLabelVisibilityChange(ViewOption View);

        void MaxRangeChanged(int NewValue);
        void BeaconsendingIntervalMinimumChanged(int NewValue);
        void BeaconsendingIntervalMaximumChanged(int NewValue);
        void AveragingIntervalChanged(int NewValue);
    }
}
