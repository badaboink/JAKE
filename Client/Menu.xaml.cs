using JAKE.classlibrary.Patterns;
using JAKE.client.Composite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JAKE.client
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        private readonly MainMenu mainMenu;

        public Menu(MainMenu mainMenu)
        {
            InitializeComponent();
            this.mainMenu = mainMenu;

            ColorSubMenu colorSubMenu = new ColorSubMenu();
            colorSubMenu.AddMenuItem(new BackgroundColorMenuItem(Colors.Gray));
            colorSubMenu.AddMenuItem(new BackgroundColorMenuItem(Colors.DarkGray));
            colorSubMenu.AddMenuItem(new BackgroundColorMenuItem(Colors.Black));
            colorSubMenu.AddMenuItem(new BackgroundColorMenuItem(Colors.White));
            mainMenu.AddMenuItem(colorSubMenu);

            ChatMenuItem chatMenuItem = new ChatMenuItem();
            mainMenu.AddMenuItem(chatMenuItem);
        }

        private void ChangeBackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            SubMenu.Visibility = Visibility.Visible;
        }

        private void ChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string colorName)
            {
                ExecuteColorSubMenu(new string(colorName.Where(c => !Char.IsWhiteSpace(c)).ToArray()));
            }
        }

        private void ExecuteColorSubMenu(string colorName)
        {
            System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorName);
            while (mainMenu.HasMore())
            {
                if (mainMenu.GetNext() is ColorSubMenu colorSubMenu)
                {
                    colorSubMenu.SetSelectedColor(color);
                    colorSubMenu.Execute(Application.Current.MainWindow as MainWindow);
                    break;
                }
            }
            mainMenu.ResetMenu();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SubMenu.Visibility = Visibility.Collapsed;
        }

        private void ShowChatButton_Click(object sender, RoutedEventArgs e)
        {
            while (mainMenu.HasMore())
            {
                if (mainMenu.GetNext() is ChatMenuItem chatMenuItem)
                {
                    chatMenuItem.Execute(Application.Current.MainWindow as MainWindow);
                    break;
                }
            }
            mainMenu.ResetMenu();
        }

        private void CloseMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
