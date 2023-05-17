using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

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
            ColorQuantityPercent = new ToggleNode(true);
            ColorQuantity = new RangeNode<int>(100, 0, 220);
            BorderThickness = new RangeNode<int>(1, 1, 6);
            ShowPackSizePercent = new ToggleNode(true);
            ShowModWarnings = new ToggleNode(true);
            ShowCompletion = new ToggleNode(true);
            HorizontalLines = new ToggleNode(true);
            PadForNinjaPricer = new ToggleNode(false);
            PadForAltPricer = new ToggleNode(false);
            AlwaysShowTooltip = new ToggleNode(true);
            AlwaysShowCompletionBorder = new ToggleNode(true);
            StyleTextForBorder = new ToggleNode(false);
            ShowForZanaMaps = new ToggleNode(true);
            ShowLineForZanaMaps = new ToggleNode(true);
            TargetRegions = new ToggleNode(true);
            ShowForWatchstones = new ToggleNode(true);
            ShowForHeist = new ToggleNode(true);
            ShowForInvitations = new ToggleNode(true);
            NonUnchartedList = new ToggleNode(false);
            BoxForBricked = new ToggleNode(true);
            BoxForMapWarnings = new ToggleNode(true);
            BoxForMapBadWarnings = new ToggleNode(true);
            BadModWarningsLoader = new ListNode();

            ElderGuardian = new Vector4(0.32f, 0.55f, 0.78f, 1f);
            ShaperGuardian = new Vector4(0.32f, 0.4f, 0.78f, 1f);
            Harvest = new Vector4(0f, 1f, 1f, 1f);
            Delirium = new Vector4(0.94f, 0f, 1f, 1f);
            Blighted = new Vector4(0.94f, 0.63f, 0.12f, 1f);
            Metamorph = new Vector4(0.4f, 0.78f, 0.24f, 1f);
            Legion = new Vector4(0.4f, 0.12f, 0.7f, 1f);
            BlightEncounter = new Vector4(0.43f, 0.4f, 0.24f, 1f);
            DefaultBorderTextColor = new Vector4(0.9f, 0.85f, 0.65f, 1f);

            ElderGuardianBorder = new ToggleNode(true);
            ShaperGuardianBorder = new ToggleNode(true);
            HarvestBorder = new ToggleNode(true);
            DeliriumBorder = new ToggleNode(true);
            BlightedBorder = new ToggleNode(true);
            MetamorphBorder = new ToggleNode(false);
            LegionBorder = new ToggleNode(false);
            BlightEncounterBorder = new ToggleNode(false);
            CompletionBorder = new ToggleNode(true);

            Incomplete = new Vector4(0f, 1f, 0f, 0.3f);
            BonusIncomplete = new Vector4(1f, 0.47f, 0f, 0.3f);
            AwakenedIncomplete = new Vector4(1f, 0.9f, 0f, 0.3f);

            Bricked = new Vector4(1f, 0f, 0f, 1f);
            MapBorderWarnings = new Vector4(1f, 0f, 0f, 1f);
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
        public ToggleNode ColorQuantityPercent { get; set; }
        public RangeNode<int> ColorQuantity { get; set; }
        public RangeNode<int> BorderThickness { get; set; }
        public ToggleNode ShowPackSizePercent { get; set; }
        public ToggleNode ShowModWarnings { get; set; }
        public ToggleNode ShowCompletion { get; set; }
        public ToggleNode HorizontalLines { get; set; }
        public ToggleNode PadForNinjaPricer { get; set; }
        public ToggleNode PadForAltPricer { get; set; }
        public ToggleNode AlwaysShowTooltip { get; set; }
        public ToggleNode AlwaysShowCompletionBorder { get; set; }
        public ToggleNode StyleTextForBorder { get; set; }
        public ToggleNode ShowForZanaMaps { get; set; }
        public ToggleNode ShowLineForZanaMaps { get; set; }
        public ToggleNode TargetRegions { get; set; }
        public ToggleNode ShowForWatchstones { get; set; }
        public ToggleNode ShowForHeist { get; set; }
        public ToggleNode ShowForInvitations { get; set; }
        public ToggleNode NonUnchartedList { get; set; }
        public ToggleNode BoxForBricked { get; set; }
        public ToggleNode BoxForMapWarnings { get; set; }
        public ToggleNode BoxForMapBadWarnings { get; set; }

        public ListNode BadModWarningsLoader { get; set; }
        public Vector4 ElderGuardian { get; set; }
        public Vector4 ShaperGuardian { get; set; }
        public Vector4 Harvest { get; set; }
        public Vector4 Delirium { get; set; }
        public Vector4 Blighted { get; set; }
        public Vector4 Metamorph { get; set; }
        public Vector4 Legion { get; set; }
        public Vector4 BlightEncounter { get; set; }
        public Vector4 DefaultBorderTextColor { get; set; }
        public ToggleNode ElderGuardianBorder { get; set; }
        public ToggleNode ShaperGuardianBorder { get; set; }
        public ToggleNode HarvestBorder { get; set; }
        public ToggleNode DeliriumBorder { get; set; }
        public ToggleNode BlightedBorder { get; set; }
        public ToggleNode MetamorphBorder { get; set; }
        public ToggleNode LegionBorder { get; set; }
        public ToggleNode BlightEncounterBorder { get; set; }
        public ToggleNode CompletionBorder { get; set; }
        public Vector4 Incomplete { get; set; }
        public Vector4 BonusIncomplete { get; set; }
        public Vector4 AwakenedIncomplete { get; set; }
        public Vector4 Bricked { get; set; }
        public Vector4 MapBorderWarnings { get; set; }
    }
}
