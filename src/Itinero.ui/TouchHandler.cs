using System;
using Android.Graphics;
using Android.Views;

namespace Itinero.ui
{
    enum MapAction
    {
        None,
        RefreshData,
        RefreshGraphics
    }

    class TouchHandler
    {
        enum TouchMode
        {
            None,
            Dragging,
            Zooming
        }

        public PointF PreviousTouch { get; private set; }
        public PointF Touch { get; private set; }
        public double Scale { get; private set; }

        TouchMode _mode = TouchMode.None;
        float _oldDist = 1f;
        
        public MapAction Handle(MotionEvent motionEvent)
        {
            MapAction mapAction = MapAction.None;

            var x = (int)motionEvent.RawX;
            var y = (int)motionEvent.RawY;

            switch (motionEvent.Action)
            {
                case MotionEventActions.Down:
                    PreviousTouch = new PointF(x, y);
                    Touch = new PointF(x, y);
                    _mode = TouchMode.Dragging;
                    break;
                case MotionEventActions.Up:
                    return MapAction.RefreshData;
                case MotionEventActions.Pointer2Down:
                    PreviousTouch = new PointF(x, y);
                    Touch = new PointF(x, y);
                    _oldDist = Spacing(motionEvent);
                    MidPoint(Touch, motionEvent);
                    PreviousTouch = Touch;
                    _mode = TouchMode.Zooming;
                    break;
                case MotionEventActions.Pointer2Up:
                    return MapAction.RefreshData;
                case MotionEventActions.Move:
                    switch (_mode)
                    {
                        case TouchMode.Dragging:
                            Scale = 1;
                            PreviousTouch = Touch;
                            Touch = new PointF(x, y);
                            mapAction = MapAction.RefreshGraphics;
                            break;
                        case TouchMode.Zooming:
                            if (motionEvent.PointerCount < 2) return MapAction.None;

                            var newDist = Spacing(motionEvent);
                            Scale = newDist / _oldDist;

                            _oldDist = Spacing(motionEvent);
                            PreviousTouch = new PointF(Touch.X, Touch.Y);
                            MidPoint(Touch, motionEvent);
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