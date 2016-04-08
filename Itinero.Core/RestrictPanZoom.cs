using System;
using System.Collections.Generic;
using System.Linq;
using Mapsui;
using Mapsui.Geometries;

namespace Itinero.Core
{
    public class PairOfDoubles : Tuple<double, double>
    {
        public PairOfDoubles(double item1, double item2) : base(item1, item2)
        {
        }
    }

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
        public PairOfDoubles Resolutions { get; set; }

        public static PairOfDoubles ToPairOfResolutions(PairOfDoubles pairOfResolutions, IList<double> resolutions)
        {
            if (pairOfResolutions != null) return pairOfResolutions;
            if (resolutions == null || resolutions.Count == 0) return null;
            return new PairOfDoubles(resolutions.First(), resolutions.Last());
        }

        public double RestrictZoom(PairOfDoubles pairOfResolutions, double resolution)
        {
            if (ZoomMode == RestrictedZoomMode.KeepWithinResolutions)
            {
                if (pairOfResolutions == null) return resolution;

                var smallest = Math.Min(pairOfResolutions.Item1, pairOfResolutions.Item2);
                var biggest = Math.Max(pairOfResolutions.Item1, pairOfResolutions.Item2);

                if (smallest > resolution) return smallest;
                if (biggest < resolution) return biggest;
            }
            return resolution;
        }

        public void RestrictPan(IViewport viewport, BoundingBox maxExtent)
        {
            if (maxExtent == null) return; // Even the Map.Extent can be null if the extent of all layers is null

            if (PanMode == RestrictedPanMode.KeepCenterWithinExtents)
            {
                if (viewport.Center.X < maxExtent.Left) viewport.Center.X = maxExtent.Left;
                if (viewport.Center.X > maxExtent.Right) viewport.Center.X = maxExtent.Right;
                if (viewport.Center.Y > maxExtent.Top) viewport.Center.Y = maxExtent.Top;
                if (viewport.Center.Y < maxExtent.Bottom) viewport.Center.Y = maxExtent.Bottom;
            }

            if (PanMode == RestrictedPanMode.KeepViewportWithinExtents)
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
