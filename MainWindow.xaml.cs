using MapPlotter.Services;
using MapPlotter.ViewModels;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Win32;
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

namespace MapPlotter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void myMap_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            Point mousePosition = e.GetPosition(this);
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);

            var vm = this.DataContext as MapViewModel;

            vm.Latitude = pinLocation.Latitude.ToString();
            vm.Longitude = pinLocation.Longitude.ToString();
        }

        private void myMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Point mousePosition = e.GetPosition(this);
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);

            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;

            var vm = this.DataContext as MapViewModel;

            vm.Latitude = pinLocation.Latitude.ToString();
            vm.Longitude = pinLocation.Longitude.ToString();

            vm.Pushpins.Add(pin);
        }
    }
}
