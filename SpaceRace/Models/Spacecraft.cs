using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace SpaceRace.Models;

public class MoveState
{
    public required Route Route;
    public int ElapsedYears;
    public int RequiredYears;
}

public class Spacecraft
{
    public string Name{get;set;} = "Spacecraft #0";
    public List<Component> Components{get;set;} = new();
    public required Location Location{get;set;}
    public MoveState? MoveState{get;set;}

    public int Mass => Components.Sum(c => c.Def.Mass);

    public IEnumerable<Component> IterAstronaut() => Components.Where(c => c.Def.Type == ComponentType.Astronaut);
}
