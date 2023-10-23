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

namespace JAKE.client.Visuals
{
    /// <summary>
    /// Interaction logic for ZombiesVisual.xaml
    /// </summary>
    public partial class ZombiesVisual : UserControl
    {
        //public ZombiesVisual(string name, int size)
        //{
        //    InitializeComponent();
        //    zombieRectangle.Fill = new SolidColorBrush(Colors.Green);
        //    ZombieName = name;
        //    ZombieSize = size;
        //}

        public ZombiesVisual()
        {
            InitializeComponent();
        }

        public string ZombieName
        {
            get { return (string)GetValue(ZombieNameProperty); }
            set { SetValue(ZombieNameProperty, value); }
        }
        public static readonly DependencyProperty ZombieNameProperty =
          DependencyProperty.Register("ZombieName", typeof(string), typeof(ZombiesVisual), new PropertyMetadata("BOSAS"));

        public int ZombieSize
        {
            get { return (int)GetValue(ZombieSizeProperty); }
            set { SetValue(ZombieSizeProperty, value); }
        }
        public static readonly DependencyProperty ZombieSizeProperty =
          DependencyProperty.Register("ZombieSize", typeof(int), typeof(ZombiesVisual), new PropertyMetadata(70));

    }
}
