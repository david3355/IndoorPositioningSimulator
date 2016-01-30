using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    public interface OptionChangeEventHandler
    {
        void TagMaximumScopeVisibilityChange(View View);
        void TagDistanceScopeVisibilityChange(View View);
        void TagDistanceLineVisibilityChange(View View);
        void TagDistanceLabelVisibilityChange(View View);
    }
}
