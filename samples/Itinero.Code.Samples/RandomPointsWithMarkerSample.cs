using System;
using System.Collections.Generic;
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
            return new Layer("pointLayer")
            {
                DataSource = new MemoryProvider(GenerateRandomPoints(envelope, count)),
                Style = CreateBitmapStyle("Itinero.Samples.Data.Images.marker.png")
            };
        }

        public static SymbolStyle CreateBitmapStyle(string embeddedResourcePath)
        {
            var bitmapId = GetBitmapIdForEmbeddedResource(embeddedResourcePath);
            return new SymbolStyle { BitmapId = bitmapId };
        }

        public static int GetBitmapIdForEmbeddedResource(string imagePath)
        {
            var image = Assembly.GetExecutingAssembly().GetManifestResourceStream(imagePath);
            var bitmapId = BitmapRegistry.Instance.Register(image);
            return bitmapId;
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
