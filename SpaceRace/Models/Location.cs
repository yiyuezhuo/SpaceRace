using CommunityToolkit.Mvvm.ComponentModel;

namespace SpaceRace.Models;

public enum LocationType
{
    Planet,
    Orbit,
    FlyBy
}

public class Location
{
    public string Name = "";
    public LocationType Type;
    public bool IsHome; // Earth
    public bool Explored;
    // variant (Mars, Moon variant, supply, sample etc)
}