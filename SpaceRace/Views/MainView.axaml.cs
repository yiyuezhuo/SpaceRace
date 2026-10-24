
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using SpaceRace.ViewModels;
using Svg.Skia;

namespace SpaceRace.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        _mapRefreshTimer.Tick += (_,_) => RefreshLocationMarkers();

        _mapRefreshTimer.Start();
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
            hit.CustomAttributes.TryGetValue(inkscapeLabelKey, out var name);
            // Console.WriteLine($"name={name}");
            if(name != null && vm.Session.State.NameToLocation.TryGetValue(name, out var location))
            {
                vm.SelectLocation(location);                
            }
        }
    }

    Dictionary<Models.Location, LocationMarker> _markers = new();

    void AddLocationMarker(Models.Location location, double x, double y)
    {
        var marker = new LocationMarker();

        Canvas.SetLeft(marker, x - 6);
        Canvas.SetTop(marker, y - 6);

        MapOverlay.Children.Add(marker);
        _markers.Add(location, marker);
    }

    const string inkscapeLabelKey = "http://www.inkscape.org/namespaces/inkscape:label";

    bool locationMarkersInitialized = false;

    void EnsureLocationMarkersInitialized()
    {
        if(locationMarkersInitialized || DataContext is not MainViewModel vm)
            return;

        var svg = MapSvg.SkSvg;
        var picture = MapSvg.Picture;

        if(svg is null || picture is null || !svg.TryEnsureRetainedSceneGraph(out var scene) || scene is null)
            return;

        var size = MapSvg.Bounds.Size; // Svg component size in the shown UI
        var source = picture.CullRect; // Svg file's internal view rect, EX. (left, top, width, height) = (0, 0, 150, 110)

        if(size.Width <= 0 || source.Height <= 0)
            return;

        var scale = Math.Min(size.Width / source.Width, size.Height / source.Height);

        var offsetX = (size.Width - source.Width * scale) / 2; // used to handle empty area when aspect ratio are not meet
        var offsetY = (size.Height - source.Height * scale) / 2;

        foreach(var node in scene.Traverse())
        {
            var element = node.Element;

            if(element is null || !element.CustomAttributes.TryGetValue(inkscapeLabelKey, out var name) ||
                !vm.Session.State.NameToLocation.TryGetValue(name, out var location))
                continue;
            
            var bounds = node.TransformedBounds; // Svg's file's node's transform bounds
            
            var centerX = (bounds.Left + bounds.Right) / 2; // svg's file's node's "resolved" center
            var centerY = (bounds.Top + bounds.Bottom) / 2;

            var point = new Point( // canvas's 
                offsetX + (centerX - source.Left) * scale,
                offsetY + (centerY - source.Top) * scale
            );

            // var overlayPoint = MapSvg.TranslatePoint(point, MapOverlay);
            var overlayPoint = point; // They're the same point at the current UI structure

            AddLocationMarker(location, overlayPoint.X, overlayPoint.Y);
        }

        locationMarkersInitialized = true;
    }

    void RefreshLocationMarkers()
    {
        // Console.WriteLine("Try Refresh");

        EnsureLocationMarkersInitialized();

        if(locationMarkersInitialized && DataContext is MainViewModel vm)
        {
            var locToCount = vm.Session.State.Spacecrafts.GroupBy(s => s.Location).ToDictionary(g => g.Key, g => g.Count());

            Dictionary<Models.Location, Models.Route>? destToRoute = null;
            if(vm.SelectedLocation != null)
            {
                vm.Session.State.SourceToDestinationToRoute.TryGetValue(vm.SelectedLocation, out destToRoute);
            }

            foreach(var (loc, locationMarker) in _markers)
            {
                locationMarker.UpdateState(
                    loc == vm.SelectedLocation,
                    vm.SelectState == MainViewModelSelectState.SelectManeuverTarget && destToRoute != null && destToRoute.ContainsKey(loc),
                    locToCount.GetValueOrDefault(loc)
                );
            }

            // Console.WriteLine("Refresh");
        }
    }

    DispatcherTimer _mapRefreshTimer = new(){Interval=TimeSpan.FromMilliseconds(16)}; // Target 60 fps
}