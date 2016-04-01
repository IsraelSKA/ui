using System.Collections.Generic;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace Itinero.Core
{
    public class DeviceLocationLayer : BaseLayer
    {
        private readonly Feature _deviceLocation;

        public DeviceLocationLayer()
        {
            CRS = "EPSG:3857";

            _deviceLocation = new Feature { Geometry = new Point()};

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            locator.PositionChanged += LocatorOnPositionChanged;
            locator.StartListeningAsync(5000, 100);
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

        private bool HasLocation => _deviceLocation?.Geometry != null && !_deviceLocation.Geometry.IsEmpty();

        public override BoundingBox Envelope => !HasLocation ? null : _deviceLocation.Geometry.GetBoundingBox();

        public override void AbortFetch()
        {
            // no need to implement for DeviceLocationLayer
        }

        public override void ViewChanged(bool majorChange, BoundingBox extent, double resolution)
        {
            // no need to implement for DeviceLocationLayer
        }

        public override void ClearCache()
        {
            // no need to implement for DeviceLocationLayer
        }
    }
}
