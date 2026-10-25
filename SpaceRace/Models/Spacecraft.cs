using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpaceRace.Models;

public class MoveState
{
    public required Route Route;
    public int ElapsedYears;
    public int RequiredYears;
}

public partial class Spacecraft : ObservableObject
{
    public string Name{get;set;} = "Spacecraft #0";
    public ObservableCollection<Component> Components{get;} = new();
    public IEnumerable<Component> Thrusters => Components.Where(c => c.Def.Thrust > 0);

    public Spacecraft()
    {
        Components.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(Thrusters));
            OnPropertyChanged(nameof(Mass));
        };
    }

    public required Location Location{get;set;}
    public MoveState? MoveState{get;set;}

    public int Mass => Components.Sum(c => c.Def.Mass);

    public IEnumerable<Component> IterAstronaut() => Components.Where(c => c.Def.Type == ComponentType.Astronaut);
}
