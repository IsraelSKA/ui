using Android.Views;
using Itinero.ui.Core.Marker;
using Mapsui.Geometries;

namespace Itinero.ui.MapMarkers
{
    public interface IMarker
    {
        Point GeoPosition { get; set; }

        HorizontalAlignmentType HorizontalAlignment { get; set; }

        VerticalAlignmentType VerticalAlignment { get; set; }
        
        View View { get; }
    }
}
