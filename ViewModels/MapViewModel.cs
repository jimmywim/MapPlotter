using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapPlotter.Data;
using Microsoft.EntityFrameworkCore;
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
    internal enum LocationFilterMode
    {
        All,
        LocationSet,
        NoLocationSet
    }

    public class MapViewModel : ObservableObject
    {
        private readonly ResidencesContext context;

        private double latitude;
        public double Latitude { get => latitude; set => SetProperty(ref latitude, value); }

        private double longitude;
        public double Longitude { get => longitude; set => SetProperty(ref longitude, value); }

        public bool IsSpeculative
        {
            get
            {
                if (SelectedResidence?.Location != null)
                {
                    return SelectedResidence.IsSpeculative;
                }
                return false;
            }
            set
            {
                if (EditedResidence?.Location != null)
                {
                    EditedResidence.IsSpeculative = value;
                }

                UpdateUIForResidence();
            }
        }

        private ObservableCollection<Pushpin> pins;
        public ObservableCollection<Pushpin> Pushpins { get => pins; set => SetProperty(ref pins, value); }

        private ObservableCollection<Residence> residences;
        public ObservableCollection<Residence> Residences
        {
            get => residences;
            set
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

                var filtered = FilterResidences(filterMode);
                FilteredResidences = new ObservableCollection<Residence>(filtered);

                OnPropertyChanged(nameof(NumberInView));
            }
        }

        private LocationFilterMode filterMode;
        public bool FilterLocationsAssigned
        {
            get => filterMode == LocationFilterMode.LocationSet;
            set
            {
                filterMode = LocationFilterMode.LocationSet;
                OnPropertyChanged(nameof(FilterLocationsUnassigned));
                OnPropertyChanged(nameof(FilterLocationsAll));

                var filtered = FilterResidences(filterMode);
                FilteredResidences = new ObservableCollection<Residence>(filtered);

                OnPropertyChanged(nameof(NumberInView));
            }
        }

        public bool FilterLocationsUnassigned
        {
            get => filterMode == LocationFilterMode.NoLocationSet;
            set
            {
                filterMode = LocationFilterMode.NoLocationSet;
                OnPropertyChanged(nameof(FilterLocationsAssigned));
                OnPropertyChanged(nameof(FilterLocationsAll));

                var filtered = FilterResidences(filterMode);
                FilteredResidences = new ObservableCollection<Residence>(filtered);

                OnPropertyChanged(nameof(NumberInView));
            }
        }

        public bool FilterLocationsAll
        {
            get => filterMode == LocationFilterMode.All;
            set
            {
                filterMode = LocationFilterMode.All;
                OnPropertyChanged(nameof(FilterLocationsAssigned));
                OnPropertyChanged(nameof(FilterLocationsUnassigned));

                var filtered = FilterResidences(filterMode);
                FilteredResidences = new ObservableCollection<Residence>(filtered);

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
                UpdateUIForResidence();
            }
        }
        public bool CanEditResidence => SelectedResidence != null;

        private bool isEditMode;
        public bool IsEditMode { get => isEditMode; set => SetProperty(ref isEditMode, value); }

        private Residence? editedResidence;
        public Residence? EditedResidence
        {
            get => editedResidence; 
            set
            {
                SetProperty(ref editedResidence, value);
                UpdateUIForResidence();
            }
        }

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

            context = new ResidencesContext();
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

        public async Task RemoveLocation()
        {
            if (EditedResidence != null && EditedResidence.Location != null)
            {
                context.ResidenceLocations.Remove(EditedResidence.Location);
                await context.SaveChangesAsync();

                EditedResidence = null;
                Pushpins.Clear();
            }
        }

        public async Task SaveEditedResidence()
        {
            if (EditedResidence != null)
            {
                var res = Residences.FirstOrDefault(r => r.PrimaryKey == EditedResidence.PrimaryKey);
                if (res != null)
                {
                    if (res.Location != null)
                    {
                        res.Location.Latitude = EditedResidence.Location.Latitude;
                        res.Location.Longitude = EditedResidence.Location.Longitude;
                        res.IsSpeculative = EditedResidence.IsSpeculative;
                    }

                    EditedResidence = null;
                    IsEditMode = false;

                    OnPropertyChanged(nameof(SelectedResidence));

                    await SaveResidences();
                }
            }
        }

        public void CancelEditResidence()
        {
            EditedResidence = null;
            IsEditMode = false;
        }

        private void UpdateUIForResidence()
        {
            OnPropertyChanged(nameof(IsSpeculative));
            OnPropertyChanged(nameof(CanEditResidence));
        }

        private async Task LoadResidences()
        {
            var dbResidences = context.Residences
                .OrderBy(r => r.Address).ThenBy(r => r.Number)
                .Include(r => r.ResidenceLocations);

            // Can't parse & sort on the EF query, so do it on the hydrated result
            var sorted = dbResidences.ToList()
                .OrderBy(r => r.Address)
                .ThenBy(r => !string.IsNullOrEmpty(r.Number) ? double.Parse(r.Number) : 0);

            Residences = new ObservableCollection<Residence>(sorted);
            FilteredResidences = new ObservableCollection<Residence>(Residences);

            OnPropertyChanged(nameof(ResidencesLoaded));
            OnPropertyChanged(nameof(NumberInView));
        }

        private async Task SaveResidences()
        {
            await context.SaveChangesAsync();
        }

        private IEnumerable<Residence> FilterResidences(LocationFilterMode filterMode)
        {
            IEnumerable<Residence> residences = Residences.AsQueryable();
            switch (filterMode)
            {
                case LocationFilterMode.LocationSet:
                    residences = residences.Where(r => r.HasGeo == true);
                    break;
                case LocationFilterMode.NoLocationSet:
                    residences = residences.Where(r => r.HasGeo == false);
                    break;
            }

            if (!string.IsNullOrEmpty(FilterText))
            {
                residences = residences.Where(r => r.Name.ToLower().Contains(FilterText.ToLower())).ToList();
            }

            return residences;
        }
    }
}
