using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.PoEMemory.Models;
using ExileCore.Shared.Enums;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using nuVector2 = System.Numerics.Vector2;
using nuVector4 = System.Numerics.Vector4;

namespace MapNotify
{
    public partial class MapNotify : BaseSettingsPlugin<MapNotifySettings>
    {
        public static nuVector4 GetRarityColor(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Rare:
                    return new nuVector4(0.99f, 0.99f, 0.46f, 1f);
                case ItemRarity.Magic:
                    return new nuVector4(0.52f, 0.52f, 0.99f, 1f);
                case ItemRarity.Unique:
                    return new nuVector4(0.68f, 0.37f, 0.11f, 1f);
                default:
                    return new nuVector4(1F, 1F, 1F, 1F);
            }
        }

        public static Dictionary<int, string> ZanaMods = new Dictionary<int, string>()
        {
            {1, "Slay Corrupted Boss" },
            {2, "Slay Rogue Exiles" },
            {3, "Find Map Item" },
            {4, "Find Unique Item" },
            {5, "Find Divination Card" },
            {6, "Find Vaal Fragment" },
            {8, "Complete Lab Trial" },
            {9, "Complete Abyss" },
            {10, "Slay Corrupted Monsters" },
            {11, "Slay Corrupted Monsters" },
            {12, "Slay Corrupted Monsters" },
            {20, "Find Lodestones" },
            {21, "Complete Map" },
            {22, "Slay Beyond Boss" },
            {23, "Slay Warband" },
            {24, "Slay Essence" },
            {25, "Slay Invasion Boss" },
            {26, "Slay Harbingers" },
            {27, "Open Unique Strongbox" },
            {28, "Open Breaches" },
            {29, "Complete Map" },
            {30, "Defeat Map Boss" },
            {31, "Defeat Elder Guardian" },
            {32, "Defeat Shaper Guardian" },
            {33, "Complete Legion Monolith" },
            {34, "Complete The Harvest" },
            {35, "Complete The Blight Encounter" },
            {37, "Complete The Delirium Encounter" },
            {38, "Complete The Metamorph Encounter" },
        };

        public class StyledText
        {
            public string Text { get; set; }
            public Vector4 Color { get; set; }
        }

        public class MavenDetails
        {
            public MavenDetails()
            {
                MavenInvitation = false;
                MavenCompletion = false;
                MavenUncharted = false;
                MavenArea = string.Empty;
                MavenRegion = string.Empty;
            }
            public bool MavenInvitation { get; set; }
            public bool MavenCompletion { get; set; }
            public bool MavenUncharted { get; set; }
            public string MavenArea { get; set; }
            public string MavenRegion { get; set; }
            public List<(string Boss, bool Complete)> MavenBosses { get; set; }
        }

        public class ItemDetails
        {
            public ItemDetails(NormalInventoryItem Item, Entity Entity)
            {
                this.Item = Item;
                this.Entity = Entity;
                ActiveWarnings = new List<StyledText>();
                Update();
            }

            public NormalInventoryItem Item { get; }
            public Entity Entity { get; }
            public List<StyledText> ActiveWarnings { get; set; }
            public StyledText ZanaMod { get; set; }
            public nuVector4 ItemColour { get; set; }
            public string MapName { get; set; }
            public string MapRegion { get; set; }
            public string ClassID { get; set; }
            public int PackSize { get; set; }
            public int Quantity { get; set; }
            public int ModCount { get; set; }
            public bool NeedsPadding { get; set; }
            public bool Bonus { get; set; }
            public bool Awakened { get; set; }
            public bool Completed { get; set; }
            public MavenDetails MavenDetails { get; set; }
            public int Tier { get; set; }

