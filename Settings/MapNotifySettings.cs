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
            ShowMapRegion = new ToggleNode(true);
            ShowModCount = new ToggleNode(true);
            ShowQuantityPercent = new ToggleNode(true);
            ColourQuantityPercent = new ToggleNode(true);
            ColourQuantity = new RangeNode<int>(100, 0, 220);
            ShowPackSizePercent = new ToggleNode(true);
            ShowModWarnings = new ToggleNode(true);
            ShowCompletion = new ToggleNode(true);
            HorizontalLines = new ToggleNode(true);
            PadForNinjaPricer = new ToggleNode(false);
            PadForAltPricer = new ToggleNode(false);
            AlwaysShowTooltip = new ToggleNode(true);
            ShowForZanaMaps = new ToggleNode(true);
            ShowLineForZanaMaps = new ToggleNode(true);
            TargetRegions = new ToggleNode(true);
            ShowForWatchstones = new ToggleNode(true);
            ShowForHeist = new ToggleNode(true);
            ShowForInvitations = new ToggleNode(true);

        }
        public bool TargettingHaewarkHamlet = false;
        public bool TargettingTirnsEnd = false;
        public bool TargettingLexProxima= false;
        public bool TargettingLexEjoris = false;
        public bool TargettingNewVastir = false;
        public bool TargettingGlennachCairns = false;
        public bool TargettingValdosRest = false;
        public bool TargettingLiraArthain = false;

        public float AtlasX { get; set; }
        public float AtlasY { get; set; }

        public ToggleNode Enable { get; set; }
        public ToggleNode ShowMapName { get; set; }
        public ToggleNode ShowMapRegion { get; set; }
        public ToggleNode ShowModCount { get; set; }
        public ToggleNode ShowQuantityPercent { get; set; }
        public ToggleNode ColourQuantityPercent { get; set; }
        public RangeNode<int> ColourQuantity { get; set; }
        public ToggleNode ShowPackSizePercent { get; set; }
        public ToggleNode ShowModWarnings { get; set; }
        public ToggleNode ShowCompletion { get; set; }
        public ToggleNode HorizontalLines { get; set; }
        public ToggleNode PadForNinjaPricer { get; set; }
        public ToggleNode PadForAltPricer { get; set; }
        public ToggleNode AlwaysShowTooltip { get; set; }
        public ToggleNode ShowForZanaMaps { get; set; }
        public ToggleNode ShowLineForZanaMaps { get; set; }
        public ToggleNode TargetRegions { get; set; }
        public ToggleNode ShowForWatchstones { get; set; }
        public ToggleNode ShowForHeist { get; set; }
        public ToggleNode ShowForInvitations { get; set; }
    }
}
