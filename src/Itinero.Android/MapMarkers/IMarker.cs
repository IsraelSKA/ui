using Android.Content;
using Android.Views;
using Itinero.Core.Marker;
using Mapsui.Geometries;

namespace Itinero.Android.MapMarkers
{
    public interface IMarker
    {
        Point GeoPosition { get; set; }

        HorizontalAlignmentType HorizontalAlignment { get; set; }

        VerticalAlignmentType VerticalAlignment { get; set; }
        
        View View { get; }
    }
}
