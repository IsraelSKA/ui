using Android.App;
using Android.OS;
using BruTile.Predefined;
using Itinero.Android;
using Itinero.Code.Samples;
using Mapsui.Layers;

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
    }
}

