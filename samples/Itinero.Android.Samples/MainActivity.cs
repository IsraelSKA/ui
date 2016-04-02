using Android.App;
using Android.OS;
using Itinero.Code.Samples;

namespace Itinero.Android.Samples
{
    [Activity(Label = "Itinero.Android.Samples", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var mapControl = FindViewById<MapControl>(Resource.Id.mapcontrol);

            mapControl.Map.Layers.Add(OsmTilesSample.CreateLayer());
            mapControl.Map.Layers.Add(LineStringSample.CreateLayer());
            mapControl.Map.Layers.Add(CitiesLayerSample.CreateLayer());
            
            mapControl.ShowCurrentLocation = true;
            
            mapControl.Map.Viewport.RenderResolutionMultiplier = 2;
        }
    }
}

