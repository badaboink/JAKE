using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using JAKE.client;
using JAKE.Client;

namespace JAKE.classlibrary.Patterns
{
    public class ShotVisualBuilder : IBuilderVisual<ShotVisual>
    {
        private ShotVisual shotVisual;

        public IBuilderVisual<ShotVisual> New()
        {
            shotVisual = new ShotVisual();
            return this;
        }

        public IBuilderVisual<ShotVisual> SetColor(string colorshape)
        {
            string[] data = colorshape.Split(',');
            SolidColorBrush solidColorBrush;
            Color shotColor = (Color)ColorConverter.ConvertFromString(data[0]);
            solidColorBrush = new SolidColorBrush(shotColor);
            shotVisual.FillColor = solidColorBrush;
            shotVisual.UpdateShot(solidColorBrush, data[1]);
            return this;
        }

        public IBuilderVisual<ShotVisual> SetName(string name)
        {
            return this;
        }
        public IBuilderVisual<ShotVisual> SetSize(int size)
        {
            shotVisual.EllipseSize = size;
            shotVisual.PolygonSize = size;
            return this;
        }
        public IBuilderVisual<ShotVisual> SetPosition(double x, double y)
        {
            Canvas.SetLeft(shotVisual, x);
            Canvas.SetTop(shotVisual, y);
            return this;
        }

        public ShotVisual Build()
        {
            return shotVisual;
        }
    }
}
