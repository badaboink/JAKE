using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.client.Composite
{
    public class BackgroundColorMenuItem : IMenuItem
    {
        public readonly System.Windows.Media.Color backgroundColor;

        public BackgroundColorMenuItem(System.Windows.Media.Color backgroundColor)
        {
            this.backgroundColor = backgroundColor;
        }

        public void Execute(MainWindow mainWindow)
        {
            mainWindow.SetBackgroundColor(backgroundColor);
        }
    }
}
