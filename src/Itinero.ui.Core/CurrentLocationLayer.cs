using System;
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

namespace Itinero.ui.Core
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
            locator.StartListeningAsync(IntervalInSeconds * 1000, 100);
        }

        public int IntervalInSeconds { get; set; } = 60;

        public void StartListening()
        {
            CrossGeolocator.Current.StartListeningAsync(IntervalInSeconds * 1000, 100);
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
            var stream = LoadEmbeddedResource("Itinero.ui.Core.Images.current_location.png", typeof(CurrentLocationLayer));
            return new SymbolStyle { BitmapId = BitmapRegistry.Instance.Register(stream) };
        }
        
        private static Stream LoadEmbeddedResource(string embeddedResourcePath, Type type)
        {
            var assembly = type.GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }
    }
}
