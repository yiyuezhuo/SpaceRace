using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace SpaceRace.Models;

public class ModelState : ObservableObject
{
    public int year = 1956;
    public int funds = 25;
    public int score = 0;

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

    public ObservableCollection<Component> Components { get; } = new()
    {
        Component.MakeThruster("Juno", BuildableTag.JunoRocket, mass: 10, price: 1, thrust: 20),
        Component.MakeThruster("Atlas", BuildableTag.AtlasRocket, mass: 20, price: 5, thrust: 40),
        Component.MakeThruster("Soyuz", BuildableTag.SoyuzRocket, mass: 25, price: 8, thrust: 50),
        Component.MakeThruster("Saturn", BuildableTag.SaturnRocket, mass: 30, price: 15, thrust: 60),
        Component.MakeThruster("Ion Thruster", BuildableTag.IonThruster, mass: 5, price: 10, thrust: 30, isOneshot: false),
        new Component(){Name="Probe", Type=ComponentType.Device, Surveyable=true, Mass=1, Price=2},
        Component.MakeCapsule("Vostok", mass: 2, price: 2, seats: 1, hasHeatShield: true),
        Component.MakeCapsule("Apollo", mass: 3, price: 4, seats: 3, hasHeatShield: true),
        Component.MakeCapsule("Eagle", mass: 1, price: 4, seats: 2, hasHeatShield: false),
        Component.MakeCapsule("Aldrin", mass: 3, price: 4, seats: 8, hasHeatShield: false, radiationModifier: -1),
        new Component(){Name="Supply", Type=ComponentType.Supply, Mass=1, Price=1},
        Component.MakeAstronaut("Boris Yegorov", isDoctor: true),
        Component.MakeAstronaut("Buzz Aldrin", isMechanic: true),
        Component.MakeAstronaut("Neil Armstrong", isPilot: true),
        Component.MakeAstronaut("Valery Bykovsky", isDoctor: true),
        Component.MakeAstronaut("Mike Collins", isDoctor: true),
        Component.MakeAstronaut("Konstantin Feoktistov", isMechanic: true),
        Component.MakeAstronaut("Yuri Gagarin", isPilot: true),
        Component.MakeAstronaut("John Glenn", isPilot: true),
        Component.MakeAstronaut("Gus Grissom", isMechanic: true),
        Component.MakeAstronaut("Vladimir Komarov", isMechanic: true),
        Component.MakeAstronaut("Jim Lovell", isMechanic: true),
        Component.MakeAstronaut("Alan Shepard", isPilot: true),
        Component.MakeAstronaut("Valentina Tereshkova", isDoctor: true),
        Component.MakeAstronaut("Gherman Titov", isDoctor: true),
        Component.MakeAstronaut("Joseph Walker", isPilot: true),
    };

    public ObservableCollection<Location> Locations { get; } = new();
    public List<Route> routes = new();

    public ModelState()
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

        routes = new()
        {
            Route.Make(Earth, SuborbitalFlight, 3, hasAstronautIncapacitedCheck: true),
            Route.Make(Earth, EarthOrbit, 8, hasAstronautIncapacitedCheck: true),
            
            Route.Make(SuborbitalFlight, EarthOrbit, 5),

            Route.Make(EarthOrbit, LunarOrbit, 3),
            Route.Make(EarthOrbit, LunarFlyBy, 1),
            Route.Make(EarthOrbit, MarsOrbit, 5, years: 3, hasRadiation: true),
            Route.Make(EarthOrbit, MarsFlyBy, 3, years: 3, hasRadiation: true),
            Route.Make(EarthOrbit, InnerPlanetsTransfer, 3, hasRadiation: true),

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

    }
}