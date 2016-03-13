using System.IO;
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
            return new Layer("Point")
            {
                DataSource = new MemoryProvider(CreateBitmapPoint())
            };
        }

        private static Feature CreateBitmapPoint()
        {
            var feature = new Feature {Geometry = new Point(0, 1000000)};
            feature.Styles.Add(new SymbolStyle { BitmapId = BitmapRegistry.Instance.Register(GetImageStream()) });
            return feature;
        }

        private static Stream GetImageStream()
        {
            var embeddedResourcePath = "Itinero.Samples.Data.Images.loc.png";
            var assembly = typeof (PointWithMarkerSample).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }
    }
}