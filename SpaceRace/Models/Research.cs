using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace SpaceRace.Models;

public partial class Research : ObservableObject
{
    public string Name {get; init;} = "";
    public int Price{get;set;} = 10;

    [ObservableProperty]
    // [NotifyPropertyChangedFor(nameof(ResearchedStatus))]
    public bool researched;

    public HashSet<BuildableTag> Unlocks = new();
    public bool EnableRendezvous = false;
    public bool EnableReEntry = false;
    public bool EnableLanding = false;
    public bool EnableLifeSupport = false;
    public bool EnableSurveying = false;

    public string Description
    {
        get
        {
            var words = new List<string>();
            if(Unlocks.Count > 0)
            {
                words.Add($"Unlocks: {string.Join(", ", Unlocks)}");
            }
            var enables = new List<string>();
            if(EnableRendezvous) enables.Add("Rendezvous");
            if(EnableReEntry) enables.Add("Re-Entry");
            if(EnableLanding) enables.Add("Landing");
            if(EnableLifeSupport) enables.Add("Life Support");
            if(EnableSurveying) enables.Add("Surveying");
            if(enables.Count > 0)
            {
                words.Add($"Enables: {string.Join(", ", enables)}");
            }
            return string.Join(",", words);
        }
    }

    // public string ResearchedStatus => Researched ? "Researched" : "Not Researched";
}