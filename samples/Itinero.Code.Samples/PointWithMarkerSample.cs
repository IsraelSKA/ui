using System.Reflection;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;

namespace Itinero.Code.Samples
{
    public static class PointWithMarkerSample
    {
        public static ILayer CreateLayer()
        {
            return new Layer("Points with markers")
            {
                DataSource = new MemoryProvider(CreateBitmapPoint())
            };
        }

        private static Feature CreateBitmapPoint()
        {
            var feature = new Feature {Geometry = new Point(0, 1000000)};
            feature.Styles.Add(CreateBitmapStyle("Itinero.Samples.Data.Images.loc.png"));
            return feature;
        }

        public static SymbolStyle CreateBitmapStyle(string embeddedResourcePath)
        {
            var bitmapId = GetBitmapIdForEmbeddedResource(embeddedResourcePath);
            return new SymbolStyle {BitmapId = bitmapId};
        }

        public static int GetBitmapIdForEmbeddedResource(string imagePath)
        {
            var image = Assembly.GetExecutingAssembly().GetManifestResourceStream(imagePath);
            var bitmapId = BitmapRegistry.Instance.Register(image);
            return bitmapId;
        }
    }
}