using CommunityToolkit.Mvvm.ComponentModel;

namespace SpaceRace.Models;

public class Route
{
    public required Location Source;
    public required Location Destination;
    public int Difficulty;
    public bool IsLanding = false;
    public bool IsReEntry = false; // deep atomsphere
    public bool HasRadiation;
    public bool HasAstronautIncapacitedCheck;
    public int Years = 0;

    public static Route Make(Location source, Location destination, int difficulty, bool isLanding = false, bool isReEntry = false, bool hasRadiation = false, bool hasAstronautIncapacitedCheck = false, int years = 0)
    {
        return new()
        {
            Source = source,
            Destination = destination,
            Difficulty = difficulty,
            IsLanding = isLanding,
            IsReEntry = isReEntry,
            HasRadiation = hasRadiation,
            HasAstronautIncapacitedCheck = hasAstronautIncapacitedCheck,
            Years = years
        };
    }
}