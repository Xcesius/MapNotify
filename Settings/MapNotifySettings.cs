using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using Vector4 = System.Numerics.Vector4;

namespace MapNotify
{
    public class MapNotifySettings : ISettings
    {
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

        public ToggleNode Enable { get; set; } = new(true);
        public RangeNode<int> InventoryCacheInterval { get; set; } = new(50, 1, 2000);
        public RangeNode<int> StashCacheInterval { get; set; } = new(500, 1, 2000);
        public ToggleNode ShowMapName { get; set; } = new(true);
        public ToggleNode ShowMapRegion { get; set; } = new(true);
        public ToggleNode ShowModCount { get; set; } = new(true);
        public ToggleNode ShowQuantityPercent { get; set; } = new(true);
        public ToggleNode ColorQuantityPercent { get; set; } = new(true);
        public ToggleNode MapBorderStyle { get; set; } = new(false);
        public RangeNode<int> ColorQuantity { get; set; } = new(100, 0, 220);
        public RangeNode<int> BorderDeflation { get; set; } = new(4, 0, 50);
        public RangeNode<int> BorderThickness { get; set; } = new(1, 1, 6);
        public RangeNode<int> MapQuantSetting { get; set; } = new(100, 0, 220);
        public RangeNode<int> MapPackSetting { get; set; } = new(100, 0, 220);
        public ToggleNode ShowPackSizePercent { get; set; } = new(true);
        public ToggleNode ShowModWarnings { get; set; } = new(true);
        public ToggleNode ShowCompletion { get; set; } = new(true);
        public ToggleNode HorizontalLines { get; set; } = new(true);
        public ToggleNode PadForNinjaPricer { get; set; } = new(false);
        public ToggleNode PadForNinjaPricer2 { get; set; } = new(false);
        public ToggleNode PadForAltPricer { get; set; } = new(false);
        public ToggleNode AlwaysShowTooltip { get; set; } = new(true);
        public ToggleNode AlwaysShowCompletionBorder { get; set; } = new(true);
        public ToggleNode StyleTextForBorder { get; set; } = new(false);
        public ToggleNode ShowForZanaMaps { get; set; } = new(true);
        public ToggleNode ShowLineForZanaMaps { get; set; } = new(true);
        public ToggleNode TargetRegions { get; set; } = new(true);
        public ToggleNode ShowForWatchstones { get; set; } = new(true);
        public ToggleNode ShowForHeist { get; set; } = new(true);
        public ToggleNode ShowForInvitations { get; set; } = new(true);
        public ToggleNode NonUnchartedList { get; set; } = new(false);
        public ToggleNode BoxForBricked { get; set; } = new(true);
        public ToggleNode BoxForMapWarnings { get; set; } = new(true);
        public ToggleNode BoxForMapBadWarnings { get; set; } = new(true);

        public ListNode BadModWarningsLoader { get; set; } = new();
        public Vector4 ElderGuardian { get; set; } = new(0.32f, 0.55f, 0.78f, 1f);
        public Vector4 ShaperGuardian { get; set; } = new(0.32f, 0.4f, 0.78f, 1f);
        public Vector4 Harvest { get; set; } = new(0f, 1f, 1f, 1f);
        public Vector4 Delirium { get; set; } = new(0.94f, 0f, 1f, 1f);
        public Vector4 Blighted { get; set; } = new(0.94f, 0.63f, 0.12f, 1f);
        public Vector4 Metamorph { get; set; } = new(0.4f, 0.78f, 0.24f, 1f);
        public Vector4 Legion { get; set; } = new(0.4f, 0.12f, 0.7f, 1f);
        public Vector4 BlightEncounter { get; set; } = new(0.43f, 0.4f, 0.24f, 1f);
        public Vector4 DefaultBorderTextColor { get; set; } = new(0.9f, 0.85f, 0.65f, 1f);
        public ToggleNode ElderGuardianBorder { get; set; } = new(true);
        public ToggleNode ShaperGuardianBorder { get; set; } = new(true);
        public ToggleNode HarvestBorder { get; set; } = new(true);
        public ToggleNode DeliriumBorder { get; set; } = new(true);
        public ToggleNode BlightedBorder { get; set; } = new(true);
        public ToggleNode MetamorphBorder { get; set; } = new(false);
        public ToggleNode LegionBorder { get; set; } = new(false);
        public ToggleNode BlightEncounterBorder { get; set; } = new(false);
        public ToggleNode CompletionBorder { get; set; } = new(true);
        public Vector4 Incomplete { get; set; } = new(0f, 1f, 0f, 0.3f);
        public Vector4 BonusIncomplete { get; set; } = new(1f, 0.47f, 0f, 0.3f);
        public Vector4 AwakenedIncomplete { get; set; } = new(1f, 0.9f, 0f, 0.3f);
        public Vector4 Bricked { get; set; } = new(1f, 0f, 0f, 1f);
        public Vector4 MapBorderWarnings { get; set; } = new(0f, 1f, 0f, 1f);

        public Vector4 MapBorderBad { get; set; } = new(1f, 0f, 0f, 1f);
        public RangeNode<int> BorderThicknessMap { get; set; } = new(2, 1, 6);
    }
}
