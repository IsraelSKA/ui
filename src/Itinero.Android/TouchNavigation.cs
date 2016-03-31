using System;
using Android.Graphics;
using Android.Views;
using Mapsui;

namespace Itinero.Android
{
    enum TouchState
    {
        None,
        Dragging,
        Zooming
    }
    class TouchNavigation
    {
        TouchState _mode = TouchState.None;
        PointF _previousMap;
        PointF _currentMap;
        PointF _previousMid = new PointF();
        readonly PointF _currentMid = new PointF();
        float _oldDist = 1f;
        
        public Map Map { get; set; }

        public EventHandler RefreshGraphics;

        private void OnRefresGraphics()
        {
            var handler = RefreshGraphics;
            if (handler != null) RefreshGraphics(this, EventArgs.Empty);
        }

        public void Touch(MotionEvent motionEvent)
        {
            if (Map.Lock) return;

            var x = (int)motionEvent.RawX;
            var y = (int)motionEvent.RawY;

            switch (motionEvent.Action)
            {
                case MotionEventActions.Down:
                    _previousMap = null;
                    _mode = TouchState.Dragging;
                    break;
                case MotionEventActions.Up:
                    _previousMap = null;
                    _mode = TouchState.None;
                    Map.ViewChanged(true);
                    break;
                case MotionEventActions.Pointer2Down:
                    _previousMap = null;
                    _oldDist = Spacing(motionEvent);
                    MidPoint(_currentMid, motionEvent);
                    _previousMid = _currentMid;
                    _mode = TouchState.Zooming;
                    break;
                case MotionEventActions.Pointer2Up:
                    _previousMap = null;
                    _previousMid = null;
                    _mode = TouchState.Dragging;
                    Map.ViewChanged(true);
                    break;
                case MotionEventActions.Move:
                    switch (_mode)
                    {
                        case TouchState.Dragging:
                            _currentMap = new PointF(x, y);
                            if (_previousMap != null)
                            {
                                Map.Viewport.Transform(_currentMap.X, _currentMap.Y, _previousMap.X, _previousMap.Y);
                                OnRefresGraphics();
                            }
                            _previousMap = _currentMap;
                            break;
                        case TouchState.Zooming:
                            if (motionEvent.PointerCount < 2) return;

                            var newDist = Spacing(motionEvent);
                            var scale = newDist / _oldDist;

                            _oldDist = Spacing(motionEvent);
                            _previousMid = new PointF(_currentMid.X, _currentMid.Y);
                            MidPoint(_currentMid, motionEvent);
                            Map.Viewport.Transform(_currentMid.X, _currentMid.Y, _previousMid.X, _previousMid.Y,
                                scale);
                            OnRefresGraphics();
                            break;
                    }
                    break;
            }
        }

        static float Spacing(MotionEvent me)
        {
            if (me.PointerCount < 2) throw new ArgumentException();

            var x = me.GetX(0) - me.GetX(1);
            var y = me.GetY(0) - me.GetY(1);
            return (float)Math.Sqrt(x * x + y * y);
        }

        static void MidPoint(PointF point, MotionEvent me)
        {
            var x = me.GetX(0) + me.GetX(1);
            var y = me.GetY(0) + me.GetY(1);
            point.Set(x / 2, y / 2);
        }
    }
}