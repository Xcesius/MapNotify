
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace MapNotify
{
    public class MapNotifySettings : ISettings
    {
        public MapNotifySettings()
        {
            Enable = new ToggleNode(true);
            ShowMapName = new ToggleNode(true);
            ShowModCount = new ToggleNode(true);
            ShowQuantityPercent = new ToggleNode(true);
            ColourQuantityPercent = new ToggleNode(true);
            ColourQuantity = new RangeNode<int>(100, 0, 220);
            ShowPackSizePercent = new ToggleNode(true);
            ShowModWarnings = new ToggleNode(true);
            ShowCompletion = new ToggleNode(true);
            HorizontalLines = new ToggleNode(true);
            PadForNinjaPricer = new ToggleNode(true);
            AlwaysShowTooltip = new ToggleNode(true);
        }
        public ToggleNode Enable { get; set; }
        public ToggleNode ShowMapName { get; set; }
        public ToggleNode ShowModCount { get; set; }
        public ToggleNode ShowQuantityPercent { get; set; }
        public ToggleNode ColourQuantityPercent { get; set; }
        public RangeNode<int> ColourQuantity { get; set; }
        public ToggleNode ShowPackSizePercent { get; set; }
        public ToggleNode ShowModWarnings { get; set; }
        public ToggleNode ShowCompletion { get; set; }
        public ToggleNode HorizontalLines { get; set; }
        public ToggleNode PadForNinjaPricer { get; set; }
        public ToggleNode AlwaysShowTooltip { get; set; }
    }
}
