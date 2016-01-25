using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IndoorNavSimulator
{
    class Intersection
    {
        public Intersection(List<Point> Points)
        {
            points = Points;
        }

        private List<Point> points;

        public List<Point> Points
        {
            get { return points; }
        }
    }
}
