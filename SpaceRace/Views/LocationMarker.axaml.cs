using Avalonia.Controls;

namespace SpaceRace.Views;

public partial class LocationMarker: UserControl
{
    public LocationMarker()
    {
        InitializeComponent();
    }

    public void UpdateState(bool selected, int count)
    {
        SelectedEllipse.IsVisible = selected;

        CountBadge.IsVisible = count > 0;
        CountText.Text = count.ToString();
    }
}