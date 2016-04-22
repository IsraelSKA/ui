using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Mapsui.Geometries;

namespace Itinero.Android
{
    public class Marker : Button
    {
        public Marker(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public Marker(Context context) : base(context)
        {
        }

        public Marker(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public Marker(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        public Point GeoPosition { get; set; } = new Point(0, 0);

        public RelativeLayout.LayoutParams RelativeLayoutParams { get; } = new RelativeLayout.LayoutParams(100, 100);

        public void SetScreenPosition(Point screenPosition)
        {
            RelativeLayoutParams.SetMargins(0, 0, 0, 0);
            SetX((int)(screenPosition.X - 0.5 * Width));
            SetY((int)(screenPosition.Y - 0.5 * Height));
        }
    }
}