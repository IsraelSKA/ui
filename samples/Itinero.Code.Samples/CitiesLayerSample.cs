using System;
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
            var imageStream = GetResourceStream(typeof(RandomPointsWithMarkerSample), 
                "Itinero.Code.Samples.Images.marker.png");

            return new MemoryLayer
            {
                DataSource = new MemoryProvider(GetFeatures()),
                Style = new SymbolStyle
                {
                    BitmapId = BitmapRegistry.Instance.Register(imageStream),
                    SymbolOffset = new Offset {  X = 0, Y = 40 }
                }
            };
        }

        private static IEnumerable<Geoname> GetCities()
        {
            var jsonStream = GetResourceStream(typeof(CitiesLayerSample), "Itinero.Code.Samples.Data.cities.json");

            using (var reader = new StreamReader(jsonStream))
            {
                return JsonConvert.DeserializeObject<Rootobject>(reader.ReadToEnd()).geonames;
            }
        }
        
        public static IEnumerable<IFeature> GetFeatures()
        {
             return GetCities().Select(c => new Feature {
                 Geometry = SphericalMercator.FromLonLat(c.lng, c.lat)
             });
        }

        private static Stream GetResourceStream(Type t, string embeddedResourcePath)
        {
            var assembly = t.GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }
    }
}
