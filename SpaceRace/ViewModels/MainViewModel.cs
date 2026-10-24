using CommunityToolkit.Mvvm.ComponentModel;
using SpaceRace.Models;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System;
using Avalonia.Controls.Selection;
using System.Linq;
using System.Collections.Specialized;
using SpaceRace.Views;


namespace SpaceRace.ViewModels;

// public record Country(string Name, string Region, int Population);

public enum MainViewModelSelectState
{
    Select,
    SelectManeuverTarget
}

public partial class MainViewModel : ViewModelBase
{
    public GameSession Session{get;} = new();

    // public Spacecraft? SelectedSpacecraft{get;set;} = null;

    // [ObservableProperty]
    // public partial string Greeting { get; set; } = "Welcome to Avalonia!";

    // public Dictionary<Location, LocationMarkerViewModel> LocationMarkers{get;}


    public MainViewModel()
    {
        
    }

    public MainViewModel(GameSession session)
    {
        Session = session;

        Session.State.Spacecrafts.CollectionChanged += OnSpacecraftChanged;

        // LocationMarkers = Session.State.Locations.ToDictionary(
        //     location => location,
        //     location => new LocationMarkerViewModel(location)
        // );

        // RefreshLocationMarkers();
    }

    void OnSpacecraftChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SpacecraftsOfSelectedLocation));
    }

    // public ObservableCollection<Country> Countries { get; } =
    // [
    //     new("Afghanistan", "Asia", 31056997),
    //     new("Albania", "Eastern Europe", 3581655),
    //     new("Algeria", "Northern Africa", 32930091),
    // ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LocationDescription))]
    [NotifyPropertyChangedFor(nameof(SpacecraftsOfSelectedLocation))]
    Location? selectedLocation;

    [ObservableProperty]
    Spacecraft? selectedSpacecraft;

    public List<Spacecraft> SpacecraftsOfSelectedLocation
    {
        get => Session.State.Spacecrafts.Where(s => s.Location == SelectedLocation).ToList();
    }

    public MainViewModelSelectState MainViewModelSelectState;

    public void SelectLocation(Location location)
    {
        Console.WriteLine($"Select Location: {location}");

        if(MainViewModelSelectState == MainViewModelSelectState.Select)
        {
            SelectedLocation = location;
        }
        else if(MainViewModelSelectState == MainViewModelSelectState.SelectManeuverTarget)
        {
            // TODO: Check if it's a valid target
            if(SelectedSpacecraft != null && Session.State.TryGetManeuverRoute(SelectedSpacecraft, location, out var route) && route != null)
            {
                Session.State.StartPrepareManeuver(SelectedSpacecraft, route, route.Years);
                
                IsManeuverDialogOpen = true;
            }

            MainViewModelSelectState = MainViewModelSelectState.Select;
        }
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

    [RelayCommand]
    void OpenMissionDialog()
    {
        Console.WriteLine("OpenMissionDialog");

        IsMissionDialogOpen = true;
    }

    [ObservableProperty]
    bool isResearchDialogOpen;

    [ObservableProperty]
    bool isInventoryDialogOpen;

    [ObservableProperty]
    bool isMissionDialogOpen;

    [ObservableProperty]
    bool isMessageDialogOpen;

    [RelayCommand]
    void CloseMessageDialog()
    {
        IsMessageDialogOpen = false;
    }

    [RelayCommand]
    void CloseMissionDialog()
    {
        IsMissionDialogOpen = false;
    }

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

    [ObservableProperty]
    string messageDialogTitle = "Title";
    
    [ObservableProperty]
    string messageDialogContent = "Content";

    public void ShowMessage(string content="Content", string title="Message")
    {
        MessageDialogContent = content;
        MessageDialogTitle = title;

        IsMessageDialogOpen = true;
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

    public ObservableCollection<Component> SelectedComponentsOfSelectedSpacecraft{get;} = new();
    public int MassOfSelectedComponentsOfSelectedSpacecraft => SelectedComponentsOfSelectedSpacecraft.Sum(c => c.Def.Mass);

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
        if(Session.State.IsGameEnd())
        {
            ShowMessage($"Game End, Final Score: {Session.State.Score}");
            return;
        }
        
        Session.State.NextTurnHousekeeping();
        // Branch End of Game
    }

    [RelayCommand]
    void DoManeuver()
    {
        Console.WriteLine("DoManeuver");
    }

    [RelayCommand]
    void DoDock()
    {
        Console.WriteLine("DoDock");
    }

    [RelayCommand]
    void DoSeparate()
    {
        Console.WriteLine("DoSeparate");

        if(SelectedSpacecraft == null || !CanDoSeparate())
            return;

        Session.State.SeparateSpacecraft(SelectedSpacecraft, SelectedComponentsOfSelectedSpacecraft);
    }

    bool CanDoSeparate()
    {
        return SelectedSpacecraft != null && SelectedComponentsOfSelectedSpacecraft.Count > 0;
    }

    [RelayCommand(CanExecute =nameof(CanDoDisassemble))]
    void DoDisassemble()
    {
        Console.WriteLine("DoDisassemble");

        if(SelectedSpacecraft == null)
            return;

        Session.State.DisassembleSpacecraft(SelectedSpacecraft);
    }

    bool CanDoDisassemble()
    {
        if(SelectedLocation == null || SelectedSpacecraft == null)
            return false;

        return SelectedLocation.IsHome;
    }

    [RelayCommand(CanExecute =nameof(CanDoRepair))]
    void DoRepair() => Session.State.DoRepair(SelectedSpacecraft);

    bool CanDoRepair() => Session.State.CanDoRepair(SelectedSpacecraft);

    [RelayCommand(CanExecute =nameof(CanDoHeal))]
    void DoHeal() => Session.State.DoHeal(SelectedSpacecraft);

    bool CanDoHeal() => Session.State.CanDoHeal(SelectedSpacecraft);

    [ObservableProperty]
    bool isManeuverDialogOpen;

    [RelayCommand]
    void CancelManeuverDialog()  => IsManeuverDialogOpen = false;

    [RelayCommand(CanExecute =nameof(CanApplyManeuver))]
    void ApplyManeuver()
    {
        Session.State.BeginPrepareManeuver();
    }

    bool CanApplyManeuver() => Session.State.PrepareManeuverBeginable();

    public List<Component> AvailableThruesterOfSelectedSpacecraft = new();

    public bool IsManeuverDialogTargetYearReadOnly
    {
        get
        {
            var state = Session.State.PrepareManeuverState;
            if(state == null)
                return true;
            return state.AppliedThrust > 0;
        }
    }
}
