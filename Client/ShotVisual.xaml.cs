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
        public void UpdateShot(Brush newColor)
        {
            shotCircle.Fill = newColor;
        }
        public ShotVisual()
        {
            InitializeComponent();
        }
    }
}
