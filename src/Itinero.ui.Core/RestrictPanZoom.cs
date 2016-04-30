using System;
using System.Collections.Generic;
using System.Linq;
using Mapsui;
using Mapsui.Geometries;

namespace Itinero.ui.Core
{
    public class MinMax
    {
        public MinMax(double value1, double value2)
        {
            if (value1 < value2)
            {
                Min = value1;
                Max = value2;
            }
            else
            {
                Min = value2;
                Max = value1;
            }
        }

        public double Min { get; }
        public double Max { get; }
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
        KeepWithinResolutionsAndAlwaysFillViewport
    }

    public class RestrictPanZoom
    {
        public RestrictPanMode PanMode { get; set; } = RestrictPanMode.KeepViewportWithinExtents;

        public RestrictZoomMode ZoomMode { get; set; } = RestrictZoomMode.KeepWithinResolutionsAndAlwaysFillViewport;

        /// <summary>
        /// Set this property in combination KeepCenterWithinExtents or KeepViewportWithinExtents.
        /// If Extent is not set Map.Extent will be used as restricted extent.
        /// </summary>
        public BoundingBox Extent { get; set; }

        /// <summary>
        /// Pair of the extreme resolution (biggest and smalles). The resolution is kept between these.
        /// The order of the two extreme resolutions does not matter.
        /// </summary>
        public MinMax ResolutionExtremes { get; set; }

        private static MinMax GetExtremes(IList<double> resolutions)
        {
            if (resolutions == null || resolutions.Count == 0) return null;
            resolutions = resolutions.OrderByDescending(r => r).ToList();
            var mostZoomedOut = resolutions[0];
            var mostZoomedIn = resolutions[resolutions.Count - 1]/2; // divide by two to allow one extra level zoom-in
            return new MinMax(mostZoomedOut, mostZoomedIn);
        }

        public double RestrictZoom(IViewport viewport, IList<double> resolutions, BoundingBox mapEnvelope)
        {
            if (ZoomMode == RestrictZoomMode.None) return viewport.Resolution;

            var resolutionExtremes = ResolutionExtremes ?? GetExtremes(resolutions);
            if (resolutionExtremes == null) return viewport.Resolution;

            if (ZoomMode == RestrictZoomMode.KeepWithinResolutions)
            {
                if (resolutionExtremes.Min > viewport.Resolution) return resolutionExtremes.Min;
                if (resolutionExtremes.Max < viewport.Resolution) return resolutionExtremes.Max;
            }
            else if (ZoomMode == RestrictZoomMode.KeepWithinResolutionsAndAlwaysFillViewport)
            {
                if (resolutionExtremes.Min > viewport.Resolution) return resolutionExtremes.Min;
                
                // This is the ...AndAlwaysFillViewport part
                var viewportFillingResolution = CalculateResolutionAtWhichMapFillsViewport(viewport, mapEnvelope);
                if (viewportFillingResolution < resolutionExtremes.Min) return viewport.Resolution; // Mission impossible. Can't adhere to both restrictions
                var limit = Math.Min(resolutionExtremes.Max, viewportFillingResolution);
                if (limit < viewport.Resolution) return limit;
            }
            return viewport.Resolution;
        }

        private double CalculateResolutionAtWhichMapFillsViewport(IViewport viewport, BoundingBox mapEnvelope)
        {
            return Math.Min(mapEnvelope.Width / viewport.Width, mapEnvelope.Height / viewport.Height);
        }

        public void RestrictPan(IViewport viewport, BoundingBox mapEnvelope)
        {
            var maxExtent = Extent ?? mapEnvelope;
            if (maxExtent == null) return; // Can be null because both Extent and Map.Extent. The Map.Extent can be null if the extent of all layers is null

            if (PanMode == RestrictPanMode.KeepCenterWithinExtents)
            {
                if (viewport.Center.X < maxExtent.Left) viewport.Center.X = maxExtent.Left;
                if (viewport.Center.X > maxExtent.Right) viewport.Center.X = maxExtent.Right;
                if (viewport.Center.Y > maxExtent.Top) viewport.Center.Y = maxExtent.Top;
                if (viewport.Center.Y < maxExtent.Bottom) viewport.Center.Y = maxExtent.Bottom;
            }
            else if (PanMode == RestrictPanMode.KeepViewportWithinExtents)
            {
                if (MapWidthSpansViewport(maxExtent.Width, viewport.Width, viewport.Resolution)) // if it does't fit don't restrict
                {
                    if (viewport.Extent.Left < maxExtent.Left)
                        viewport.Center.X += maxExtent.Left - viewport.Extent.Left;
                    if (viewport.Extent.Right > maxExtent.Right)
                        viewport.Center.X += maxExtent.Right - viewport.Extent.Right;
                }
                if (MapHeightSpansViewport(maxExtent.Height, viewport.Height, viewport.Resolution)) // if it does't fit don't restrict
                {
                    if (viewport.Extent.Top > maxExtent.Top)
                        viewport.Center.Y += maxExtent.Top - viewport.Extent.Top;
                    if (viewport.Extent.Bottom < maxExtent.Bottom)
                        viewport.Center.Y += maxExtent.Bottom - viewport.Extent.Bottom;
                }
            }
        }

        private static bool MapWidthSpansViewport(double extentWidth, double viewportWidth, double resolution)
        {
            var mapWidth = extentWidth / resolution; // in screen units
            return viewportWidth <= mapWidth;
        }

        private static bool MapHeightSpansViewport(double extentHeight, double viewportHeight, double resolution)
        {
            var mapHeight = extentHeight / resolution; // in screen units
            return viewportHeight <= mapHeight;
        }
    }
}
