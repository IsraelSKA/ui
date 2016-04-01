using System;
using Android.Graphics;
using Android.Views;

namespace Itinero.Android
{
    enum MapAction
    {
        None,
        RefreshData,
        RefreshGraphics
    }

    class TouchNavigation
    {
        enum TouchState
        {
            None,
            Dragging,
            Zooming
        }

        TouchState _mode = TouchState.None;
        public PointF PreviousMap { get; private set; }
        public PointF CurrentMap { get; private set; }
        public double Scale { get; private set; }
        float _oldDist = 1f;
        
        public MapAction Touch(MotionEvent motionEvent)
        {
            MapAction mapAction = MapAction.None;

            var x = (int)motionEvent.RawX;
            var y = (int)motionEvent.RawY;

            switch (motionEvent.Action)
            {
                case MotionEventActions.Down:
                    PreviousMap = new PointF(x, y);
                    CurrentMap = new PointF(x, y);
                    _mode = TouchState.Dragging;
                    break;
                case MotionEventActions.Up:
                    PreviousMap = null;
                    _mode = TouchState.None;
                    return MapAction.RefreshData;
                case MotionEventActions.Pointer2Down:
                    PreviousMap = new PointF(CurrentMap.X, CurrentMap.Y);
                    CurrentMap = new PointF(CurrentMap.X, CurrentMap.Y);
                    _oldDist = Spacing(motionEvent);
                    MidPoint(CurrentMap, motionEvent);
                    PreviousMap = CurrentMap;
                    _mode = TouchState.Zooming;
                    break;
                case MotionEventActions.Pointer2Up:
                    PreviousMap = null;
                    _mode = TouchState.Dragging;
                    return MapAction.RefreshData;
                case MotionEventActions.Move:
                    switch (_mode)
                    {
                        case TouchState.Dragging:
                            Scale = 1;
                            PreviousMap = CurrentMap;
                            CurrentMap = new PointF(x, y);
                            mapAction = MapAction.RefreshGraphics;
                            break;
                        case TouchState.Zooming:
                            if (motionEvent.PointerCount < 2) return MapAction.None;

                            var newDist = Spacing(motionEvent);
                            Scale = newDist / _oldDist;

                            _oldDist = Spacing(motionEvent);
                            PreviousMap = new PointF(CurrentMap.X, CurrentMap.Y);
                            MidPoint(CurrentMap, motionEvent);
                            mapAction = MapAction.RefreshGraphics;
                            break;
                    }
                    break;
            }
            return mapAction;
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