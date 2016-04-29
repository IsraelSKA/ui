using System;
using System.IO;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Itinero.ui.Core.Marker;
using Point = Mapsui.Geometries.Point;

namespace Itinero.ui.MapMarkers
{
    public class Marker : IMarker
    {
        //private readonly Func<Context, View> _createView;
        public Marker(Context context)
        {
            View = CreateDefaultView(context);
            VerticalAlignment = VerticalAlignmentType.Bottom;
        }

        private static ImageButton CreateDefaultView(Context context)
        {
            var imageStream = LoadEmbeddedResource("Itinero.ui.Core.Images.marker.png", typeof(VerticalAlignmentType));
            
            return new ImageButton(context)
            {
                Background = new BitmapDrawable(BitmapFactory.DecodeStream(imageStream)),
            };
        }

        public HorizontalAlignmentType HorizontalAlignment { get; set; }

        public VerticalAlignmentType VerticalAlignment { get; set; }

        public Point GeoPosition { get; set; }

        public View View { get; set; }

        private static Stream LoadEmbeddedResource(string embeddedResourcePath, Type type)
        {
            var assembly = type.Assembly;
            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }
    }
}