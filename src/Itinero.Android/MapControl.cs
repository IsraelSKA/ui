using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using Mapsui;
using Mapsui.Fetcher;

namespace Itinero.Android
{
    public sealed class MapControl : FrameLayout
    {
        readonly TouchNavigation _touchNavigation = new TouchNavigation();
        bool _viewportInitialized;
        Map _map;
        readonly OpenTKSurface _openTKSurface;

        public event EventHandler<EventArgs> ViewportInitialized;
        
        public MapControl(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _openTKSurface = new OpenTKSurface(Context, attrs);
            
            Map = new Map();
            TryInitializeViewport();
            AddView(_openTKSurface);
            Touch += OnTouch;
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
                _openTKSurface.Map = _map;

                if (_map != null)
                {
                    _map.DataChanged += MapDataChanged;
                    _map.PropertyChanged += MapPropertyChanged;
                    _map.RefreshGraphics += MapRefreshGraphics;
                    _map.ViewChanged(true);
                }

                TryInitializeViewport();
                _openTKSurface.RefreshGraphics();
            }
        }
        void MapRefreshGraphics(object sender, EventArgs e)
        {
            ((Activity) Context).RunOnUiThread(new Java.Lang.Runnable(() => 
            { 
                TryInitializeViewport();
                _openTKSurface.RefreshGraphics();
            }));
        }

        void MapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Map.Envelope))
            {
                TryInitializeViewport();
                _map.ViewChanged(true);
                _openTKSurface.RefreshGraphics();
            }
            else if (e.PropertyName == "Enabled")
            {
                _openTKSurface.RefreshGraphics();
            }
        }

        void TryInitializeViewport()
        {
            if (_viewportInitialized) return;
            if (Math.Abs(Width - 0f) < Mapsui.Utilities.Constants.Epsilon) return;
            if (_map?.Envelope == null) return;
            if (Math.Abs(_map.Envelope.Width - 0d) < Mapsui.Utilities.Constants.Epsilon) return;
            if (Math.Abs(_map.Envelope.Height - 0d) < Mapsui.Utilities.Constants.Epsilon) return;
            if (_map.Envelope.GetCentroid() == null) return;

            if (double.IsNaN(_map.Viewport.Resolution))
                _map.Viewport.Resolution = _map.Envelope.Width / Width;
            if (double.IsNaN(_map.Viewport.Center.X) || double.IsNaN(_map.Viewport.Center.Y))
                _map.Viewport.Center = _map.Envelope.GetCentroid();
            _map.Viewport.Width = Width;
            _map.Viewport.Height = Height;
            if (Width >= 1080 && Height >= 1080) _map.Viewport.RenderResolutionMultiplier = 2;

            _viewportInitialized = true;
            _openTKSurface.ViewportInitialized = true;
            OnViewportInitialized();
            _map.ViewChanged(true);
        }

        public void MapDataChanged(object sender, DataChangedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                //todo test code below:
                //((Activity)Context).RunOnUiThread(new Runnable(Toast.MakeText(Context, GetErrorMessage(e), ToastLength.Short).Show));
            }
            else // no problems
            {
                ((Activity)Context).RunOnUiThread(new Java.Lang.Runnable(() =>
                {
                    TryInitializeViewport();
                    _openTKSurface.RefreshGraphics();
                }));
            }
        }

        void OnViewportInitialized()
        {
            var handler = ViewportInitialized;
            handler?.Invoke(this, new EventArgs());
        }

        void OnTouch(object sender, TouchEventArgs args)
        {
            if (Map.Lock) return;

            var mapAction = _touchNavigation.HandleTouch(args.Event);
            if (mapAction == MapAction.RefreshGraphics)
            {
                Map.Viewport.Transform(
                    _touchNavigation.Touch.X, _touchNavigation.Touch.Y, 
                    _touchNavigation.PreviousTouch.X, _touchNavigation.PreviousTouch.Y, 
                    _touchNavigation.Scale);

                _openTKSurface.RefreshGraphics();
            }
            else if (mapAction == MapAction.RefreshData) Map.ViewChanged(true);
        }
    }
}