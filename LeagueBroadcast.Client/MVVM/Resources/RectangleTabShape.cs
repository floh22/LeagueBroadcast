using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LeagueBroadcast.Client.MVVM.Resources
{
    public class RectangleTabShape : Shape
    {
        private double _lastRenderedWidth;

        public RectangleTabShape()
        {
            Stretch = Stretch.Fill;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (constraint.Width == double.PositiveInfinity || constraint.Height == double.PositiveInfinity)
            {
                return Size.Empty;
            }
            // we will size ourselves to fit the available space
            return constraint;
        }

        protected override Geometry DefiningGeometry => GetGeometry();

        private Geometry GetGeometry()
        {
            double width = DesiredSize.Width - StrokeThickness;
            double height = 25;
            double x1 = width - 15;
            double x2 = width - 10;
            double x3 = width - 2;
            double x4 = width - 0;
            double x5 = width - 0;
            //For some reason this is needed to update the visual if the initial width is 0
            if (ActualWidth > 0 && _lastRenderedWidth == 0)
            {
                InvalidateVisual();
            }

            _lastRenderedWidth = DesiredSize.Width;
            //return new RectangleGeometry(new Rect(new Point(0, 0), new Point(width, height)), 1, 1);
            return Geometry.Parse($"M0,{height} L0,0 L{width},0 L{width},{height}");
        }
    }
}
