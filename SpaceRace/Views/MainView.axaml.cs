
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using SpaceRace.ViewModels;
using Svg.Skia;

namespace SpaceRace.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    void Map_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Console.WriteLine($"sender={sender}, DataContext={DataContext}, e={e}");
    
        if(sender is not Avalonia.Svg.Skia.Svg svg || DataContext is not MainViewModel vm)
        {
            return;
        }

        // Support touch pointer?
        if(!e.GetCurrentPoint(svg).Properties.IsLeftButtonPressed)
            return;

        var position = e.GetPosition(svg);

        foreach(var hit in svg.HitTestElements(position))
        {
            // Console.WriteLine($"hit={hit}");
            hit.CustomAttributes.TryGetValue("http://www.inkscape.org/namespaces/inkscape:label", out var name);
            // Console.WriteLine($"name={name}");
            if(name != null && vm.Session.State.NameToLocation.TryGetValue(name, out var location))
            {
                vm.SelectLocation(location);                
            }
        }
    }
}