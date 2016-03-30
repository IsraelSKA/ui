using Android.Content;
using Android.Util;
using Android.Widget;
using Mapsui;

namespace Itinero.Android
{
    public sealed class MapViewOpenTK : FrameLayout
    {
        private MapControl _mapControl;
        public MapViewOpenTK(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _mapControl = new MapControl(Context, attrs);
            AddView(_mapControl);
        }

        public Map Map { get { return _mapControl.Map;  } }
    }
}