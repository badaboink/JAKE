using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JAKE.client
{
    internal class PlayerVisualWrapper
    {
        public SolidColorBrush PlayerColor { get; set; }

        public PlayerVisualWrapper(SolidColorBrush playerColor)
        {
            PlayerColor = playerColor;
        }
    }
}
