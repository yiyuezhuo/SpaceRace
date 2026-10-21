using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace SpaceRace.Models;

public class Route
{
    public required Location Source{get;set;}
    public required Location Destination{get;set;}
    public int Difficulty{get;set;}
    public bool IsLanding{get;set;} = false;
    public bool IsReEntry{get;set;} = false; // deep atomsphere
    public bool HasRadiation{get;set;}
    public bool HasAstronautIncapacitedCheck{get;set;}
    public int Years{get;set;} = 0;

    public string Description
    {
        get
        {
            var words = new List<string>();
            if(IsLanding)
            {
                words.Add($"Landing");
            }
            if(IsReEntry)
            {
                words.Add($"Re-Entry");
            }
            if(HasRadiation)
            {
                words.Add($"Radiation");
            }
            if(HasAstronautIncapacitedCheck)
            {
                words.Add($"Astronaut Incapacitation Check");
            }
            return string.Join(",", words);
        }
    }

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