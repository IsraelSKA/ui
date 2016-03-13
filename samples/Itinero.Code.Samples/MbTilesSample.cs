using BruTile;
using Mapsui.Layers;
using SQLite.Net;
using SQLite.Net.Interop;

namespace Itinero.Code.Samples
{
    public static class MbTilesSample
    {
        public static ILayer CreateLayer(ISQLitePlatform platform, string path)
        {
            MbTilesTileSource.SetPlatform(platform);
            var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false));
            return new TileLayer(tileSource) { Name = "MbTiles" };
        }
    }
}
