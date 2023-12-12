using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.client.Composite
{
    public class MainMenu : IMenuItem, IIterator
    {
        protected readonly List<IMenuItem> menuItems = new List<IMenuItem>();
        private int currentIndex = 0;

        public string Name { get; set; }

        public void AddMenuItem(IMenuItem menuItem)
        {
            menuItems.Add(menuItem);
        }

        public void Execute(MainWindow mainWindow)
        {
            while (HasMore())
            {
                GetNext().Execute(mainWindow);
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
        public void ResetMenu()
        {
            currentIndex = 0;
        }
        public bool HasMore()
        {
            return currentIndex < menuItems.Count;
        }
        public void ShowMenuWindow()
        {
            Menu menuWindow = new Menu(this);
            menuWindow.Show();
        }
        public IIterator CreateIterator()
        {
            return this;
        }
    }
}
