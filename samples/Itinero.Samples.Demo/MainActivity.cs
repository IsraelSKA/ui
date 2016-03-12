using Android.App;
using Android.OS;
using BruTile.Predefined;
using Itinero.Android;
using Itinero.Samples.Data;
using Mapsui.Layers;
using Mapsui.Styles;

namespace Itinero.Samples.Demo
{
    [Activity(Label = "Itinero.Samples.Demo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var mapControl = FindViewById<MapControl>(Resource.Id.mapcontrol);

            mapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OSM" });
            mapControl.Map.Layers.Add(LineStringSample.CreateLineStringLayer(LineStringSample.CreateLineStringStyle()));
            mapControl.Map.Layers.Add(PointLayerSample.CreateRandomPointLayer(mapControl.Map.Envelope,
                style: PointLayerSample.CreateBitmapStyle("Itinero.Samples.Data.Images.marker.png")));
            mapControl.Map.Layers.Add(PointLayerSample.CreateBitmapPointLayer());

            mapControl.Map.Viewport.RenderResolutionMultiplier = 2;
        }
        private static TileLayer CreateTileLayer()
        {
            return new TileLayer(KnownTileSources.Create()) { Name = "OSM" };
        }

        private static IStyle CreatePointLayerStyle()
        {
            return new SymbolStyle
            {
                SymbolScale = 1,
                Fill = new Brush(Color.Cyan),
                Outline = { Color = Color.White, Width = 4 },
                Line = null
            };
        }

        private static IStyle CreateLineStringStyle()
        {
            return new VectorStyle
            {
                Fill = null,
                Outline = null,
                Line = { Color = Color.Red, Width = 4 }
            };
        }
    }
}