            public void Update()
            {
                BaseItemType BaseItem = gameController.Files.BaseItemTypes.Translate(Entity.Path);
                string ItemName = BaseItem.BaseName;
                ClassID = BaseItem.ClassName;
                MavenDetails mavenDetails = new MavenDetails();

                int packSize = 0;
                int quantity = Entity.GetComponent<Quality>()?.ItemQuality ?? 0;

                // get and evaluate mods
                var mapComponent = Entity.GetComponent<Map>() ?? null;
                Tier = mapComponent?.Tier ?? -1;
                NeedsPadding = Tier == -1 ? false : true;


                var modsComponent = Entity.GetComponent<Mods>() ?? null;
                ModCount = modsComponent?.ItemMods.Count() ?? 0;
                if (modsComponent != null && ModCount > 0)
                {
                    // add warnings, add quant, add packsize
                    if (modsComponent != null && modsComponent.ItemRarity != ItemRarity.Unique)
                    {
                        foreach (var mod in modsComponent.ItemMods.Where(x =>
                                                !x.Group.Contains("MapAtlasInfluence")
                                                && !x.RawName.Equals("InfectedMap")
                                                && !x.RawName.Equals("MapForceCorruptedSideArea")
                                                && !x.RawName.Equals("MapGainsRandomZanaMod")
                                                && !x.RawName.StartsWith("MapDoesntConsumeSextantCharge")
                                                && !x.RawName.StartsWith("MapEnchant")
                                                && !x.RawName.Equals("MapBossSurroundedByTormentedSpirits")
                                                && !x.RawName.Equals("MapZanaSubAreaMissionDetails")
                                                ))
                        {
                            #region Elder Guardian Maven Areas and Regions
                            if (mod.Group.Contains("MapElderContainsBoss"))
                            {
                                switch (mod.Value1)
                                {
                                    case 1:
                                        mavenDetails.MavenRegion = "The Twisted";
                                        mavenDetails.MavenArea = "The Enslaver";
                                        break;
                                    case 2:
                                        mavenDetails.MavenRegion = "The Twisted";
                                        mavenDetails.MavenArea = "The Eradicator";
                                        break;
                                    case 3:
                                        mavenDetails.MavenRegion = "The Twisted";
                                        mavenDetails.MavenArea = "The Constrictor";
                                        break;
                                    case 4:
                                        mavenDetails.MavenRegion = "The Twisted";
                                        mavenDetails.MavenArea = "The Purifier";
                                        break;
                                }
                                continue;
                            }
                            #endregion
                            quantity += mod.Value1;
                            packSize += mod.Value3;
                            if (WarningDictionary.Where(x => mod.RawName.Contains(x.Key)).Any())
                                ActiveWarnings.Add(WarningDictionary.Where(x => mod.RawName.Contains(x.Key)).FirstOrDefault().Value);
                        }
                    }

                    // Check Zana missions
                    if (!modsComponent.ItemMods.Any(x => x.RawName == "MapZanaSubAreaMissionDetails"))
                    {
                        ZanaMod = null;
                    }
                    else if (ZanaMods.TryGetValue(modsComponent.ItemMods.
                        FirstOrDefault(x => x.RawName == "MapZanaSubAreaMissionDetails").Value2, out string modName))
                    {
                        Vector4 textColor = new Vector4(0.9f, 0.85f, 0.65f, 1f);
                        if (modName.Contains("Guardian")) textColor = new Vector4(0.5f, 1f, 0.45f, 1f);
                        if (modName.Contains("Harvest")) textColor = new Vector4(0f, 1f, 1f, 1f);
                        if (modName.Contains("Delirium")) textColor = new Vector4(1f, 0f, 1f, 1f);
                        ZanaMod = new StyledText() { Color = textColor, Text = modName };
                    }
                    else
                    {
                        Vector4 textColor = new Vector4(0.9f, 0.85f, 0.65f, 1f);
                        modName = $"Unknown Zana Mission: {modsComponent.ItemMods.FirstOrDefault(x => x.RawName == "MapZanaSubAreaMissionDetails").Value2}";
                        ZanaMod = new StyledText() { Color = textColor, Text = modName };
                    }
                };

                Quantity = quantity;
                PackSize = packSize;

                if (!ClassID.Contains("HeistContract") &&
                    !ClassID.Contains("HeistBlueprint") &&
                    !ClassID.Contains("AtlasRegionUpgradeItem") &&
                    !ClassID.Contains("QuestItem") &&
                    !ClassID.Contains("MiscMapItem") &&
                    !ClassID.Contains("MapFragment"))
                {
                    string mapTrim = Entity.GetComponent<Base>().Name.Replace(" Map", "");
                    MapName = $"[T{mapComponent.Tier}] {mapTrim}";
                    Awakened = AwakenedAreas.Contains(mapComponent.Area) ? true : false;
                    Bonus = BonusAreas.Contains(mapComponent.Area) ? true : false;
                    Completed = CompletedAreas.Contains(mapComponent.Area) ? true : false;
                    mavenDetails.MavenCompletion = MavenAreas.Contains(mapComponent.Area) ? true : false;

                    if (AreaRegion.TryGetValue(mapTrim, out string region))
                        MapRegion = region;
                    else
                        MapRegion = "Unknown Region";
                }

                if (Entity.Path.Contains("MavenMap"))
                {
                    mavenDetails.MavenInvitation = true;
                    MapName = ItemName;
                }
                if (ClassID.Contains("MapFragment"))
                {
                    MapName = ItemName;
                    NeedsPadding = true;
                }
                #region Maven Regions & Areas
                if (Entity.Path.Contains("BreachFragmentPhysical") && MavenAreas.Any(x => x.Name == "Uul-Netol's Domain"))
                {
                    mavenDetails.MavenRegion = "The Hidden";
                    mavenDetails.MavenArea = "Uul-Netol's Domain";
                }
                else if (Entity.Path.Contains("BreachFragmentCold"))
                {
                    mavenDetails.MavenRegion = "The Hidden";
                    mavenDetails.MavenArea = "Tul's Domain";
                }
                else if (Entity.Path.Contains("BreachFragmentFire"))
                {
                    mavenDetails.MavenRegion = "The Hidden";
                    mavenDetails.MavenArea = "Xoph's Domain";
                }
                else if (Entity.Path.Contains("BreachFragmentLightning"))
                {
                    mavenDetails.MavenRegion = "The Hidden";
                    mavenDetails.MavenArea = "Esh's Domain";
                }
                else if (Entity.Path.Contains("BreachFragmentChaos"))
                {
                    mavenDetails.MavenRegion = "The Feared";
                    mavenDetails.MavenArea = "Chayula's Domain";
                }
                else if (Entity.Path.Contains("CurrencyElderFragment"))
                {
                    mavenDetails.MavenRegion = "The Feared";
                    mavenDetails.MavenArea = "Absence of Value and Meaning";
                }
                else if (Entity.Path.Contains("ShaperFragment"))
                {
                    mavenDetails.MavenRegion = "The Feared";
                    mavenDetails.MavenArea = "The Shaper's Realm";
                }
                else if (Entity.Path.Contains("VaalFragment2_"))
                {
                    mavenDetails.MavenRegion = "The Feared";
                    mavenDetails.MavenArea = "The Alluring Abyss";
                }
                else if (ItemName == "Cortex")
                {
                    mavenDetails.MavenRegion = "The Feared";
                    mavenDetails.MavenArea = "Cortex";
                }
                else if (ItemName == "Lair of the Hydra Map" ||
                    ItemName == "Maze of the Minotaur Map" ||
                    ItemName == "Forge of the Phoenix Map" ||
                    ItemName == "Pit of the Chimera Map")
                {
                    mavenDetails.MavenRegion = "The Formed";
                    mavenDetails.MavenArea = ItemName;
                }
                else if (ItemName == "Rewritten Distant Memory" ||
                    ItemName == "Augmented Distant Memory" ||
                    ItemName == "Altered Distant Memory" ||
                    ItemName == "Twisted Distant Memory")
                {
                    mavenDetails.MavenRegion = "The Forgotten";
                    mavenDetails.MavenArea = ItemName;
                }
                if (mavenDetails.MavenInvitation || mavenDetails.MavenArea != string.Empty)
                {
                    mavenDetails.MavenUncharted = MavenAreas.Any(x => x.Name == mavenDetails.MavenArea) ? true : false;
                    mavenDetails.MavenBosses = MavenBosses(Item.Item.Path, mavenDetails.MavenArea, mavenDetails.MavenRegion);
                }
                #endregion
                MavenDetails = mavenDetails;
                // evaluate rarity for colouring item name
                ItemColour = GetRarityColor(modsComponent?.ItemRarity ?? ItemRarity.Normal);
            }
        }
        public static List<(string, bool)> MavenBosses(string path, string area, string region)//NormalInventoryItem item)
        {
            List<(string, bool)> MavenBosses = new List<(string, bool)>();
            string activeRegion = string.Empty;

            Dictionary<string, List<string>> MavenRegionCompletion = new Dictionary<string, List<string>>();
            foreach (WorldArea worldArea in MavenAreas)
            {
                if (AreaRegion.TryGetValue(worldArea.Name, out string regionName)) { }
                else
                {
                    regionName = "Uncharted";
                }
                if (!MavenRegionCompletion.ContainsKey(regionName)) 
                    MavenRegionCompletion[regionName] = new List<string>() { worldArea.Name };
                else 
                    MavenRegionCompletion[regionName].Add(worldArea.Name);
            }

            if (!path.Contains("MavenMapVoid"))
                activeRegion = region;//RegionReadable.FirstOrDefault(x => item.Item.Path.Contains(x.Key)).Value;
            else if (path.Contains("MavenMapVoid5"))
                activeRegion = "The Feared";
            else if (path.Contains("MavenMapVoid4"))
                activeRegion = "The Hidden";
            else if (path.Contains("MavenMapVoid3"))
                activeRegion = "The Forgotten";
            else if (path.Contains("MavenMapVoid2"))
                activeRegion = "The Twisted";
            else if (path.Contains("MavenMapVoid1"))
                activeRegion = "The Formed";

            if (RegionArea.ContainsKey(activeRegion))
                foreach (string rArea in RegionArea[activeRegion])
                {
                    if (MavenRegionCompletion.ContainsKey(activeRegion))
                    {
                        if (!MavenRegionCompletion[activeRegion].Contains(rArea))
                            MavenBosses.Add((rArea, false));
                        else
                            MavenBosses.Add((rArea, true));
                    }
                    else
                        MavenBosses.Add((rArea, false));
                }
            else 
                foreach (string cArea in MavenRegionCompletion[activeRegion])
                    MavenBosses.Add((cArea, false));

            return MavenBosses;
        }
    }
}
