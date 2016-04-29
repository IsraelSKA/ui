using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using Itinero.ui.Core;
using Itinero.ui.MapMarkers;
using Java.Lang;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Utilities;
using Math = System.Math;

namespace Itinero.ui
{
    public sealed class MapControl : FrameLayout
    {
        private readonly OpenTKSurface _openTKSurface;
        private readonly TouchHandler _touchHandler = new TouchHandler();
        private Map _map;
        private string _previousDataError = "";
        private bool _showCurrentLocation = true;
        private bool _viewportInitialized;
        private readonly List<IMarker> _markers = new List<IMarker>();
        private readonly ViewGroup _markerContainer;
        
        public MapControl(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _openTKSurface = new OpenTKSurface(context, attrs)
            {
                Width = 0,
                Height = 0
            };

            
            AddView(_openTKSurface);

            _markerContainer = CreateMarkerContainer(context, attrs);
            AddMarkerContainer();

            CurrentLocationLayer.DataChanged += (sender, args) => Invalidate();

            Map = new Map();
            Touch += OnTouch;

            SetWillNotDraw(false);
        }

        private ViewGroup CreateMarkerContainer(Context context, IAttributeSet attrs)
        {
            var markerContainer = new RelativeLayout(context, attrs);
            markerContainer.SetBackgroundColor(Color.Transparent);
            markerContainer.Visibility = ViewStates.Invisible;
            return markerContainer;
        }

        private void AddMarkerContainer()
        {
            var layoutParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            AddView(_markerContainer, layoutParams);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            Map.Viewport.Width = w;
            Map.Viewport.Height = h;
            Restrict.RestrictZoom(Map.Viewport, Map.Resolutions, Map.Envelope);
        }

        public RestrictPanZoom Restrict = new RestrictPanZoom();

        public IEnumerable<IMarker> Markers => _markers;

        public void AddMarker(IMarker marker) 
        {
            _markerContainer.AddView(marker.View);
            _markers.Add(marker);
        }

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
                _map.Viewport.Resolution = Math.Max(_map.Envelope.Width / Width, _map.Envelope.Height / Height);
            if (double.IsNaN(_map.Viewport.Center.X) || double.IsNaN(_map.Viewport.Center.Y))
                _map.Viewport.Center = _map.Envelope.GetCentroid();

            _map.Viewport.Width = Width;
            _map.Viewport.Height = Height;
            if (Width >= 1080 && Height >= 1080) _map.Viewport.RenderResolutionMultiplier = 2;
            if (Restrict.ZoomMode == RestrictZoomMode.KeepWithinResolutionsAndAlwaysFillViewport)
                Map.NavigateTo(Map.Envelope, ScaleMethod.Fill);

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
                if (_touchHandler.Mode == TouchHandler.TouchMode.None) RunOnUiThread(Invalidate);
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

                Map.Viewport.Resolution = Restrict.RestrictZoom(Map.Viewport, Map.Resolutions, Map.Envelope);
                Restrict.RestrictPan(Map.Viewport, Map.Envelope);

                Invalidate();
            }
            else if (mapAction == MapAction.RefreshData) Map.ViewChanged(true);
        }
        
        protected override void OnDraw(Canvas canvas)
        {
            if (!_viewportInitialized) if (!TryInitializeViewport()) return; // failed to initialized 
            
            ICollection <ILayer> layers = Map.Layers;
            if (ShowCurrentLocation)
            {
                layers = layers.ToList();
                layers.Add(CurrentLocationLayer);
            }

            var copiedViewport = new Viewport(Map.Viewport) { // Copy so Viewport is not changed during rendering to prevent flickering
                Center = { X = Map.Viewport.Center.X, Y = Map.Viewport.Center.Y } // Next version of Mapsui will have deep Viewport copy constructor
            };

            _openTKSurface.RefreshGraphics(copiedViewport, layers, Map.BackColor, 
                () => UpdateMarkerLayer(copiedViewport, _markers));

            if (_markerContainer.Visibility == ViewStates.Invisible) 
            {
                // trick so show marker after layout has been updated
                // Only done on first render
                UpdateMarkerLayer(copiedViewport, _markers);
                _markerContainer.Visibility = ViewStates.Visible;
            }
        }

        private static void UpdateMarkerLayer(Viewport viewport, List<IMarker> markers)
        {

            foreach (var marker in markers)
            {
                var screenPosition = viewport.WorldToScreen(marker.GeoPosition.X, marker.GeoPosition.Y);
                MarkerHelper.SetScreenPosition(marker.View, screenPosition, marker.HorizontalAlignment, marker.VerticalAlignment);
            }
        }
    }
}