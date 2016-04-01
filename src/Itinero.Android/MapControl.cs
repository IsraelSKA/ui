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
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;

namespace Itinero.Android
{
    public sealed class MapControl : FrameLayout
    {
        public bool ShowCurrentLocation
        {
            get
            {
                return _showCurrentLocation;
            }
            set
            {
                _showCurrentLocation = value; 
                if (value) CurrentLocationLayer.StartListening();
                else CurrentLocationLayer.StopListening();
            }

        } 
        public CurrentLocationLayer CurrentLocationLayer { get; set; } = new CurrentLocationLayer();

        private bool _showCurrentLocation = true;
        readonly TouchHandler _touchHandler = new TouchHandler();
        bool _viewportInitialized;
        Map _map;
        readonly OpenTKSurface _openTKSurface;
        string _previousDataError = "";

        public event EventHandler<EventArgs> ViewportInitialized;
        
        public MapControl(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _openTKSurface = new OpenTKSurface(Context, attrs);
            AddView(_openTKSurface);
            CurrentLocationLayer.DataChanged += (sender, args) => Invalidate();

            Map = new Map();
            Touch += OnTouch;

            SetWillNotDraw(false);
        }

        public Map Map
        {
            get
            {
                return _map;
            }
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

        void MapRefreshGraphics(object sender, EventArgs e)
        {
            RunOnUiThread(Invalidate);
        }

        void MapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Map.Envelope))
            {
                _map.ViewChanged(true);
                Invalidate();
            }
        }

        bool TryInitializeViewport()
        {
            if (_viewportInitialized) return true;
            if (Math.Abs(Width - 0f) < Mapsui.Utilities.Constants.Epsilon) return false;
            if (_map?.Envelope == null) return false;
            if (Math.Abs(_map.Envelope.Width - 0d) < Mapsui.Utilities.Constants.Epsilon) return false;
            if (Math.Abs(_map.Envelope.Height - 0d) < Mapsui.Utilities.Constants.Epsilon) return false;
            if (_map.Envelope.GetCentroid() == null) return false;

            if (double.IsNaN(_map.Viewport.Resolution))
                _map.Viewport.Resolution = _map.Envelope.Width / Width;
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
                if (_previousDataError != e.Error.Message) // Don't repeat the same failed tile request 
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

        void RunOnUiThread(Action method)
        {
            ((Activity)Context).RunOnUiThread(new Java.Lang.Runnable(method));
        }

        void OnViewportInitialized()
        {
            var handler = ViewportInitialized;
            handler?.Invoke(this, new EventArgs());
        }

        void OnTouch(object sender, TouchEventArgs args)
        {
            if (Map.Lock) return;

            var mapAction = _touchHandler.Handle(args.Event);
            if (mapAction == MapAction.RefreshGraphics)
            {
                Map.Viewport.Transform(
                    _touchHandler.Touch.X, _touchHandler.Touch.Y, 
                    _touchHandler.PreviousTouch.X, _touchHandler.PreviousTouch.Y, 
                    _touchHandler.Scale);

                Invalidate();
            }
            else if (mapAction == MapAction.RefreshData) Map.ViewChanged(true);
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

            _openTKSurface.RefreshGraphics(Map.Viewport, layers, Map.BackColor);
        }
    }
}