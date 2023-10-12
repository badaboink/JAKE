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
    /// Interaction logic for CoinVisual.xaml
    /// </summary>
    public partial class CoinVisual : UserControl
    {
        // Property for binding the Coin image source
        public string CoinImageSource
        {
            get { return (string)GetValue(CoinImageSourceProperty); }
            set { SetValue(CoinImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CoinImageSource. 
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoinImageSourceProperty =
            DependencyProperty.Register("CoinImageSource", typeof(string),
                                        typeof(CoinVisual), new PropertyMetadata(null));

        public int CoinImageHeight
        {
            get { return (int)GetValue(CoinImageHeightProperty); }
            set { SetValue(CoinImageHeightProperty, value); }
        }

        // Property for the coin image width
        public int CoinImageWidth
        {
            get { return (int)GetValue(CoinImageWidthProperty); }
            set { SetValue(CoinImageWidthProperty, value); }
        }

        // Dependency properties for binding in XAML
        public static readonly DependencyProperty CoinImageHeightProperty =
            DependencyProperty.Register("CoinImageHeight", typeof(int), typeof(CoinVisual), new PropertyMetadata(15));

        public static readonly DependencyProperty CoinImageWidthProperty =
            DependencyProperty.Register("CoinImageWidth", typeof(int), typeof(CoinVisual), new PropertyMetadata(15));

        /// <summary>
        /// ---------------------
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// 
        public SolidColorBrush CoinColor
        {
            get { return (SolidColorBrush)GetValue(CoinColorProperty); }
            set { SetValue(CoinColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CoinColor.
        public static readonly DependencyProperty CoinColorProperty =
            DependencyProperty.Register("CoinColor", typeof(SolidColorBrush), typeof(CoinVisual), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        public CoinVisual(string image, int width, int height)
        {
            InitializeComponent();
            // Set the default Coin image source path
            //CoinImageSource = image;
            //CoinImageHeight = height;
            //CoinImageWidth = width;
            CoinColor = new SolidColorBrush(Colors.Black);
        }



        public static readonly DependencyProperty FillColorPropertyC =
            DependencyProperty.Register("FillColor", typeof(Brush), typeof(EnemyVisual), new PropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty EllipseSizePropertyC =
            DependencyProperty.Register("EllipseSize", typeof(double), typeof(EnemyVisual), new PropertyMetadata(20.0));

        public Brush FillColorC
        {
            get { return (Brush)GetValue(FillColorPropertyC); }
            set { SetValue(FillColorPropertyC, value); }
        }

        public double EllipseSizeC
        {
            get { return (double)GetValue(EllipseSizePropertyC); }
            set { SetValue(EllipseSizePropertyC, value); }
        }
        public CoinVisual()
        {
            InitializeComponent();
        }
    }

}
