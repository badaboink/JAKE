using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
using static System.Net.Mime.MediaTypeNames;

namespace JAKE.Client
{
    /// <summary>
    /// Interaction logic for ColorChoiceForm.xaml
    /// </summary>
    public partial class ColorChoiceForm : Window
    {
        public string SelectedColor { get; private set; }
        public string Name { get; private set; }

        public ColorChoiceForm()
        {
            InitializeComponent();
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateName();
            SelectedColor = "Green";
            this.Close();
        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateName();
            SelectedColor = "Blue";
            this.Close();
        }

        private void RedButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateName();
            SelectedColor = "Red";
            this.Close();
        }
        private void UpdateName()
        {
            Name = string.IsNullOrEmpty(NameTextBox.Text) ? "Unnamed" : NameTextBox.Text;
        }
    }
}
