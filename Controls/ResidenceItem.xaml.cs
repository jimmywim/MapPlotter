using MapPlotter.Models;
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

namespace MapPlotter.Controls
{
    /// <summary>
    /// Interaction logic for ResidenceItem.xaml
    /// </summary>
    public partial class ResidenceItem : UserControl
    {
        public Residence Residence { get; set; }

        public static DependencyProperty ResidenceProperty = DependencyProperty.Register(
            nameof(Residence),
            typeof(Residence),
            typeof(ResidenceItem),
            new PropertyMetadata(ResidenceChanged));

        public ResidenceItem()
        {
            InitializeComponent();
            DataContext = this;
        }

        private static void ResidenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as ResidenceItem;
            var newValue = e.NewValue as Residence;

            item.Residence = newValue;
        }
    }
}
