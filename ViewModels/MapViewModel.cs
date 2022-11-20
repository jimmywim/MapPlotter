using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapPlotter.Models;
using MapPlotter.Services;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapPlotter.ViewModels
{
    public class MapViewModel : ObservableObject
    {
        private string latitude;
        public string Latitude { get => latitude; set => SetProperty(ref latitude, value); }

        private string longitude;
        public string Longitude { get => longitude; set => SetProperty(ref longitude, value); }

        private ObservableCollection<Pushpin> pins;
        public ObservableCollection<Pushpin> Pushpins { get => pins; set => SetProperty(ref pins, value); }

        private ObservableCollection<Residence> residences;
        public ObservableCollection<Residence> Residences { get => residences; set => SetProperty(ref residences, value); }

        private ICommand loadResidencesCommand;
        public ICommand LoadResidencesCommand
        {
            get
            {
                if (loadResidencesCommand == null)
                {
                    loadResidencesCommand = new RelayCommand(LoadResidences);
                }

                return loadResidencesCommand;
            }
        }

        public MapViewModel()
        {
            Pushpins = new ObservableCollection<Pushpin>();
            Residences = new ObservableCollection<Residence>();
        }

        private void LoadResidences()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            {
                if (openFile.ShowDialog() == true)
                {
                    ResidenceDataService residenceDataService = new ResidenceDataService(openFile.FileName);
                    Residences = new ObservableCollection<Residence>(residenceDataService.LoadResidences());
                }
            }
        }
    }
}
