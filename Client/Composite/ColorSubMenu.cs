using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace JAKE.client.Composite
{
    public class ColorSubMenu : IMenuItem, IIterator
    {
        private int currentIndex = 0;
        private readonly List<IMenuItem> menuItems = new List<IMenuItem>();
        private Color selectedColor;

        public void AddMenuItem(IMenuItem menuItem)
        {
            menuItems.Add(menuItem);
        }
        public void SetSelectedColor(Color color)
        {
            selectedColor = color;
        }
        public void Execute(MainWindow mainWindow)
        {
            while (HasMore())
            {
                var menuItem = GetNext();

                if (menuItem is BackgroundColorMenuItem backgroundColorMenuItem &&
                    backgroundColorMenuItem.backgroundColor == selectedColor)
                {
                    menuItem.Execute(mainWindow);
                    break;
                }
            }
        }

        public IMenuItem GetNext()
        {
            if (currentIndex < menuItems.Count)
            {
                return menuItems[currentIndex++];
            }
            return null;
        }

        public bool HasMore()
        {
            return currentIndex < menuItems.Count;
        }
    }
}
