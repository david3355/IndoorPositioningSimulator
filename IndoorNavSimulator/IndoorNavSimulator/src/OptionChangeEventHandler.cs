using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    public interface OptionChangeEventHandler
    {
        void TagMaximumScopeVisibilityChange(ViewOption View);
        void TagDistanceScopeVisibilityChange(ViewOption View);
        void TagDistanceLineVisibilityChange(ViewOption View);
        void TagDistanceLabelVisibilityChange(ViewOption View);
        void RealDevicePositionVisibilityChange(ViewOption View);
        void SimulatedDevicePositionVisibilityChange(ViewOption View);
    }
}
