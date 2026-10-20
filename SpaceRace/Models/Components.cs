using CommunityToolkit.Mvvm.ComponentModel;

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

public class Component
{
    public string Name = "";
    public ComponentType Type;
    public BuildableTag Tag = BuildableTag.None;
    public bool IsOneshot = false;
    public int Quantity = 0;
    public int Price = 1;
    public int Mass = 1;
    public int Thrust = 0;
    public bool Surveyable = false;
    public int Seats = 0;
    public bool HasHeatShield = false;
    public int RadiationModifier = 0;
    public bool IsDoctor = false;
    public bool IsMechanic = false;
    public bool IsPilot = false;
    public bool damaged = false;

    public static Component MakeThruster(string name, BuildableTag tag, int mass, int price, int thrust, bool isOneshot = true)
    {
        return new()
        {
            Name=name,
            Type=ComponentType.Device,
            Surveyable = true,
            Tag=tag,
            Mass=mass,
            Price=price,
            IsOneshot=isOneshot,
        };
    }

    public static Component MakeCapsule(string name, int mass, int price, int seats, bool hasHeatShield, int radiationModifier=0)
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

    public static Component MakeAstronaut(string name, bool isDoctor = false, bool isMechanic = false, bool isPilot = false, int price=2)
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