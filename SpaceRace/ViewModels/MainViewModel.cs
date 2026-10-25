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

        // Session.State.Spacecrafts.CollectionChanged += OnSpacecraftChanged;

        SelectedInventoryComponents.CollectionChanged += (_, _) =>
        {
            // DoAssembleStarcraftCommand.NotifyCanExecuteChanged();
            NotifySomethingChanged();
        };

        // SelectedSpacecraft.PropertyChanged += (_, _) =>
        // {
            
        // };

        // LocationMarkers = Session.State.Locations.ToDictionary(
        //     location => location,
        //     location => new LocationMarkerViewModel(location)
        // );

        // RefreshLocationMarkers();
    }

    partial void OnSelectedSpacecraftChanged(Spacecraft? value)
    {
        SelectedComponentsOfSelectedSpacecraft.Clear();
        NotifySomethingChanged();
    }

    // void OnSpacecraftChanged(object? sender, NotifyCollectionChangedEventArgs e)
    // {
    //     OnPropertyChanged(nameof(SpacecraftsOfSelectedLocation));
    // }

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
    // [NotifyPropertyChangedFor(nameof(ThrustersOfSelectedSpacecraft))]
    // [NotifyCanExecuteChangedFor(nameof(DoManeuverCommand))]
    // [NotifyCanExecuteChangedFor(nameof(DoDockCommand))]
    // [NotifyCanExecuteChangedFor(nameof(DoSeparateCommand))]
    // [NotifyCanExecuteChangedFor(nameof(DoDisassembleCommand))]
    // [NotifyCanExecuteChangedFor(nameof(DoRepairCommand))]
    // [NotifyCanExecuteChangedFor(nameof(DoHealCommand))]
    Spacecraft? selectedSpacecraft;

    public List<Spacecraft> SpacecraftsOfSelectedLocation
    {
        get => Session.State.Spacecrafts.Where(s => s.Location == SelectedLocation).ToList();
    }

    public MainViewModelSelectState SelectState;

    public void SelectLocation(Location location)
    {
        Console.WriteLine($"Select Location: {location}");

        if(SelectState == MainViewModelSelectState.Select)
        {
            SelectedLocation = location;
        }
        else if(SelectState == MainViewModelSelectState.SelectManeuverTarget)
        {
            // TODO: Check if it's a valid target
            if(SelectedSpacecraft != null && Session.State.TryGetManeuverRoute(SelectedSpacecraft, location, out var route) && route != null)
            {
                Session.State.StartPrepareManeuver(SelectedSpacecraft, route, route.Years);
                
                // RefreshManeuverDialog();

                IsManeuverDialogOpen = true;
            }

            SelectState = MainViewModelSelectState.Select;
        }

        NotifySomethingChanged();
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

        NotifySomethingChanged();

        // ResearchCommand.NotifyCanExecuteChanged(); // Not Researched -> Researched may disable the button
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

        NotifySomethingChanged();

        // DoPurchaseComponentCommand.NotifyCanExecuteChanged();
    }

    bool CanDoPurchaseComponent()
    {
        if(SelectedComponentDef == null)
            return false;
        
        // if(SelectedComponentDef.Price > )
        return Session.State.CanBuyComponent(SelectedComponentDef);
    }

    [RelayCommand(CanExecute =nameof(CanDoAssembleStarcraft))]
    void DoAssembleStarcraft()
    {
        Console.WriteLine("DoAssembleStarcraft");

        Session.State.AssembleSpacecraft(SelectedInventoryComponents);

        SelectedInventoryComponents.Clear();

        NotifySomethingChanged();
    }

    bool CanDoAssembleStarcraft()
    {
        return SelectedInventoryComponents.Count > 0;
    }

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

        NotifySomethingChanged();
    }

    [RelayCommand(CanExecute =nameof(CanDoManeuver))]
    void DoManeuver()
    {
        Console.WriteLine("DoManeuver");

        SelectState = MainViewModelSelectState.SelectManeuverTarget;
    }

    bool CanDoManeuver()
    {
        return SelectedSpacecraft != null;
    }

    [RelayCommand(CanExecute =nameof(CanDoDock))]
    void DoDock()
    {
        Console.WriteLine("DoDock");
    }

    bool CanDoDock()
    {
        return SelectedSpacecraft != null && !SelectedSpacecraft.Location.IsHome;
    }

    [RelayCommand(CanExecute =nameof(CanDoSeparate))]
    void DoSeparate()
    {
        Console.WriteLine("DoSeparate");

        if(SelectedSpacecraft == null || !CanDoSeparate())
            return;

        Session.State.SeparateSpacecraft(SelectedSpacecraft, SelectedComponentsOfSelectedSpacecraft);
    
        NotifySomethingChanged();
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

        NotifySomethingChanged();
    }

    bool CanDoDisassemble()
    {
        if(SelectedLocation == null || SelectedSpacecraft == null)
            return false;

        return SelectedSpacecraft.Location.IsHome;
    }

    [RelayCommand(CanExecute =nameof(CanDoRepair))]
    void DoRepair()
    {
        Session.State.DoRepair(SelectedSpacecraft);
    
        NotifySomethingChanged();
    }

    bool CanDoRepair() => Session.State.CanDoRepair(SelectedSpacecraft);

    [RelayCommand(CanExecute =nameof(CanDoHeal))]
    void DoHeal()
    {
        Session.State.DoHeal(SelectedSpacecraft);

        NotifySomethingChanged();
    }

    bool CanDoHeal() => Session.State.CanDoHeal(SelectedSpacecraft);

    [ObservableProperty]
    bool isManeuverDialogOpen;

    [RelayCommand]
    void CancelManeuverDialog()  => IsManeuverDialogOpen = false;

    // [RelayCommand(CanExecute =nameof(CanApplyManeuver))]
    // void ApplyManeuver()
    // {
    //     Session.State.BeginPrepareManeuver();

    //     NotifySomethingChanged();
    // }

    // bool CanApplyManeuver() => Session.State.PrepareManeuverBeginable();

    // public List<Component> ThrustersOfSelectedSpacecraft
    // {
    //     get
    //     {
    //         if(SelectedSpacecraft == null)
    //             return new();
    //         return SelectedSpacecraft.Components.Where(c => c.Def.Thrust > 0).ToList(); // Contain "Ready" thruster and unready thruster (damaged or used)
    //     }
    // }

    // public bool IsManeuverDialogTargetYearReadOnly
    // {
    //     get
    //     {
    //         var state = Session.State.PrepareManeuverState;
    //         if(state == null)
    //             return true;
    //         return state.AppliedThrust > 0;
    //     }
    // }

    [RelayCommand(CanExecute =nameof(CanUseThruster))]
    void UseThruster(Component thruster)
    {
        Session.State.UseThrusterForManeuver(thruster);

        if(Session.State.PrepareManeuverState != null && Session.State.PrepareManeuverState.AppliedThrust >= Session.State.PrepareManeuverState.RequiredThrust)
        {
            Session.State.BeginPrepareManeuver();
            NotifySomethingChanged();
        }

        // RefreshManeuverDialog();
        NotifySomethingChanged();
    }

    // void RefreshManeuverDialog()
    // {
    //     // TODO: Fix the Code-smell
    //     // OnPropertyChanged(nameof(ThrustersOfSelectedSpacecraft));
    //     // OnPropertyChanged(nameof(IsManeuverDialogTargetYearReadOnly));
    // }

    void RefreshViewModels()
    {
        // Spacecraft Control Buttons
        DoManeuverCommand.NotifyCanExecuteChanged();
        DoDockCommand.NotifyCanExecuteChanged();
        DoDisassembleCommand.NotifyCanExecuteChanged();
        DoSeparateCommand.NotifyCanExecuteChanged();
        DoRepairCommand.NotifyCanExecuteChanged();
        DoHealCommand.NotifyCanExecuteChanged();

        DoPurchaseComponentCommand.NotifyCanExecuteChanged();
        DoAssembleStarcraftCommand.NotifyCanExecuteChanged();

        UseThrusterCommand.NotifyCanExecuteChanged();
        // ApplyManeuverCommand.NotifyCanExecuteChanged();

        ResearchCommand.NotifyCanExecuteChanged();

        // Complex derived attributes
        OnPropertyChanged(nameof(SpacecraftsOfSelectedLocation));

        // Refresh Maneuver Dialog
        // OnPropertyChanged(nameof())

        SyncLogs();
    }

    public void RefreshViewModelsIfDirty()
    {
        if(Dirty)
        {
            Dirty = false;
            RefreshViewModels();
        }
    }

    public void NotifySomethingChanged()
    {
        // Dirty = true;
        RefreshViewModels();
    }

    public bool Dirty;

    bool CanUseThruster(Component thruster) => thruster.IsAvailable;

    int displayedLogs = 0;

    public void SyncLogs()
    {
        if(displayedLogs < Session.State.Logs.Count)
        {
            var LogStr = string.Join("\n", Session.State.Logs.Skip(displayedLogs));

            ShowMessage(LogStr, "Score Update");

            displayedLogs = Session.State.Logs.Count;
        }
    }
}
