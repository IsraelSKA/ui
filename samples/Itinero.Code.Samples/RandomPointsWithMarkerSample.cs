using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;

namespace Itinero.Code.Samples
{
    public class RandomPointsWithMarkerSample
    {
        private static readonly Random Random = new Random(0);

        public static ILayer CreateLayer(BoundingBox envelope, int count = 25)
        {
            return new Layer("Random points")
            {
                DataSource = new MemoryProvider(GenerateRandomPoints(envelope, count)),
                Style = new SymbolStyle { BitmapId = BitmapRegistry.Instance.Register(GetImageStream()) }
            };
        }
        
        private static Stream GetImageStream()
        {
            var embeddedResourcePath = "Itinero.Samples.Data.Images.marker.png";
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourcePath);
        }

        public static IEnumerable<IGeometry> GenerateRandomPoints(BoundingBox box, int count = 25)
        {
            var result = new List<IGeometry>();
            for (var i = 0; i < count; i++)
            {
                result.Add(new Point(Random.NextDouble() * box.Width + box.Left, Random.NextDouble() * box.Height - (box.Height - box.Top)));
            }
            return result;
        }

    }
}
