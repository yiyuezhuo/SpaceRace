using CommunityToolkit.Mvvm.ComponentModel;
using SpaceRace.Models;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System;


namespace SpaceRace.ViewModels;

// public record Country(string Name, string Region, int Population);

public partial class MainViewModel : ViewModelBase
{
    public GameSession Session{get;} = new();

    public Spacecraft? SelectedSpacecraft{get;set;} = null;

    // [ObservableProperty]
    // public partial string Greeting { get; set; } = "Welcome to Avalonia!";


    public MainViewModel()
    {
        
    }

    public MainViewModel(GameSession session)
    {
        Session = session;
    }

    // public ObservableCollection<Country> Countries { get; } =
    // [
    //     new("Afghanistan", "Asia", 31056997),
    //     new("Albania", "Eastern Europe", 3581655),
    //     new("Algeria", "Northern Africa", 32930091),
    // ];

    public void SelectLocation(Location location)
    {
        Console.WriteLine($"Select Location: {location}");
    }
}
