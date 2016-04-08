using System;
using System.Collections.Generic;
using System.Linq;
using Mapsui;
using Mapsui.Geometries;

namespace Itinero.Core
{
    public class PairOfDoubles : Tuple<double, double>
    {
        public PairOfDoubles(double item1, double item2) : base(item1, item2) { }
    }

    public enum RestrictPanMode
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

    public enum RestrictZoomMode
    {
        None,
        KeepWithinResolutions,
    }

    public class RestrictPanZoom
    {
        public RestrictPanMode PanMode { get; set; }
            = RestrictPanMode.KeepViewportWithinExtents;

        public RestrictZoomMode ZoomMode { get; set; }
            = RestrictZoomMode.KeepWithinResolutions;

        /// <summary>
        /// Set this property in combination KeepCenterWithinExtents or KeepViewportWithinExtents.
        /// If Extent is not set Map.Extent will be used as restricted extent.
        /// </summary>
        public BoundingBox Extent { get; set; }

        /// <summary>
        /// Resolutions to keep the Map.Resolution within. The order of the two resolutions does not matter.
        /// </summary>
        public PairOfDoubles Resolutions { get; set; }

        private static PairOfDoubles ResolutionsToPair(IList<double> resolutions)
        {
            if (resolutions == null || resolutions.Count == 0) return null;
            return new PairOfDoubles(resolutions.First(), resolutions.Last());
        }

        public double RestrictZoom(IList<double> resolutions, double resolution)
        {
            if (ZoomMode == RestrictZoomMode.KeepWithinResolutions)
            {
                var pairOfResolutions = Resolutions ?? ResolutionsToPair(resolutions);
                if (pairOfResolutions == null) return resolution;

                var smallest = Math.Min(pairOfResolutions.Item1, pairOfResolutions.Item2);
                var biggest = Math.Max(pairOfResolutions.Item1, pairOfResolutions.Item2);

                if (smallest > resolution) return smallest;
                if (biggest < resolution) return biggest;
            }
            return resolution;
        }

        public bool MapWidthSpansViewport(IViewport viewport, double width)
        {
            return viewport.Width < width / viewport.Resolution;
        }

        public bool MapHeightSpansViewport(IViewport viewport, double height)
        {
            return viewport.Height < height / viewport.Resolution;
        }

        public void RestrictPan(IViewport viewport, BoundingBox mapExtent)
        {
            var maxExtent = Extent ?? mapExtent;
            if (maxExtent == null) return; // Even the Map.Extent can be null if the extent of all layers is null

            if (PanMode == RestrictPanMode.KeepCenterWithinExtents)
            {
                if (viewport.Center.X < maxExtent.Left) viewport.Center.X = maxExtent.Left;
                if (viewport.Center.X > maxExtent.Right) viewport.Center.X = maxExtent.Right;
                if (viewport.Center.Y > maxExtent.Top) viewport.Center.Y = maxExtent.Top;
                if (viewport.Center.Y < maxExtent.Bottom) viewport.Center.Y = maxExtent.Bottom;
            }

            if (PanMode == RestrictPanMode.KeepViewportWithinExtents)
            {
                if (MapWidthSpansViewport(viewport, maxExtent.Width))
                {
                    if (viewport.Extent.Left < maxExtent.Left)
                        viewport.Center.X += maxExtent.Left - viewport.Extent.Left;
                    if (viewport.Extent.Right > maxExtent.Right)
                        viewport.Center.X += maxExtent.Right - viewport.Extent.Right;
                }
                if (MapHeightSpansViewport(viewport, maxExtent.Height))
                {
                    if (viewport.Extent.Top > maxExtent.Top)
                        viewport.Center.Y += maxExtent.Top - viewport.Extent.Top;
                    if (viewport.Extent.Bottom < maxExtent.Bottom)
                        viewport.Center.Y += maxExtent.Bottom - viewport.Extent.Bottom;
                }
            }
        }
    }
}
