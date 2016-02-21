using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace IndoorNavSimulator
{
    class Map
    {
        public Map(ImageBrush MapImage, double Scale)
        {
            mapImage = MapImage;
            scale = Scale;
        }

        private ImageBrush mapImage;
        private double scale;   // A térkép tényleges hossza (méterben) és a kép hosszának aránya

        public ImageBrush MapImage
        {
            get { return mapImage; }
        }
    }
}
