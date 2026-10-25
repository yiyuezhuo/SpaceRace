using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System;
using Avalonia.Controls.Embedding.Offscreen;

namespace SpaceRace.Models;

public partial class PrepareManeuverState : ObservableObject
{
    public required Spacecraft Spacecraft{get;set;}
    public required Route Route{get;set;}
    public required int StartMass{get;set;}
    public required int RequiredThrust{get;set;}
    public required int TargetYears{get;set;}

    [ObservableProperty]
    public int appliedThrust;
}

public partial class GameState : ObservableObject
{
    [ObservableProperty]
    int year = 1956;

    [ObservableProperty]
    int endYear = 1976;

    [ObservableProperty]
    int funds = 25;

    [ObservableProperty]
    int score = 0;

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
    public ObservableCollection<Component> Inventory{get;} = new();
    public ObservableCollection<Spacecraft> Spacecrafts{get;} = new();

    public List<string> Logs{get;} = new();
    public void Log(string log)
    {
        Console.WriteLine(log);
        Logs.Add(log);
        // SHow it in dialog through some event?
    }

    public Dictionary<string, Location> NameToLocation = new();
    public Dictionary<Location, Dictionary<Location, Route>> SourceToDestinationToRoute = new();
    int spacecraftCreated = 0;

    [ObservableProperty]
    PrepareManeuverState? prepareManeuverState; 

