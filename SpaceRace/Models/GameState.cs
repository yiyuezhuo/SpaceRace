using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;

namespace SpaceRace.Models;

public class GameState : ObservableObject
{
    public int Year{get;set;} = 1956;
    public int Funds{get;set;} = 25;
    public int Score{get;set;} = 0;

    public ObservableCollection<Research> Researches { get; } = new()
    {
        new() { Name = "Juno Rocket", Unlocks= new() { BuildableTag.JunoRocket } },
        new() { Name = "Atlas Rocket", Unlocks= new() { BuildableTag.AtlasRocket } },
        new() { Name = "Soyuz Rocket", Unlocks= new() { BuildableTag.SoyuzRocket } },
        new() { Name = "Saturn Rocket", Unlocks= new() { BuildableTag.SaturnRocket } },
        new() { Name = "Ion Thruster", Unlocks= new() { BuildableTag.IonThruster } },
        new() { Name = "Rendezvous", EnableRendezvous=true},
        new() { Name = "Re-Entry", EnableReEntry=true},
        new() { Name = "Landing", EnableLanding=true},
        new() { Name = "Life Support", EnableLifeSupport=true},
        new() { Name = "Surveying", EnableSurveying=true},
    };

    public ObservableCollection<ComponentDef> ComponentDefs { get; } = new()
    {
        ComponentDef.MakeThruster("Juno", BuildableTag.JunoRocket, mass: 1, price: 1, thrust: 4),
        ComponentDef.MakeThruster("Atlas", BuildableTag.AtlasRocket, mass: 4, price: 5, thrust: 27),
        ComponentDef.MakeThruster("Soyuz", BuildableTag.SoyuzRocket, mass: 9, price: 8, thrust: 80),
        ComponentDef.MakeThruster("Saturn", BuildableTag.SaturnRocket, mass: 20, price: 15, thrust: 200),
        ComponentDef.MakeThruster("Ion Thruster", BuildableTag.IonThruster, mass: 1, price: 10, thrust: 5, isOneshot: false),
        new ComponentDef(){Name="Probe", Type=ComponentType.Device, Surveyable=true, Mass=1, Price=2},
        ComponentDef.MakeCapsule("Vostok", mass: 2, price: 2, seats: 1, hasHeatShield: true),
        ComponentDef.MakeCapsule("Apollo", mass: 3, price: 4, seats: 3, hasHeatShield: true),
        ComponentDef.MakeCapsule("Eagle", mass: 1, price: 4, seats: 2, hasHeatShield: false),
        ComponentDef.MakeCapsule("Aldrin", mass: 3, price: 4, seats: 8, hasHeatShield: false, radiationModifier: -1),
        new ComponentDef(){Name="Supply", Type=ComponentType.Supply, Mass=1, Price=1},
        ComponentDef.MakeAstronaut("Boris Yegorov", isDoctor: true),
        ComponentDef.MakeAstronaut("Buzz Aldrin", isMechanic: true),
        ComponentDef.MakeAstronaut("Neil Armstrong", isPilot: true),
        ComponentDef.MakeAstronaut("Valery Bykovsky", isDoctor: true),
        ComponentDef.MakeAstronaut("Mike Collins", isDoctor: true),
        ComponentDef.MakeAstronaut("Konstantin Feoktistov", isMechanic: true),
        ComponentDef.MakeAstronaut("Yuri Gagarin", isPilot: true),
        ComponentDef.MakeAstronaut("John Glenn", isPilot: true),
        ComponentDef.MakeAstronaut("Gus Grissom", isMechanic: true),
        ComponentDef.MakeAstronaut("Vladimir Komarov", isMechanic: true),
        ComponentDef.MakeAstronaut("Jim Lovell", isMechanic: true),
        ComponentDef.MakeAstronaut("Alan Shepard", isPilot: true),
        ComponentDef.MakeAstronaut("Valentina Tereshkova", isDoctor: true),
        ComponentDef.MakeAstronaut("Gherman Titov", isDoctor: true),
        ComponentDef.MakeAstronaut("Joseph Walker", isPilot: true),
    };

    public ObservableCollection<Location> Locations { get; } = new();
    public List<Route> Routes{get;} = new();
    public List<Component> Inventory{get;} = new();
    public List<Spacecraft> Spacecrafts{get;} = new();

    public Dictionary<string, Location> NameToLocation = new();
    public Dictionary<Location, Dictionary<Location, Route>> SourceToDestinationToRoute = new();

