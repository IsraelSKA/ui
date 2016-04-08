using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Itinero.Core;
using Java.Lang;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Utilities;
using Math = System.Math;

namespace Itinero.Android
{
    public enum RestrictedPanMode
    {
        None,
        /// <summary>
        /// Restricts center of the viewport within Map.Extents or within MaxExtents when set
        /// </summary>
        KeepCenterWithinExtents,
        /// <summary>
        /// Restricts the whole viewport within Map.Extents or within MaxExtents when set
        /// </summary>
        KeepViewportWithinExtents,
    }

    public enum RestrictedZoomMode
    {
        None,
        KeepWithinResolutions,
    }

    public sealed class MapControl : FrameLayout
    {
        private readonly OpenTKSurface _openTKSurface;
        private readonly TouchHandler _touchHandler = new TouchHandler();
        private Map _map;
        private string _previousDataError = "";
        private bool _showCurrentLocation = true;
        private bool _viewportInitialized;

        public MapControl(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _openTKSurface = new OpenTKSurface(Context, attrs);
            AddView(_openTKSurface);
            CurrentLocationLayer.DataChanged += (sender, args) => Invalidate();

            Map = new Map();
            Touch += OnTouch;

            SetWillNotDraw(false);
        }
        
        public RestrictedPanMode RestrictedPanMode { get; set; } 
            = RestrictedPanMode.KeepViewportWithinExtents;

        public RestrictedZoomMode RestrictedZoomMode { get; set; }
            = RestrictedZoomMode.KeepWithinResolutions;

        /// <summary>
        /// Set this property in combination KeepCenterWithinExtents or KeepViewportWithinExtents.
        /// If RestrictedPanExtent is not set Map.Extent will be used as restricted extent.
        /// </summary>
        public BoundingBox RestrictedPanExtent { get; set; }

        /// <summary>
        /// Resolutions to keep the Map.Resolution within. The order of the two resolutions does not matter.
        /// </summary>
        public Tuple<double, double> RestrictedZoomResolutions;
        
        public bool ShowCurrentLocation
        {
            get { return _showCurrentLocation; }
            set
            {
                _showCurrentLocation = value;
                if (value) CurrentLocationLayer.StartListening();
                else CurrentLocationLayer.StopListening();
            }
        }

        public CurrentLocationLayer CurrentLocationLayer { get; set; } = new CurrentLocationLayer();

        public Map Map
        {
            get { return _map; }
            set
            {
                if (_map != null)
                {
                    var temp = _map;
                    _map = null;
                    temp.DataChanged -= MapDataChanged;
                    temp.PropertyChanged -= MapPropertyChanged;
                    temp.RefreshGraphics -= MapRefreshGraphics;
                    temp.Dispose();
                }

                _map = value;

                if (_map != null)
                {
                    _map.DataChanged += MapDataChanged;
                    _map.PropertyChanged += MapPropertyChanged;
                    _map.RefreshGraphics += MapRefreshGraphics;
                    _map.ViewChanged(true);
                }

                Invalidate();
            }
        }

        public event EventHandler<EventArgs> ViewportInitialized;

        private void MapRefreshGraphics(object sender, EventArgs e)
        {
            RunOnUiThread(Invalidate);
        }