    public GameState()
    {
        // Earth
        Location Earth = new(){Name="Earth", IsHome=true, Explored=true, HasExploreValue=true, Surveyed=true, Entered=true, HumanEntered=true, HumanMaintained=true};
        Location SuborbitalFlight = new(){Name="Suborbital Flight", Type=LocationType.Planet};
        Location EarthOrbit = new(){Name="Earth Orbit", Type=LocationType.Orbit, HasExploreValue=true, Surveyed=true};

        // Moon
        Location Moon = new(){Name="Moon", Type=LocationType.Planet, HasExploreValue=true};
        Location LunarOrbit = new(){Name="Lunar Orbit", Type=LocationType.Orbit};
        Location LunarFlyBy = new(){Name="Lunar Fly-By", Type=LocationType.FlyBy};

        // Mars
        Location Mars = new(){Name="Mars", Type=LocationType.Planet, HasExploreValue=true};
        Location MarsOrbit = new(){Name="Mars Orbit", Type=LocationType.Orbit};
        Location MarsFlyBy = new(){Name="Mars Fly-By", Type=LocationType.FlyBy};
        Location Phobos = new(){Name="Phobos", Type=LocationType.Planet};

        // Venus
        Location Venus = new(){Name="Venus", Type=LocationType.Planet, HasExploreValue=true};
        Location VenusOrbit = new(){Name="Venus Orbit", Type=LocationType.Orbit};
        Location VenusFlyBy = new(){Name="Venus Fly-By", Type=LocationType.FlyBy};

        // Ceres
        Location Ceres = new(){Name="Ceres", Type=LocationType.Planet, HasExploreValue=true};

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

            Route.Make(EarthOrbit, Earth, 0, isLanding: true, isReEntry: true),
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

    public bool IsGameEnd()
    {
        return Year >= EndYear;
    }

    // IEnumerable<Component> IterComponents()
    // {
    //     foreach(var component in Inventory)
    //         yield return component;
    //     foreach(var spacecraft in Spacecrafts)
    //     {
    //         foreach(var component in spacecraft.Components)
    //             yield return component;
    //     }
    // }

    public void NextTurnHousekeeping()
    {
        Year++; // TODO: Handle Game End

        // Restore funds
        Funds = 25;

        foreach(var component in Inventory)
        {
            component.Used = false;
            component.Damaged = false;
        }

        foreach(var spacecraft in Spacecrafts.ToList())
        {
            if(spacecraft.Location.IsHome)
            {
                foreach(var component in spacecraft.Components)
                {
                    component.Used = false;
                    component.Damaged = false;
                }
            }
            else // Spacecraft in non-home location
            {
                foreach(var component in spacecraft.Components)
                {
                    component.Used = false;
                }

                // non-damaged Doctor restore other damaged astronaut
                var hasEffDoctor = spacecraft.Components.Any(c => c.Def.IsDoctor && !c.Damaged);
                foreach(var astronaut in spacecraft.IterAstronaut().ToList())
                {
                    if(astronaut.Damaged)
                    {
                        if(hasEffDoctor)
                        {
                            astronaut.Damaged = false;
                        }
                        else
                        {
                            DestroyComponent(spacecraft, astronaut);
                        }
                    }
                }
            }

            // Fly-By spacecraft without current route lost
            if(spacecraft.Location.Type == LocationType.FlyBy && spacecraft.MoveState == null)
            {
                DestroySpacecraft(spacecraft);
            }

            // Update MoveState
            if(spacecraft.MoveState != null)
            {
                spacecraft.MoveState.ElapsedYears += 1;
                if(spacecraft.MoveState.ElapsedYears >= spacecraft.MoveState.RequiredYears) // Complete the movement
                {
                    // spacecraft.Location = spacecraft.MoveState.Route.Destination;
                    // MoveTo(spacecraft, spacecraft.MoveState.Route.Destination);
                    MoveThrough(spacecraft, spacecraft.MoveState.Route);
                    spacecraft.MoveState = null;
                }
            }
        }

        // IsLifeSupportEnabled kill
        if(!IsLifeSupportEnabled)
        {
            foreach(var spacecraft in Spacecrafts)
            {
                if(!spacecraft.Location.IsHome)
                {
                    var astronauts = spacecraft.Components.Where(c => c.Def.Type == ComponentType.Astronaut).ToList();
                    if(astronauts.Count > 0)
                    {
                        Log($"No Life Support Research, all astronaut in {spacecraft} killed");
                        foreach(var astronaut in astronauts)
                        {
                            DestroyComponent(spacecraft, astronaut);
                        }
                    }
                }
            }
        }

        // Supply Consumption
        foreach(var spacecraft in Spacecrafts)
        {
            var astronauts = spacecraft.Components.Where(c => c.Def.Type == ComponentType.Astronaut).ToList();
            if(astronauts.Count > 0)
            {
                var supplyConsumption = astronauts.Count * 0.2;
                var currentSupply = spacecraft.Components.Count(c => c.Def.Type == ComponentType.Supply);
                if(supplyConsumption > currentSupply)
                {
                    Log($"Out-of-supply, all astronaut in {spacecraft} killed");
                    foreach(var astronaut in astronauts)
                    {
                        DestroyComponent(spacecraft, astronaut);
                    }
                }
                var usedSupply = (int)Math.Ceiling(Math.Min(supplyConsumption, currentSupply));
                for(int i=0; i<usedSupply; i++)
                {
                    var supply = spacecraft.Components.FirstOrDefault(c => c.Def.Type == ComponentType.Supply);
                    if(supply == null)
                        break;
                    DestroyComponent(spacecraft, supply);
                }
            }
        }

        // Human Maintain mission housekeeping
        foreach(var spacecraft in Spacecrafts)
        {
            if(spacecraft.Location.HasExploreValue && !spacecraft.Location.HumanMaintained && spacecraft.Components.Any(c => c.Def.Type == ComponentType.Astronaut))
            {
                spacecraft.Location.HumanMaintained = true;
                Score += 4;
                Log($"Human maintained in {spacecraft.Location.Name}, +4 score");
            }
        }
    }

    void DestroyComponent(Spacecraft spacecraft, Component component)
    {
        if(component.Def.Type == ComponentType.Astronaut)
        {
            Score -= 2;
            Log($"{component.Def.Name} Kill, -2 score");
        }
        spacecraft.Components.Remove(component);
    }

    void DestroySpacecraft(Spacecraft spacecraft) // rocket fatal failure or fly-by lost
    {
        foreach(var component in spacecraft.Components.ToList())
        {
            DestroyComponent(spacecraft, component);
        }

        Spacecrafts.Remove(spacecraft);
    }

    public void DisassembleSpacecraft(Spacecraft spacecraft)
    {
        if(!spacecraft.Location.IsHome)
            return;
        
        Spacecrafts.Remove(spacecraft);
        // Inventory.AddRange(spacecraft.Components);
        foreach(var component in spacecraft.Components)
        {
            Inventory.Add(component);
        }
    }

    public void AssembleSpacecraft(IEnumerable<Component> _components)
    {
        var components = _components.ToList();

        var inventorySet = Inventory.ToHashSet();
        if(components.Any(c => !inventorySet.Contains(c)))
            return;

        foreach(var component in components)
        {
            Inventory.Remove(component);
        }
        CreateSpacecraft(components, Locations.First(loc => loc.IsHome));
    }

    // void DoManeuver(Spacecraft spacecraft, IEnumerable<Component> usedComponents) // Move to destination immediatelly if years == 0 or create a MoveState to move it in later time
    // {
    //     // TODO: rocket or thruster check
    //     // Used rocket => removed & 
    // }

    public bool CanUsedInManeuver(Component thruster) => !(thruster.Damaged || thruster.Used || thruster.Def.Thrust == 0 || PrepareManeuverState == null || !PrepareManeuverState.Spacecraft.Components.Contains(thruster));

    public void UseThrusterForManeuver(Component thruster, int targetYears)
    {
        if(PrepareManeuverState == null)
            return;
        if(!CanUsedInManeuver(thruster))
            return;

        if(thruster.Def.IsOneshot)
        {
            PrepareManeuverState.Spacecraft.Components.Remove(thruster);
        }
        else
        {
            thruster.Used = true;
        }
        
        PrepareManeuverState.AppliedThrust += thruster.Def.Thrust * Math.Max(1, targetYears);
    }

    public void UseThrusterForManeuver(Component thruster) => UseThrusterForManeuver(thruster, PrepareManeuverState?.TargetYears ?? 0);

    public void StartPrepareManeuver(Spacecraft spacecraft, Route route, int targetYears)
    {
        PrepareManeuverState = new()
        {
            Spacecraft=spacecraft,
            Route=route,
            StartMass=spacecraft.Mass,
            RequiredThrust=route.Difficulty * spacecraft.Mass,
            TargetYears=targetYears,
            AppliedThrust=0
        };
    }

    public void CancelPrepareManeuver()
    {
        PrepareManeuverState = null;
    }

    public bool PrepareManeuverBeginable()
    {
        if(PrepareManeuverState == null)
            return false;
        return PrepareManeuverState.AppliedThrust >= PrepareManeuverState.StartMass * PrepareManeuverState.Route.Difficulty;
    }

    public void MoveThrough(Spacecraft spacecraft, Route route)
    {
        if(route.IsLanding && !IsLandingEnabled)
        {
            Log($"Landing not researched, {spacecraft.Name} destroyed");
            DestroySpacecraft(spacecraft);
            return;
        }
        if(route.IsReEntry)
        {
            var isReEntryEnabled = IsReEntryEnabled;
            var destroyedComponentsInReEntry = spacecraft.Components.Where(c => c.Def.Type == ComponentType.Capsule && (!isReEntryEnabled || !c.Def.HasHeatShield)).ToList();
            foreach(var comp in destroyedComponentsInReEntry)
                DestroyComponent(spacecraft, comp);
            // TODO: Enforce capacity limit
        }

        spacecraft.Location = route.Destination;
        MoveToHousekeepingForMission(spacecraft, route.Destination);
    }

    void MoveToHousekeepingForMission(Spacecraft spacecraft, Location destination)
    {
        if(destination.HasExploreValue)
        {
            if(!destination.Entered && spacecraft.Components.Any(c => c.Def.Surveyable))
            {
                destination.Entered = true;
                Log($"Enter {destination.Name}, +4 score");
                Score += 4;
            }
            if(!destination.HumanEntered && spacecraft.Components.Any(c => c.Def.Type == ComponentType.Astronaut))
            {
                destination.HumanEntered = true;
                Log($"Human enter {destination.Name}, +4 score");
                Score += 4;
            }
        }

        if(IsSurveyingEnabled && spacecraft.Components.Any(c => !c.Damaged && c.Def.Surveyable))
        {
            foreach(var (dest, route) in SourceToDestinationToRoute[destination])
            {
                if(dest.HasExploreValue && !dest.Surveyed)
                {
                    dest.Surveyed = true;
                    Score += 4;
                    Log($"Survey {destination.Name}, +4 score");
                }
            }
        }
    }

    public void BeginPrepareManeuver()
    {
        if(PrepareManeuverState == null)
            return;
        if(!PrepareManeuverBeginable())
            return;
        
        if(PrepareManeuverState.Route.Years == 0)
        {
            // PrepareManeuverState.Spacecraft.Location = PrepareManeuverState.Route.Destination;
            // MoveThrough(PrepareManeuverState.Spacecraft, PrepareManeuverState.Route.Destination);
            MoveThrough(PrepareManeuverState.Spacecraft, PrepareManeuverState.Route);
        }
        else
        {
            PrepareManeuverState.Spacecraft.MoveState = new()
            {
                Route=PrepareManeuverState.Route,
                ElapsedYears=0,
                RequiredYears=PrepareManeuverState.TargetYears
            };

            PrepareManeuverState = null;
        }
    }

    Spacecraft CreateSpacecraft(ICollection<Component> components, Location location)
    {
        var spacecraft = new Spacecraft()
        {
            Name = $"Spacecraft #{spacecraftCreated+1}",
            // Components=components.ToList(),
            Location = location
        };
        foreach(var comp in components)
            spacecraft.Components.Add(comp);

        Spacecrafts.Add(spacecraft);
        
        spacecraftCreated++;
        
        return spacecraft;
    }

    public void SeparateSpacecraft(Spacecraft oriSpacecraft, ICollection<Component> components)
    {
        // TODO: Rendezvous research & related check

        if(components.Any(c => !oriSpacecraft.Components.Contains(c)))
            return;

        foreach(var component in components)
        {
            oriSpacecraft.Components.Remove(component);
        }

        CreateSpacecraft(components, oriSpacecraft.Location);
    }

    public void DockSpacecraft(Spacecraft keepedSpacecraft, Spacecraft mergedSpacecraft)
    {
        // TODO: Rendezvous research & related check

        var components = mergedSpacecraft.Components.ToList();
        foreach(var component in components)
        {
            mergedSpacecraft.Components.Remove(component);
            keepedSpacecraft.Components.Add(component);
        }
        Spacecrafts.Remove(mergedSpacecraft);
    }

    public bool CanBuyComponent(ComponentDef componentDef)
    {
        // TODO: Add detailed explaination for invalid buy here?
        if(componentDef.Price > Funds)
            return false;
        if(componentDef.Tag != BuildableTag.None && !Researches.Any(r => r.Researched && r.Unlocks.Contains(componentDef.Tag)))
            return false;
        return true;
    }

    public void BuyComponent(ComponentDef componentDef)
    {
        if(!CanBuyComponent(componentDef))
            return;
        Funds -= componentDef.Price;
        Inventory.Add(new()
        {
            Def=componentDef,
        });
    }

    public bool IsResearchable(Research research)
    {
        return Funds >= research.Price && !research.Researched;
    }

    public void DoResearch(Research research)
    {
        if(!IsResearchable(research))
            return;
        
        Funds -= research.Price;
        research.Researched = true;
    }

    public void DoHeal(Spacecraft? spacecraft)
    {
        if(spacecraft == null)
            return;

        if(!CanDoHeal(spacecraft))
            return;

        foreach(var component in spacecraft.Components)
        {
            if(component.Def.Type == ComponentType.Astronaut)
            {
                component.Damaged = false;
            }
        }
    }

    public bool CanDoHeal(Spacecraft? spacecraft)
    {
        if(spacecraft == null)
            return false;

        if(!spacecraft.Components.Any(c => c.Def.Type == ComponentType.Astronaut && c.Damaged))
            return false;
        
        if(!spacecraft.Components.Any(c => !c.Damaged && c.Def.IsDoctor))
            return false;

        return true;
    }

    public void DoRepair(Spacecraft? spacecraft)
    {
        if(spacecraft == null || !CanDoRepair(spacecraft))
            return;

        var components = spacecraft.Components;

        foreach(var component in components)
        {
            if(component.Def.Type != ComponentType.Astronaut)
            {
                component.Damaged = false;
            }
        }

        var supply = components.First(c => c.Def.Type == ComponentType.Supply);
        DestroyComponent(spacecraft, supply);
    }

    public bool CanDoRepair(Spacecraft? spacecraft)
    {
        if(spacecraft == null)
            return false;
        var comps = spacecraft.Components;

        if(!comps.Any(c => c.Def.Type != ComponentType.Astronaut && c.Damaged))
            return false;

        if(!comps.Any(c => !c.Damaged && c.Def.IsMechanic))
            return false;

        if(!comps.Any(c => c.Def.Type == ComponentType.Supply))
            return false;

        return true;
    }

    public bool TryGetManeuverRoute(Spacecraft spacecraft, Location location, out Route? route)
    {
        if(SourceToDestinationToRoute.TryGetValue(spacecraft.Location, out var destToRoute))
        {
            if(destToRoute.TryGetValue(location, out route))
            {
                return true;
            }
        }
        route = null;
        return false;
    }

    public bool IsRendezvousEnabled => Researches.Any(r => r.EnableRendezvous && r.Researched);
    public bool IsReEntryEnabled => Researches.Any(r => r.EnableReEntry && r.Researched);
    public bool IsLandingEnabled => Researches.Any(r => r.EnableLanding && r.Researched);
    public bool IsLifeSupportEnabled => Researches.Any(r => r.EnableLifeSupport && r.Researched);
    public bool IsSurveyingEnabled => Researches.Any(r => r.EnableSurveying && r.Researched);

    public void DoDock(Spacecraft spacecraft)
    {
        // Merge all other now
        var otherSpacecrafts = Spacecrafts.Where(s => s != spacecraft && s.Location == spacecraft.Location).ToList();
        foreach(var otherSpacecraft in otherSpacecrafts)
        {
            foreach(var component in otherSpacecraft.Components)
            {
                spacecraft.Components.Add(component);
            }
            Spacecrafts.Remove(otherSpacecraft);
        }
    }
}