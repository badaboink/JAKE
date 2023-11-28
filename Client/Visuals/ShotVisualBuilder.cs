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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ShotVisual shotVisual;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public IBuilderVisual<ShotVisual> New()
        {
            shotVisual = new ShotVisual();
            return this;
        }

        public IBuilderVisual<ShotVisual> SetColor(string color)
        {

            string[] data = color.Split(',');
            if (data[1] == "")
            {
                data[1] = "round";
            }
            if (data[0] == "")
            {
                data[0] = "blue";
            }
            SolidColorBrush solidColorBrush;
            Color shotColor = (Color)ColorConverter.ConvertFromString(data[0]);
            solidColorBrush = new SolidColorBrush(shotColor);
            shotVisual.FillColor = solidColorBrush;
            shotVisual.UpdateShot(solidColorBrush, data[1]);
            return this;
        }

        public IBuilderVisual<ShotVisual> SetName(string color)
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
