using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JAKE.client
{
    /// <summary>
    /// Interaction logic for HealthBoostVisual.xaml
    /// </summary>
    public partial class HealthBoostVisual : UserControl
    {
        public string HealthBoostImageSource
        {
            get { return (string)GetValue(HealthBoostImageSourceProperty); }
            set { SetValue(HealthBoostImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HealthBoostImageSource. 
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HealthBoostImageSourceProperty =
            DependencyProperty.Register("HealthBoostImageSource", typeof(string),
                                        typeof(HealthBoostVisual), new PropertyMetadata(null));

        public int HealthBoostImageHeight
        {
            get { return (int)GetValue(HealthBoostImageHeightProperty); }
            set { SetValue(HealthBoostImageHeightProperty, value); }
        }

        // Property for the HealthBoost image width
        public int HealthBoostImageWidth
        {
            get { return (int)GetValue(HealthBoostImageWidthProperty); }
            set { SetValue(HealthBoostImageWidthProperty, value); }
        }

        // Dependency properties for binding in XAML
        public static readonly DependencyProperty HealthBoostImageHeightProperty =
            DependencyProperty.Register("HealthBoostImageHeight", typeof(int), typeof(HealthBoostVisual), new PropertyMetadata(15));

        public static readonly DependencyProperty HealthBoostImageWidthProperty =
            DependencyProperty.Register("HealthBoostImageWidth", typeof(int), typeof(HealthBoostVisual), new PropertyMetadata(15));

        public HealthBoostVisual(string image, int width, int height)
        {
            InitializeComponent();
            HealthBoostImageHeight = height;
            HealthBoostImageWidth = width;
            HealthBoostImageSource = image;
        }
        
    }
}
