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
    public string Name{get;set;} = "";
    public LocationType Type{get;set;}
    public bool IsHome{get;set;} // Earth
    public bool Explored{get;set;}
    // variant (Mars, Moon variant, supply, sample etc)

    public override string ToString()
    {
        return $"Location({Name})";
    }
}