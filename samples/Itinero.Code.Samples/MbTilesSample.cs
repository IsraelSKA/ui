using BruTile;
using Mapsui.Layers;
using SQLite.Net;
using SQLite.Net.Interop;

namespace Itinero.Code.Samples
{
    public static class MbTilesSample
    {
        public static ILayer CreateLayer(ISQLitePlatform platform)
        {
            MbTilesTileSource.SetPlatform(platform);
            const string path = ".\\Data\\test.mbtiles";
            var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false));
            return new TileLayer(tileSource) { Name = "MbTiles" };
        }
    }
}
