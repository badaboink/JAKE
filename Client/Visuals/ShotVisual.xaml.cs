using JAKE.classlibrary;
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
    /// Interaction logic for ShotVisual.xaml
    /// </summary>
    public partial class ShotVisual : UserControl
    {
        public static readonly DependencyProperty FillColorProperty =
            DependencyProperty.Register("FillColor", typeof(Brush), typeof(ShotVisual), new PropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty EllipseSizeProperty =
            DependencyProperty.Register("EllipseSize", typeof(double), typeof(ShotVisual), new PropertyMetadata(10.0));

        public static readonly DependencyProperty PolygonSizeProperty =
            DependencyProperty.Register("PolygonSize", typeof(double), typeof(ShotVisual), new PropertyMetadata(10.0));

        public Brush FillColor
        {
            get { return (Brush)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        public double EllipseSize
        {
            get { return (double)GetValue(EllipseSizeProperty); }
            set { SetValue(EllipseSizeProperty, value); }
        }

        public double PolygonSize
        {
            get { return (double)GetValue(PolygonSizeProperty); }
            set { SetValue(PolygonSizeProperty, value); }
        }
        public void UpdateShot(Brush newColor, string shape)
        {
            if(shape == "round")
            {
                shotCircle.Fill = newColor;
                shotCircle.Visibility = Visibility.Visible;

                shotTriangle.Fill = newColor;
                shotTriangle.Visibility = Visibility.Collapsed;
            }
            else if(shape == "triangle")
            {
                shotCircle.Fill = newColor;
                shotCircle.Visibility = Visibility.Collapsed;

                shotTriangle.Fill = newColor;
                shotTriangle.Visibility = Visibility.Visible;
            }         
        }
        public ShotVisual()
        {
            InitializeComponent();
        }
    }
}
