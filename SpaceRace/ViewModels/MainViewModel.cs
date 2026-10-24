using CommunityToolkit.Mvvm.ComponentModel;
using SpaceRace.Models;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System;
using Avalonia.Controls.Selection;
using System.Linq;


namespace SpaceRace.ViewModels;

// public record Country(string Name, string Region, int Population);

public partial class MainViewModel : ViewModelBase
{
    public GameSession Session{get;} = new();

    public Spacecraft? SelectedSpacecraft{get;set;} = null;

    // [ObservableProperty]
    // public partial string Greeting { get; set; } = "Welcome to Avalonia!";

    // public Dictionary<Location, LocationMarkerViewModel> LocationMarkers{get;}


    public MainViewModel()
    {
        
    }

    public MainViewModel(GameSession session)
    {
        Session = session;

        // LocationMarkers = Session.State.Locations.ToDictionary(
        //     location => location,
        //     location => new LocationMarkerViewModel(location)
        // );

        // RefreshLocationMarkers();
    }

    // public ObservableCollection<Country> Countries { get; } =
    // [
    //     new("Afghanistan", "Asia", 31056997),
    //     new("Albania", "Eastern Europe", 3581655),
    //     new("Algeria", "Northern Africa", 32930091),
    // ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LocationDescription))]
    Location? selectedLocation;

    public void SelectLocation(Location location)
    {
        Console.WriteLine($"Select Location: {location}");

        SelectedLocation = location;
    }

    public string LocationDescription
    {
        get
        {
            if(SelectedLocation == null)
                return "";
            var lines = new List<string>(){SelectedLocation.Name};
            if(Session.State.SourceToDestinationToRoute.TryGetValue(SelectedLocation, out var destToRoute))
            {
                foreach((var dest, var route) in destToRoute)
                {
                    var words = new List<string>(){
                        $"-> {dest.Name}",
                        $"Difficulty: {route.Difficulty}"
                    };
                    if(route.Years > 0)
                    {
                        words.Add($"Years: {route.Years}");
                    }
                    if(route.IsLanding)
                    {
                        words.Add("Landing");
                    }
                    if(route.IsReEntry)
                    {
                        words.Add("Re-Entry");
                    }
                    if(route.HasAstronautIncapacitedCheck)
                    {
                        words.Add("Astronaut Incapacited check");
                    }
                    lines.Add(string.Join(", ", words));
                }
            }
            return string.Join("\n", lines);
        }
    }

    [RelayCommand]
    void OpenResearchDialog()
    {
        Console.WriteLine("OpenResearchDialog");

        IsResearchDialogOpen = true;
    }

    [RelayCommand]
    void OpenInventoryDialog()
    {
        Console.WriteLine("OpenInventoryDialog");

        IsInventoryDialogOpen = true;
    }

    [ObservableProperty]
    bool isResearchDialogOpen;

    [ObservableProperty]
    bool isInventoryDialogOpen;

    [RelayCommand]
    void CloseResearchDialog()
    {
        IsResearchDialogOpen = false;
    }

    [RelayCommand]
    void CloseInventoryDialog()
    {
        IsInventoryDialogOpen = false;
    }

    [RelayCommand(CanExecute = nameof(CanResearch))]
    void Research()
    {
        Console.WriteLine("Research");

        if(SelectedResearch == null)
            return;

        Session.State.DoResearch(SelectedResearch);

        ResearchCommand.NotifyCanExecuteChanged(); // Not Researched -> Researched may disable the button
    }

    bool CanResearch()
    {
        if(SelectedResearch == null)
            return false;
        return Session.State.IsResearchable(SelectedResearch);
    }

    // public SelectionModel<Research> ResearchSelection{get;} = new();
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResearchCommand))]
    Research? selectedResearch;
    
    // public SelectionModel<ComponentDef> InventoryComponentDefSelection{get;} = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DoPurchaseComponentCommand))]
    ComponentDef? selectedComponentDef;    

    // public SelectionModel<Component> InventoryComponentSelection{get;} = new(){SingleSelect = false};
    public ObservableCollection<Component> SelectedInventoryComponents{get;} = new();

    [RelayCommand(CanExecute = nameof(CanDoPurchaseComponent))]
    void DoPurchaseComponent()
    {
        Console.WriteLine("DoPurchaseComponent");

        if(SelectedComponentDef == null)
            return;

        Session.State.BuyComponent(SelectedComponentDef);

        DoPurchaseComponentCommand.NotifyCanExecuteChanged();
    }

    bool CanDoPurchaseComponent()
    {
        if(SelectedComponentDef == null)
            return false;
        
        // if(SelectedComponentDef.Price > )
        return Session.State.CanBuyComponent(SelectedComponentDef);
    }

    [RelayCommand]
    void DoAssembleStarcraft()
    {
        Console.WriteLine("DoAssembleStarcraft");

        Session.State.AssembleSpacecraft(SelectedInventoryComponents);

        SelectedInventoryComponents.Clear();
    }

    // bool CanDoAssembleStarcraft()
    // {
    //     return SelectedInventoryComponents.Count > 0;
    // }

    [RelayCommand]
    void DoNextTurn()
    {
        Session.State.NextTurnHousekeeping();
    }
}
