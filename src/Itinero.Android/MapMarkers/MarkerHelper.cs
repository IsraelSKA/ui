using Android.Views;
using Itinero.ui.Core.Marker;
using Mapsui.Geometries;

namespace Itinero.ui.MapMarkers
{
    public static class MarkerHelper
    {
        public static void SetScreenPosition(View view, Point screenPosition,
            HorizontalAlignmentType horizontalAlignment, VerticalAlignmentType verticalAlignment)
        {
            view.SetX((int)(screenPosition.X - 0.5 * view.Width + GetXOffset(horizontalAlignment, view.Width)));
            view.SetY((int)(screenPosition.Y - 0.5 * view.Height + GetYOffset(verticalAlignment, view.Height)));
        }

        private static double GetYOffset(VerticalAlignmentType verticalAlignment, double height)
        {
            switch (verticalAlignment)
            {
                case VerticalAlignmentType.Top:
                    return 0.5 * height;
                case VerticalAlignmentType.Bottom:
                    return -0.5 * height;
            }
            return 0;
        }

        private static double GetXOffset(HorizontalAlignmentType horizontalAlignment, double width)
        {
            switch (horizontalAlignment)
            {
                case HorizontalAlignmentType.Left:
                    return -0.5 * width;
                case HorizontalAlignmentType.Right:
                    return 0.5 * width;
            }
            return 0;
        }
    }
}