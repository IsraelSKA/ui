using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace Itinero.Core
{
    public class CurrentLocationLayer : BaseLayer
    {
        private readonly Feature _deviceLocation;

        public CurrentLocationLayer()
        {
            CRS = "EPSG:3857";

            Style = CreateBitmapStyle();

            _deviceLocation = new Feature { Geometry = new Point()};

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            locator.PositionChanged += LocatorOnPositionChanged;
            locator.StartListeningAsync(5000, 100);
        }

        public void StartListening()
        {
            CrossGeolocator.Current.StartListeningAsync(5000, 100);
        }

        public void StopListening()
        {
            CrossGeolocator.Current.StopListeningAsync();
        }

        private void LocatorOnPositionChanged(object sender, PositionEventArgs positionEventArgs)
        {
            var position = positionEventArgs.Position;
            _deviceLocation.Geometry = SphericalMercator.FromLonLat(position.Longitude, position.Latitude);
            OnDataChanged(new DataChangedEventArgs());
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            if (!HasLocation) return new IFeature[0];
            return new[] {_deviceLocation};
        }

        private bool HasLocation
        {
            get { return _deviceLocation?.Geometry != null && 
                    !_deviceLocation.Geometry.IsEmpty() &&
                    CrossGeolocator.Current.IsListening &&
                    CrossGeolocator.Current.IsGeolocationAvailable &&
                    CrossGeolocator.Current.IsGeolocationEnabled; }
        }

        public override BoundingBox Envelope
        {
            get { return !HasLocation ? null : _deviceLocation.Geometry.GetBoundingBox(); }
        }

        public override void AbortFetch()
        {
            // no need to implement for CurrentLocationLayer
        }

        public override void ViewChanged(bool majorChange, BoundingBox extent, double resolution)
        {
            // no need to implement for CurrentLocationLayer
        }

        public override void ClearCache()
        {
            // no need to implement for CurrentLocationLayer
        }

        public static Style CreateBitmapStyle()
        {
            return new SymbolStyle { BitmapId = BitmapRegistry.Instance.Register(GetImageStream()) };
        }

        private static Stream GetImageStream()
        {
            var embeddedResourcePath = "Itinero.Core.Images.current_location.png";
            var assembly = typeof(CurrentLocationLayer).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }
    }
}
