using MapPlotter.Data;
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

            DataModel.Latitude = (long)pinLocation.Latitude;
            DataModel.Longitude = (long)pinLocation.Longitude;
        }

        private void myMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataModel.IsEditMode && DataModel.EditedResidence != null)
            {
                Point mapRelativePoint = e.GetPosition(myMap);
                Location pinLocation = myMap.ViewportPointToLocation(mapRelativePoint);


                Pushpin pin = new Pushpin();
                pin.Location = pinLocation;

                DataModel.Latitude = pinLocation.Latitude;
                DataModel.Longitude = pinLocation.Longitude;

                DataModel.Pushpins.Clear();
                DataModel.Pushpins.Add(pin);

                // Modify Residence Coords
                var editedResidence = DataModel.EditedResidence;
                if (editedResidence.HasGeo)
                {
                    var location = editedResidence.ResidenceLocations.First();
                    location.Latitude = DataModel.Latitude;
                    location.Longitude = DataModel.Longitude;
                }
                else
                {
                    editedResidence.ResidenceLocations.Add(new ResidenceLocation
                    {
                        Latitude = DataModel.Latitude,
                        Longitude = DataModel.Longitude,
                    });
                }
            }

            //e.Handled = true;
        }

        private void ResidencesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.AddedItems[0] != null)
            {
                Residence item = e.AddedItems[0] as Residence;
                if (item != null)
                {
                    DataModel.SelectedResidence = item;

                    CreatePushPin(item);
                }
            }          
        }

        private void CreatePushPin(Residence residence, bool clearFirst = true)
        {
            if (residence.HasGeo)
            {
                Pushpin pin = new Pushpin();

                double lat = (double)residence.Location.Latitude;
                double lng = (double)residence.Location.Longitude;
                pin.Location = new Location(lat, lng);

                if (residence.IsOwnerOccupier)
                {
                    pin.Background = new SolidColorBrush(Color.FromRgb(10, 17, 114));
                }

                pin.ToolTip = new ToolTip
                {
                    Content = residence.Name
                };

                pin.Content = residence.Number;

                if (clearFirst)
                {
                    DataModel.Pushpins.Clear();
                }

                DataModel.Pushpins.Add(pin);

                myMap.Center = pin.Location;
            }
            else
            {
                if (clearFirst)
                {
                    DataModel.Pushpins.Clear();
                }
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

        private void ShowAllOnMapButton_Click(object sender, RoutedEventArgs e)
        {
            DataModel.Pushpins.Clear();

            foreach(var residence in DataModel.FilteredResidences)
            {
                CreatePushPin(residence, false);
            }

            SetMapView();
        }

        private void SetMapView()
        {
            //Margin
            var w = new Pushpin().Width;
            var h = new Pushpin().Height;
            var margin = new Thickness(w / 2, h, w / 2, 0);

            var locations = DataModel.Pushpins.Where(p => p.Location.Latitude > 0).Select(p => p.Location);

            myMap.SetView(locations, margin, 0);
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure?", "Delete Point", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DataModel.RemoveLocation().ConfigureAwait(false).GetAwaiter();
            }
        }
    }
}
