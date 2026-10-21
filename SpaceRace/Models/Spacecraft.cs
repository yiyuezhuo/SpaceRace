using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace SpaceRace.Models;

public class Spacecraft
{
    public string Name{get;set;} = "";
    public List<Component> Components{get;} = new();
    public required Location Location{get;set;}

    public int Mass => Components.Sum(c => c.Def.Mass);
}
