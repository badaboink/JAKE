using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using JAKE.Client;

namespace JAKE.classlibrary.Patterns
{
    public class PlayerVisualBuilder : IBuilderVisual<PlayerVisual>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private PlayerVisual playerVisual;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public IBuilderVisual<PlayerVisual> New()
        {
            playerVisual = new PlayerVisual();
            return this;
        }

        public IBuilderVisual<PlayerVisual> SetColor(string color)
        {
            Color playerColor = (Color)ColorConverter.ConvertFromString(color);
            SolidColorBrush solidColorBrush = new(playerColor);
            playerVisual.PlayerColor = solidColorBrush;
            playerVisual.UpdateColor(solidColorBrush);
            return this;
        }
        public IBuilderVisual<PlayerVisual> SetName(string color)
        {
            playerVisual.PlayerName = color;
            return this;
        }

        public IBuilderVisual<PlayerVisual> SetSize(int size)
        {
            return this;
        }

        public IBuilderVisual<PlayerVisual> SetPosition(double x, double y)
        {
            Canvas.SetLeft(playerVisual, x);
            Canvas.SetTop(playerVisual, y);
            return this;
        }

        public PlayerVisual Build()
        {
            return playerVisual;
        }
    }
}
