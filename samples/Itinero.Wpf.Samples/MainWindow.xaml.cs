using BruTile.Predefined;
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

            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OpenStreetMap"});
            MapsuiLayerList.Initialize(MapControl.Map.Layers);
        }
    }
}
