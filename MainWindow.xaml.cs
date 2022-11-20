using MapPlotter.Models;
using MapPlotter.Services;
using MapPlotter.ViewModels;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
        public MapViewModel DataModel
        {
            get => DataContext as MapViewModel;
        }
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void myMap_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            Point mousePosition = e.GetPosition(this);
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);

            DataModel.Latitude = pinLocation.Latitude.ToString();
            DataModel.Longitude = pinLocation.Longitude.ToString();
        }

        private void myMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataModel.IsEditMode && DataModel.EditedResidence != null)
            {
                Point mapRelativePoint = e.GetPosition(myMap);
                Location pinLocation = myMap.ViewportPointToLocation(mapRelativePoint);


                Pushpin pin = new Pushpin();
                pin.Location = pinLocation;

                DataModel.Latitude = pinLocation.Latitude.ToString();
                DataModel.Longitude = pinLocation.Longitude.ToString();

                DataModel.Pushpins.Clear();
                DataModel.Pushpins.Add(pin);

                // Modify Residence Coords
                DataModel.EditedResidence.Latitude = DataModel.Latitude;
                DataModel.EditedResidence.Longitude = DataModel.Longitude;
                DataModel.EditedResidence.IsDirty = true;
            }

            //e.Handled = true;
        }

        private void ResidencesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] != null)
            {
                Residence item = e.AddedItems[0] as Residence;
                if (item != null)
                {
                    DataModel.SelectedResidence = item;

                    CreatePushPin(item);
                }
            }          
        }

        private void CreatePushPin(Residence residence)
        {
            if (residence.HasGeo)
            {
                Pushpin pin = new Pushpin();

                double lat = Double.Parse(residence.Latitude);
                double lng = Double.Parse(residence.Longitude);
                pin.Location = new Location(lat, lng);

                DataModel.Pushpins.Clear();
                DataModel.Pushpins.Add(pin);

                myMap.Center = pin.Location;
            }
            else
            {
                DataModel.Pushpins.Clear();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DataModel.IsEditMode)
            {
                DataModel.EditResidence();
                EditButton.Content = "Cancel";
            }
            else
            {
                DataModel.CancelEditResidence();
                EditButton.Content = "Edit";

                CreatePushPin(DataModel.SelectedResidence);
            }
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataModel.IsEditMode)
            {
                EditButton.Content = "Edit";
                DataModel.SaveEditedResidence();
            }          
        }
    }
}
