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
    /// Interaction logic for ShieldVisual.xaml
    /// </summary>
    public partial class ShieldVisual : UserControl
    {
        public string ShieldImageSource
        {
            get { return (string)GetValue(ShieldImageSourceProperty); }
            set { SetValue(ShieldImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShieldImageSource. 
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShieldImageSourceProperty =
            DependencyProperty.Register("ShieldImageSource", typeof(string),
                                        typeof(ShieldVisual), new PropertyMetadata(null));

        public int ShieldImageHeight
        {
            get { return (int)GetValue(ShieldImageHeightProperty); }
            set { SetValue(ShieldImageHeightProperty, value); }
        }

        // Property for the Shield image width
        public int ShieldImageWidth
        {
            get { return (int)GetValue(ShieldImageWidthProperty); }
            set { SetValue(ShieldImageWidthProperty, value); }
        }

        // Dependency properties for binding in XAML
        public static readonly DependencyProperty ShieldImageHeightProperty =
            DependencyProperty.Register("ShieldImageHeight", typeof(int), typeof(ShieldVisual), new PropertyMetadata(15));

        public static readonly DependencyProperty ShieldImageWidthProperty =
            DependencyProperty.Register("ShieldImageWidth", typeof(int), typeof(ShieldVisual), new PropertyMetadata(15));

        public ShieldVisual(string image, int width, int height)
        {
            InitializeComponent();
            ShieldImageHeight = height;
            ShieldImageWidth = width;
            ShieldImageSource = image;
        }

        public ShieldVisual()
        {
            InitializeComponent();
            shieldCircle.Fill = new SolidColorBrush(Colors.Black);
        }
    }
}
