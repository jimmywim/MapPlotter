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
        public ObservableCollection<Residence> Residences
        {
            get => residences; set
            {
                SetProperty(ref residences, value);
                OnPropertyChanged(nameof(FilteredResidences));
            }
        }

        public bool ResidencesLoaded => Residences.Any();

        private string filterText;
        public string FilterText
        {
            get => filterText ?? string.Empty;
            set
            {
                SetProperty(ref filterText, value);

                var filtered = FilterResidences(FilterUnassigned);
                FilteredResidences = new ObservableCollection<Residence>(filtered);

                OnPropertyChanged(nameof(NumberInView));
            }
        }

        private bool filterUnassigned;
        public bool FilterUnassigned
        {
            get => filterUnassigned;
            set
            {
                var filtered = FilterResidences(value);
                FilteredResidences = new ObservableCollection<Residence>(filtered);
                SetProperty(ref filterUnassigned, value);
                OnPropertyChanged(nameof(NumberInView));
            }
        }

        public int NumberInView => FilteredResidences.Count;

        private ObservableCollection<Residence> filteredResidences;
        public ObservableCollection<Residence> FilteredResidences { get => filteredResidences; set => SetProperty(ref filteredResidences, value); }


        private Residence selectedResidence;
        public Residence SelectedResidence
        {
            get => selectedResidence;
            set
            {
                SetProperty(ref selectedResidence, value);
                OnPropertyChanged(nameof(CanEditResidence));
            }
        }
        public bool CanEditResidence => SelectedResidence != null;

        private bool isEditMode;
        public bool IsEditMode { get => isEditMode; set => SetProperty(ref isEditMode, value); }

        private Residence? editedResidence;
        public Residence? EditedResidence { get => editedResidence; set => SetProperty(ref editedResidence, value); }

        private ICommand loadResidencesCommand;
        public ICommand LoadResidencesCommand
        {
            get
            {
                if (loadResidencesCommand == null)
                {
                    loadResidencesCommand = new AsyncRelayCommand(LoadResidences);
                }

                return loadResidencesCommand;
            }
        }

        private ICommand saveResidencesCommand;
        public ICommand SaveResidencesCommand
        {
            get
            {
                if (saveResidencesCommand == null)
                {
                    saveResidencesCommand = new AsyncRelayCommand(SaveResidences);
                }

                return saveResidencesCommand;
            }
        }

        private ICommand showAllCommand;
        public ICommand ShowAllCommand
        {
            get
            {
                if (showAllCommand == null)
                {
                    showAllCommand = new RelayCommand(ShowAll);
                }

                return showAllCommand;
            }
        }

        public MapViewModel()
        {
            Pushpins = new ObservableCollection<Pushpin>();
            Residences = new ObservableCollection<Residence>();
            FilteredResidences = new ObservableCollection<Residence>();
        }

        private void ShowAll()
        {
            FilteredResidences = new ObservableCollection<Residence>(Residences);
            OnPropertyChanged(nameof(NumberInView));
        }

        public void EditResidence()
        {
            IsEditMode = true;
            EditedResidence = SelectedResidence.Clone();
        }

        public void SaveEditedResidence()
        {
            if (EditedResidence != null)
            {
                var res = Residences.FirstOrDefault(r => r.PrimaryKey == EditedResidence.PrimaryKey);
                if (res != null)
                {
                    res.Latitude = EditedResidence.Latitude;
                    res.Longitude = EditedResidence.Longitude;
                    res.IsDirty = true;

                    EditedResidence = null;
                    IsEditMode = false;

                    OnPropertyChanged(nameof(SelectedResidence));
                }
            }
        }

        public void CancelEditResidence()
        {
            SelectedResidence.IsDirty = false;
            EditedResidence = null;
            IsEditMode = false;
        }

        private async Task LoadResidences()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            {
                if (openFile.ShowDialog() == true)
                {
                    ResidenceDataService residenceDataService = new ResidenceDataService(openFile.FileName);
                    var res = await residenceDataService.LoadResidences();
                    Residences = new ObservableCollection<Residence>(res);
                    FilteredResidences = new ObservableCollection<Residence>(res);

                    OnPropertyChanged(nameof(ResidencesLoaded));
                    OnPropertyChanged(nameof(NumberInView));
                }
            }
        }

        private async Task SaveResidences()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            if (saveFile.ShowDialog() == true)
            {
                ResidenceDataService residenceDataService = new ResidenceDataService(saveFile.FileName);
                var res = Residences.ToList();
                await residenceDataService.SaveResidences(res);

                res.ForEach(r => r.IsDirty = false);
                Residences = new ObservableCollection<Residence>(res);
            }
        }

        private List<Residence> FilterResidences(bool withLocation)
        {
            List<Residence> residences = Residences.Where(r => r.HasGeo == withLocation).ToList();
            if (!string.IsNullOrEmpty(FilterText))
            {
                residences = residences.Where(r => r.Name.ToLower().Contains(FilterText.ToLower())).ToList();
            }

            return residences;
        }
    }
}
