using Android.App;
using Android.OS;
using Itinero.Code.Samples;
using Mapsui.UI.Android;

namespace Itinero.Android.Samples
{
    [Activity(Label = "Itinero.Android.Samples", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var mapControl = FindViewById<MapControl>(Resource.Id.mapcontrol);

            mapControl.Map.Layers.Add(OsmTilesSample.CreateLayer());
            mapControl.Map.Layers.Add(LineStringSample.CreateLayer());
            mapControl.Map.Layers.Add(RandomPointsWithMarkerSample.CreateLayer(mapControl.Map.Envelope));
            mapControl.Map.Layers.Add(PointWithMarkerSample.CreateLayer());

            mapControl.Map.Viewport.RenderResolutionMultiplier = 2;
        }
    }
}

