using Mapsui;
using Mapsui.Geometries;
using NUnit.Framework;

namespace Itinero.Core.Tests
{
    [TestFixture]
    public class RestrictPanZoomTests
    {
        [Test]
        public void TestRestrictZoom()
        {
            // arrange
            var restrictPanZoom = new RestrictPanZoom {PanMode = RestrictPanMode.KeepCenterWithinExtents};
            var viewport = new Viewport { Center = new Point(0, 0), Width = 100, Height = 100, Resolution = 1};
            // viewport.Center is (0, 0) at this point
            var restrictTo = new BoundingBox(20, 40, 120, 140); // Minimal X value is 20, Minimal Y value is 40

            // act 
            restrictPanZoom.RestrictPan(viewport, restrictTo);

            // assert
            Assert.AreEqual(viewport.Center.X, 20);
            Assert.AreEqual(viewport.Center.Y, 40);
        }
    }
}
