using Avalonia.Controls;

namespace SpaceRace.Views;

public partial class LocationMarker: UserControl
{
    public LocationMarker()
    {
        InitializeComponent();
    }

    public void UpdateState(bool selected, bool selectedNeighbor, int count)
    {
        SelectedEllipse.IsVisible = selected;
        SelectedNeighborEllipse.IsVisible = selectedNeighbor;

        CountBadge.IsVisible = count > 0;
        CountText.Text = count.ToString();
    }
}