    public GameState()
    {
        // Earth
        Location Earth = new(){Name="Earth", IsHome=true, Explored=true};
        Location SuborbitalFlight = new(){Name="Suborbital Flight", Type=LocationType.Planet};
        Location EarthOrbit = new(){Name="Earth Orbit", Type=LocationType.Orbit};

        // Moon
        Location Moon = new(){Name="Moon", Type=LocationType.Planet};
        Location LunarOrbit = new(){Name="Lunar Orbit", Type=LocationType.Orbit};
        Location LunarFlyBy = new(){Name="Lunar Fly-By", Type=LocationType.FlyBy};

        // Mars
        Location Mars = new(){Name="Mars", Type=LocationType.Planet};
        Location MarsOrbit = new(){Name="Mars Orbit", Type=LocationType.Orbit};
        Location MarsFlyBy = new(){Name="Mars Fly-By", Type=LocationType.FlyBy};
        Location Phobos = new(){Name="Phobos", Type=LocationType.Planet};

        // Venus
        Location Venus = new(){Name="Venus", Type=LocationType.Planet};
        Location VenusOrbit = new(){Name="Venus Orbit", Type=LocationType.Orbit};
        Location VenusFlyBy = new(){Name="Venus Fly-By", Type=LocationType.FlyBy};

        // Ceres
        Location Ceres = new(){Name="Ceres", Type=LocationType.Planet};

        // Inner Planets Transfer
        Location InnerPlanetsTransfer = new(){Name="Inner Planets Transfer", Type=LocationType.FlyBy};

        Locations = new()
        {
            Earth, SuborbitalFlight, EarthOrbit, // Earth
            Moon, LunarOrbit, LunarFlyBy, // Moon
            Mars, MarsOrbit, MarsFlyBy, Phobos, // Mars
            Venus, VenusOrbit, VenusFlyBy, // Venus
            Ceres, // Ceres
            InnerPlanetsTransfer // Inner Planets Transfer
        };

        Routes = new()
        {
            Route.Make(Earth, SuborbitalFlight, 3, hasAstronautIncapacitedCheck: true),
            Route.Make(Earth, EarthOrbit, 8, hasAstronautIncapacitedCheck: true),
            
            Route.Make(SuborbitalFlight, EarthOrbit, 5),

            Route.Make(EarthOrbit, LunarOrbit, 3),
            Route.Make(EarthOrbit, LunarFlyBy, 1),
            Route.Make(EarthOrbit, MarsOrbit, 5, years: 3, hasRadiation: true),
            Route.Make(EarthOrbit, MarsFlyBy, 3, years: 3, hasRadiation: true),
            Route.Make(EarthOrbit, InnerPlanetsTransfer, 3, hasRadiation: true, years: 1),

            Route.Make(LunarFlyBy, LunarOrbit, 2),
            Route.Make(LunarFlyBy, EarthOrbit, 1),
            Route.Make(LunarFlyBy, Moon, 4, isLanding: true),

            Route.Make(LunarOrbit, EarthOrbit, 3),
            Route.Make(LunarOrbit, Moon, 2, isLanding: true),

            Route.Make(Moon, LunarOrbit, 2),

            Route.Make(MarsFlyBy, MarsOrbit, 3),
            Route.Make(MarsFlyBy, Mars, 3, isReEntry: true, isLanding: true),

            Route.Make(MarsOrbit, Mars, 0, isReEntry: true, isLanding: true),
            Route.Make(MarsOrbit, EarthOrbit, 5, years: 3, hasRadiation: true),
            Route.Make(MarsOrbit, InnerPlanetsTransfer, 4, years: 2, hasRadiation: true),
            Route.Make(MarsOrbit, Phobos, 1, isLanding: true),
            
            Route.Make(Phobos, MarsOrbit, 1),
            
            Route.Make(Mars, MarsOrbit, 3),

            Route.Make(VenusFlyBy, VenusOrbit, 1),
            Route.Make(VenusFlyBy, Venus, 1, isReEntry: true, isLanding: true),

            Route.Make(VenusOrbit, Venus, 0, isReEntry: true, isLanding: true),
            Route.Make(VenusOrbit, InnerPlanetsTransfer, 3, years: 1, hasRadiation: true),

            Route.Make(Venus, VenusOrbit, 6),

            Route.Make(Ceres, InnerPlanetsTransfer, 3, years: 1, hasRadiation: true),

            Route.Make(InnerPlanetsTransfer, EarthOrbit, 3, years: 1),
            Route.Make(InnerPlanetsTransfer, MarsOrbit, 4, years: 2, hasRadiation: true),
            Route.Make(InnerPlanetsTransfer, VenusOrbit, 3, years: 1, hasRadiation: true),
            Route.Make(InnerPlanetsTransfer, VenusFlyBy, 2, years: 1, hasRadiation: true),
            Route.Make(InnerPlanetsTransfer, Ceres, 5, years: 1, hasRadiation: true, isLanding: true),
        };

        // Create Index
        NameToLocation = Locations.ToDictionary(l => l.Name, l => l);
        foreach(var grouping in Routes.GroupBy(r => r.Source))
        {
            SourceToDestinationToRoute[grouping.Key] = grouping.ToDictionary(r => r.Destination, r => r);
        }

    }
}