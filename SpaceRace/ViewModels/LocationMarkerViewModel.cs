// using CommunityToolkit.Mvvm.ComponentModel;
// using SpaceRace.Models;

// namespace SpaceRace.ViewModels;

// public partial class LocationMarkerViewModel : ViewModelBase
// {
//     public Location Location {get;}

//     [ObservableProperty]
//     bool isSelected;

//     [ObservableProperty]
//     [NotifyPropertyChangedFor(nameof(HasSpacecraft))]
//     int spacecraftCount;

//     public bool HasSpacecraft => SpacecraftCount > 0;

//     public LocationMarkerViewModel(Location location)
//     {
//         Location = location;
//     }
// }