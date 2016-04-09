using System;
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
            feature.Styles.Add(CreateBitmapStyle());
            return feature;
        }

        public static Style CreateBitmapStyle()
        {
            var stream = LoadEmbeddedResource("Itinero.Code.Samples.Images.loc.png", typeof (PointWithMarkerSample));
            return new SymbolStyle {BitmapId = BitmapRegistry.Instance.Register(stream) };
        }

        private static Stream LoadEmbeddedResource(string embeddedResourcePath, Type type)
        {
            var assembly = type.GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }
    }
}