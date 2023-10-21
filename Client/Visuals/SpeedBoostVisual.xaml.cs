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
    /// Interaction logic for SpeedBoostVisual.xaml
    /// </summary>
    public partial class SpeedBoostVisual : UserControl
    {
        public string SpeedBoostImageSource
        {
            get { return (string)GetValue(SpeedBoostImageSourceProperty); }
            set { SetValue(SpeedBoostImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SpeedBoostImageSource. 
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpeedBoostImageSourceProperty =
            DependencyProperty.Register("SpeedBoostImageSource", typeof(string),
                                        typeof(SpeedBoostVisual), new PropertyMetadata(null));

        public int SpeedBoostImageHeight
        {
            get { return (int)GetValue(SpeedBoostImageHeightProperty); }
            set { SetValue(SpeedBoostImageHeightProperty, value); }
        }

        // Property for the SpeedBoost image width
        public int SpeedBoostImageWidth
        {
            get { return (int)GetValue(SpeedBoostImageWidthProperty); }
            set { SetValue(SpeedBoostImageWidthProperty, value); }
        }

        // Dependency properties for binding in XAML
        public static readonly DependencyProperty SpeedBoostImageHeightProperty =
            DependencyProperty.Register("SpeedBoostImageHeight", typeof(int), typeof(SpeedBoostVisual), new PropertyMetadata(15));

        public static readonly DependencyProperty SpeedBoostImageWidthProperty =
            DependencyProperty.Register("SpeedBoostImageWidth", typeof(int), typeof(SpeedBoostVisual), new PropertyMetadata(15));

        public SpeedBoostVisual(string image, int width, int height)
        {
            InitializeComponent();
            SpeedBoostImageHeight = height;
            SpeedBoostImageWidth = width;
            SpeedBoostImageSource = image;
        }

        public SpeedBoostVisual()
        {
            InitializeComponent();
            SpeedBoostCircle.Fill = new SolidColorBrush(Colors.LightBlue);
        }
    }
}
