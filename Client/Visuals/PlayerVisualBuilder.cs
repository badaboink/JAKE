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
        private PlayerVisual playerVisual;

        public IBuilderVisual<PlayerVisual> New()
        {
            playerVisual = new PlayerVisual();
            return this;
        }

        public IBuilderVisual<PlayerVisual> SetColor(string color)
        {
            Color playerColor = (Color)ColorConverter.ConvertFromString(color);
            SolidColorBrush solidColorBrush = new SolidColorBrush(playerColor);
            playerVisual.PlayerColor = solidColorBrush;
            playerVisual.UpdateColor(solidColorBrush);
            return this;
        }

        public IBuilderVisual<PlayerVisual> SetName(string name)
        {
            playerVisual.PlayerName = name;
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
