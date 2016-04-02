using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Newtonsoft.Json;

namespace Itinero.Code.Samples
{
    public static class CitiesLayerSample 
    {
        public static MemoryLayer CreateLayer()
        {
            return new MemoryLayer
            {
                DataSource = new MemoryProvider(GetFeatures()),
                Style = new SymbolStyle
                {
                    BitmapId = BitmapRegistry.Instance.Register(GetImageStream()),
                    SymbolOffset = new Offset {  X = 0, Y = 40 }
                }
            };
        }

        private static IEnumerable<Geoname> GetCities()
        {
            using (var reader = new StreamReader(GetResourceStream()))
            {
                return JsonConvert.DeserializeObject<Rootobject>(reader.ReadToEnd()).geonames;
            }
        }
        
        private static Stream GetResourceStream()
        {
            const string embeddedResourcePath = "Itinero.Code.Samples.Data.cities.json";
            var assembly = typeof(CitiesLayerSample).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }
        
        public static IEnumerable<IFeature> GetFeatures()
        {
             return GetCities().Select(c => new Feature {
                 Geometry = SphericalMercator.FromLonLat(c.lng, c.lat)
             });
        }

        private static Stream GetImageStream()
        {
            var embeddedResourcePath = "Itinero.Code.Samples.Images.marker.png";
            var assembly = typeof(RandomPointsWithMarkerSample).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }
    }
}
