using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace SpaceRace.Models;

public enum ComponentType
{
    Device,
    Capsule,
    Supply,
    Astronaut
}

public enum BuildableTag
{
    None,
    JunoRocket,
    AtlasRocket,
    SoyuzRocket,
    SaturnRocket,
    IonThruster,
}

public class ComponentDef
{
    public string Name{get;set;} = "";
    public ComponentType Type{get;set;}
    public BuildableTag Tag{get;set;} = BuildableTag.None;
    public bool IsOneshot{get;set;} = false;
    // public int Quantity{get;set;} = 0;
    public int Price{get;set;} = 1;
    public int Mass{get;set;} = 1;
    public int Thrust{get;set;} = 0;
    public bool Surveyable{get;set;} = false;
    public int Seats{get;set;} = 0;
    public bool HasHeatShield{get;set;} = false;
    public int RadiationModifier{get;set;} = 0;
    public bool IsDoctor{get;set;} = false;
    public bool IsMechanic{get;set;} = false;
    public bool IsPilot{get;set;} = false;
    // public bool Damaged{get;set;} = false;

    public string Description
    {
        get
        {
            var words = new List<string>();
            if(IsOneshot)
            {
                words.Add($"One-Shot");
            }
            if(Thrust > 0)
            {
                words.Add($"Thrust: {Thrust}");
            }
            if(Surveyable)
            {
                words.Add($"Surveyable");
            }
            if(Seats > 0)
            {
                words.Add($"Seats: {Seats}");
            }
            if(HasHeatShield)
            {
                words.Add("Heat Shield");
            }
            if(RadiationModifier != 0)
            {
                words.Add($"Radiation Modifier: {RadiationModifier}");
            }
            if(IsDoctor) words.Add("Doctor");
            if(IsMechanic) words.Add("Mechanic");
            if(IsPilot) words.Add("Pilot");
            return string.Join(", ", words);
        }
    }

    public static ComponentDef MakeThruster(string name, BuildableTag tag, int mass, int price, int thrust, bool isOneshot = true)
    {
        return new()
        {
            Name=name,
            Type=ComponentType.Device,
            Tag=tag,
            Mass=mass,
            Price=price,
            IsOneshot=isOneshot,
            Thrust=thrust
        };
    }

    public static ComponentDef MakeCapsule(string name, int mass, int price, int seats, bool hasHeatShield, int radiationModifier=0)
    {
        return new()
        {
            Name=name,
            Type=ComponentType.Capsule,
            Surveyable = true,
            Mass=mass,
            Price=price,
            Seats=seats,
            HasHeatShield=hasHeatShield,
            RadiationModifier=radiationModifier,
        };
    }

    public static ComponentDef MakeAstronaut(string name, bool isDoctor = false, bool isMechanic = false, bool isPilot = false, int price=2)
    {
        return new()
        {
            Name=name,
            Type=ComponentType.Astronaut,
            IsDoctor=isDoctor,
            IsMechanic=isMechanic,
            IsPilot=isPilot,
            Price=price
        };
    }
}

public class Component
{
    public required ComponentDef Def{get;set;}
    public bool Damaged{get;set;}
    public bool Used{get;set;} // Ion Thruster

    public string FullDescription
    {
        get
        {
            var words = new List<string>();
            if(Damaged)
            {
                words.Add("Damaged");
            }
            if(Used)
            {
                words.Add("Used");
            }
            if(words.Count == 0)
                return Def.Name;
            return $"{Def.Name} ({string.Join(", ", words)})";
        }
    }

    public bool IsAvailable => !(Damaged || Used);
}