using Itinero.Code.Samples;
using Itinero.Core;
using SQLite.Net.Platform.Win32;

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
            MapControl.Map.Layers.Add(MbTilesSample.CreateLayer(new SQLitePlatformWin32(), ".\\Data\\test.mbtiles"));
            MapControl.Map.Layers.Add(LineStringSample.CreateLayer());
            MapControl.Map.Layers.Add(CitiesLayerSample.CreateLayer());
            MapControl.Map.Layers.Add(new CurrentLocationLayer());

            MapsuiLayerList.Initialize(MapControl.Map.Layers);
        }
    }
}
