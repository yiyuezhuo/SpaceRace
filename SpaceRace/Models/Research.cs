using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace SpaceRace.Models;

public partial class Research : ObservableObject
{
    public string Name {get; init;} = "";
    public bool Reseached{get; init;} = false;
    public HashSet<BuildableTag> Unlocks = new();
    public bool EnableRendezvous = false;
    public bool EnableReEntry = false;
    public bool EnableLanding = false;
    public bool EnableLifeSupport = false;
    public bool EnableSurveying = false;
}