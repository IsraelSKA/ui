using Mapsui;

namespace Itinero.Wpf.Samples
{
    /// <summary>
    /// Interaction logic for LayerList.xaml
    /// </summary>
    public partial class LayerList
    {
        public LayerList()
        {
            InitializeComponent();
        }
        
        public void Initialize(LayerCollection layers)
        {
            Items.Children.Clear();

            foreach (var layer in layers)
            {
                Items.Children.Add(
                    new LayerListItem
                    {
                        LayerName = layer.Name,
                        Enabled = layer.Enabled,
                        LayerOpacity = layer.Opacity,
                        Layer = layer
                    });
            }
        }
    }
}
