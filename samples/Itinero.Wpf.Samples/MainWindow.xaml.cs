using BruTile.Predefined;
using Itinero.Code.Samples;
using Mapsui.Layers;

namespace Itinero.Wpf.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            MapControl.Map.Layers.Add(OsmTilesSample.CreateLayer());
            MapControl.Map.Layers.Add(LineStringSample.CreateLayer());
            MapControl.Map.Layers.Add(RandomPointsWithMarkerSample.CreateLayer(MapControl.Map.Envelope));
            MapControl.Map.Layers.Add(PointWithMarkerSample.CreateLayer());

            MapsuiLayerList.Initialize(MapControl.Map.Layers);
        }
    }
}
