using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;

namespace IndoorNavSimulator
{
    abstract class Device
    {
        public Device(Canvas Background)
        {
            background = Background;
            BaseInit();
        }

        static Device()
        {
            red = new SolidColorBrush(Colors.Red);
            green = new SolidColorBrush(Colors.Green);
        }

        protected Ellipse deviceDisplay;
        protected Canvas background;
        protected Point origo;
        protected double radius;
        protected const double commonRadius = 12;
        protected Label label_devicepos;
        protected ViewOption labelVisibility;

        protected static SolidColorBrush red, green;

        protected abstract void Init();

        public double Radius
        {
            get { return radius; }
        }

        private void BaseInit()
        {
            origo = new Point(100, 100);
            deviceDisplay = new Ellipse();
            label_devicepos = new Label();
            labelVisibility = ViewOption.Visible;
            Init();
            Display();
        }

        public void ApplyRadius()
        {
            deviceDisplay.Width = radius * 2;
            deviceDisplay.Height = radius * 2;
        }

        public void Display()
        {
            if (!background.Children.Contains(deviceDisplay)) background.Children.Add(deviceDisplay);
            if (!background.Children.Contains(label_devicepos)) background.Children.Add(label_devicepos);
        }

        public void Hide()
        {
            if (this.deviceDisplay.Visibility != Visibility.Hidden)
            {
                this.deviceDisplay.Visibility = Visibility.Hidden;
                this.label_devicepos.Visibility = Visibility.Hidden;
            }
        }

        public void Visible()
        {
            if (this.deviceDisplay.Visibility != Visibility.Visible)
            {
                this.deviceDisplay.Visibility = Visibility.Visible;
                if (labelVisibility == ViewOption.Visible) this.label_devicepos.Visibility = Visibility.Visible;
            }
        }

        public void SetPositionLabelVisibility(ViewOption View)
        {
            labelVisibility = View;
            switch (View)
            {
                case ViewOption.Visible:
                    label_devicepos.Visibility = Visibility.Visible;
                    break;
                case ViewOption.Invisible: label_devicepos.Visibility = Visibility.Hidden;
                    break;
            }
        }

        public void MoveDevice(Point Center)
        {
            //ide még annyit, hogy vár egy másik paramétert, amivel be lehet állítani a méretet, hogy ha 1 tag van csak, akkor az egész radius a jelölt hely
            origo = Center;
            Canvas.SetLeft(deviceDisplay, origo.X - radius);
            Canvas.SetTop(deviceDisplay, origo.Y - radius);
            AdditionalMoving(Center);
        }

        protected virtual void AdditionalMoving(Point Center)
        {
            // Hook
        }

        public void SetRadius(int Radius)
        {
            radius = Radius;
        }
    }

}
