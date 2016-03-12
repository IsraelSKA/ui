using BruTile.Predefined;
using Mapsui.Layers;

namespace Itinero.Code.Samples
{
    public static class OsmTilesSample
    {
        public static ILayer CreateLayer()
        {
            return new TileLayer(KnownTileSources.Create()) {Name = "OSM"};
        }
    }
}
