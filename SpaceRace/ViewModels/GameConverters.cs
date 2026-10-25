using Avalonia.Data.Converters;

namespace SpaceRace.ViewModels;

public static class GameConverters
{
    public static FuncValueConverter<bool, string> ResearchStatus{get;} = new FuncValueConverter<bool, string>(
        researched => researched? "Researched" : "Not Researched"
    );

    // public static FuncValueConverter<int, string>
}