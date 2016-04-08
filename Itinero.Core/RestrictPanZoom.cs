using System;
using System.Collections.Generic;
using Mapsui;
using Mapsui.Geometries;

namespace Itinero.Core
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

    public class RestrictPanZoom
    {
        public RestrictedPanMode PanMode { get; set; }
            = RestrictedPanMode.KeepViewportWithinExtents;

        public RestrictedZoomMode ZoomMode { get; set; }
            = RestrictedZoomMode.KeepWithinResolutions;

        /// <summary>
        /// Set this property in combination KeepCenterWithinExtents or KeepViewportWithinExtents.
        /// If Extent is not set Map.Extent will be used as restricted extent.
        /// </summary>
        public BoundingBox Extent { get; set; }

        /// <summary>
        /// Resolutions to keep the Map.Resolution within. The order of the two resolutions does not matter.
        /// </summary>
        public Tuple<double, double> Resolutions;

        public static double RestrictZoom(IList<double> resolutions, Tuple<double, double> restrictedZoomResolutions, double resolution, RestrictedZoomMode mode)
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

        public static void RestrictPan(IViewport viewport, RestrictedPanMode mode, BoundingBox maxExtent)
        {
            if (maxExtent == null) return; // Even the Map.Extent can be null if the extent of all layers is null

            if (mode == RestrictedPanMode.KeepCenterWithinExtents)
            {
                if (viewport.Center.X < maxExtent.Left) viewport.Center.X = maxExtent.Left;
                if (viewport.Center.X > maxExtent.Right) viewport.Center.X = maxExtent.Right;
                if (viewport.Center.Y > maxExtent.Top) viewport.Center.Y = maxExtent.Top;
                if (viewport.Center.Y < maxExtent.Bottom) viewport.Center.Y = maxExtent.Bottom;
            }

            if (mode == RestrictedPanMode.KeepViewportWithinExtents)
            {
                // todo: do not keep within extent when viewport does not fit.
                if (viewport.Extent.Left < maxExtent.Left)
                    viewport.Center.X += maxExtent.Left - viewport.Extent.Left;
                if (viewport.Extent.Right > maxExtent.Right)
                    viewport.Center.X += maxExtent.Right - viewport.Extent.Right;
                if (viewport.Extent.Top > maxExtent.Top)
                    viewport.Center.Y += maxExtent.Top - viewport.Extent.Top;
                if (viewport.Extent.Bottom < maxExtent.Bottom)
                    viewport.Center.Y += maxExtent.Bottom - viewport.Extent.Bottom;
            }
        }
    }
}
