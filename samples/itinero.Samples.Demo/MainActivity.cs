using Android.App;
using Android.OS;
using BruTile.Predefined;
using itinero.Android;
using itinero.Samples.Data;
using Mapsui.Layers;
using Mapsui.Styles;

namespace itinero.Samples.Demo
{
    [Activity(Label = "itinero.Samples.Demo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var mapControl = FindViewById<MapControl>(Resource.Id.mapcontrol);
            var tileLayer = CreateTileLayer();
            mapControl.Map.Layers.Add(tileLayer);
            mapControl.Map.Layers.Add(LineStringSample.CreateLineStringLayer(CreateLineStringStyle()));
            var pointLayer = PointLayerSample.CreateRandomPointLayer(mapControl.Map.Envelope);
            pointLayer.Style = CreatePointLayerStyle();
            mapControl.Map.Layers.Add(pointLayer);
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