        private void MapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Map.Envelope))
            {
                _map.ViewChanged(true);
                Invalidate();
            }
        }

        private bool TryInitializeViewport()
        {
            if (_viewportInitialized) return true;
            if (Math.Abs(Width - 0f) < Constants.Epsilon) return false;
            if (_map?.Envelope == null) return false;
            if (Math.Abs(_map.Envelope.Width - 0d) < Constants.Epsilon) return false;
            if (Math.Abs(_map.Envelope.Height - 0d) < Constants.Epsilon) return false;
            if (_map.Envelope.GetCentroid() == null) return false;

            if (double.IsNaN(_map.Viewport.Resolution))
                _map.Viewport.Resolution = _map.Envelope.Width/Width;
            if (double.IsNaN(_map.Viewport.Center.X) || double.IsNaN(_map.Viewport.Center.Y))
                _map.Viewport.Center = _map.Envelope.GetCentroid();
            _map.Viewport.Width = Width;
            _map.Viewport.Height = Height;
            if (Width >= 1080 && Height >= 1080) _map.Viewport.RenderResolutionMultiplier = 2;

            _viewportInitialized = true;
            OnViewportInitialized();
            _map.ViewChanged(true);
            return true;
        }

        public void MapDataChanged(object sender, DataChangedEventArgs e)
        {
            if (e.Cancelled)
            {
                RunOnUiThread(() => Toast.MakeText(Context, "Data fetch was cancelled", ToastLength.Long).Show());
            }
            else if (e.Error != null)
            {
                if (_previousDataError != e.Error.Message) // Don't repeat the same error messages. Can occur for many tile requests
                {
                    var message = $"Error during data fetch: {e.Error.Message}";
                    RunOnUiThread(() => Toast.MakeText(Context, message, ToastLength.Long).Show());
                    _previousDataError = e.Error.Message;
                }
            }
            else // no problems
            {
                RunOnUiThread(Invalidate);
            }
        }

        private void RunOnUiThread(Action method)
        {
            ((Activity) Context).RunOnUiThread(new Runnable(method));
        }

        private void OnViewportInitialized()
        {
            var handler = ViewportInitialized;
            handler?.Invoke(this, new EventArgs());
        }

        private void OnTouch(object sender, TouchEventArgs args)
        {
            if (Map.Lock) return;

            var mapAction = _touchHandler.Handle(args.Event);
            if (mapAction == MapAction.RefreshGraphics)
            {
                Map.Viewport.Transform(
                    _touchHandler.Touch.X, _touchHandler.Touch.Y,
                    _touchHandler.PreviousTouch.X, _touchHandler.PreviousTouch.Y,
                    _touchHandler.Scale);
                
                RestrictPan(Map.Viewport, RestrictedPanMode, RestrictedPanExtent ?? Map.Envelope);
                Map.Viewport.Resolution = RestrictZoom(Map.Resolutions, RestrictedZoomResolutions, Map.Viewport.Resolution, RestrictedZoomMode);
                
                Invalidate();
            }
            else if (mapAction == MapAction.RefreshData) Map.ViewChanged(true);
        }

        private static double RestrictZoom(IList<double> resolutions, Tuple<double, double> restrictedZoomResolutions, double resolution, RestrictedZoomMode mode)
        {
            if (mode == RestrictedZoomMode.KeepWithinResolutions)
            {
                double smallest;
                double biggest;
                if (restrictedZoomResolutions != null)
                {
                    smallest = Math.Min(restrictedZoomResolutions.Item1, restrictedZoomResolutions.Item2);
                    biggest = Math.Max(restrictedZoomResolutions.Item1, restrictedZoomResolutions.Item2);
                }
                else
                {
                    if (resolutions.Count == 0) return resolution;
                    smallest = resolutions[resolutions.Count - 1];
                    biggest = resolutions[0];
                }

                if (smallest > resolution) return smallest;
                if (biggest < resolution) return biggest;
            }
            return resolution;
        }

        private static void RestrictPan(IViewport viewport, RestrictedPanMode mode, BoundingBox maxExtent)
        {
            if (maxExtent == null) return; // Even the Map.Extent can be null if the extent of all layers is null

            if (mode == RestrictedPanMode.KeepCenterWithinExtents)
            {
                if (viewport.Center.X < maxExtent.Left)   viewport.Center.X = maxExtent.Left;
                if (viewport.Center.X > maxExtent.Right)  viewport.Center.X = maxExtent.Right;
                if (viewport.Center.Y > maxExtent.Top)    viewport.Center.Y = maxExtent.Top;
                if (viewport.Center.Y < maxExtent.Bottom) viewport.Center.Y = maxExtent.Bottom;
            }

            if (mode == RestrictedPanMode.KeepViewportWithinExtents)
            {
                // todo: do not keep within extent when viewport does not fit.
                if (viewport.Extent.Left < maxExtent .Left)
                    viewport.Center.X += maxExtent .Left - viewport.Extent.Left;
                if (viewport.Extent.Right > maxExtent .Right)
                    viewport.Center.X += maxExtent .Right - viewport.Extent.Right;
                if (viewport.Extent.Top > maxExtent .Top)
                    viewport.Center.Y += maxExtent .Top - viewport.Extent.Top;
                if (viewport.Extent.Bottom < maxExtent .Bottom)
                    viewport.Center.Y += maxExtent .Bottom - viewport.Extent.Bottom;
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (!_viewportInitialized) if (!TryInitializeViewport()) return;

            ICollection<ILayer> layers = Map.Layers;
            if (ShowCurrentLocation)
            {
                layers = layers.ToList();
                layers.Add(CurrentLocationLayer);
            }

            var copiedViewport = new Viewport(Map.Viewport) { // Copy so Viewport is not change during rendering to prevent flickering
                Center = { X = Map.Viewport.Center.X, Y = Map.Viewport.Center.Y } // Next version of Mapsui will have deep Viewport copy constructor
            };
            _openTKSurface.RefreshGraphics(copiedViewport, layers, Map.BackColor);
        }
    }
}