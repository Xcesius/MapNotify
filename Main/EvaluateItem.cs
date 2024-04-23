using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    return new nuVector4(0.68f, 0.68f, 1f, 1f); //0.52f, 0.52f, 0.99f, 1f
                case ItemRarity.Unique:
                    return new nuVector4(1f, 0.50f, 0.10f, 1f);
                default:
                    return new nuVector4(1F, 1F, 1F, 1F);
            }
        }
        public enum ObjectiveType
        {
            None,
            ElderGuardian,
            ShaperGuardian,
            Harvest,
            Delirium,
            Blighted,
            Metamorph,
            Legion,
            BlightEncounter,
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
            {34, "Harvest" },
            {35, "Blight Encounter" },
            {36, "Blighted Map" },
            {37, "Delirium Encounter" },
            {38, "Metamorph Encounter" },
        };

        public static readonly List<string> ModNameBlacklist = new List<string>(){
            "AfflictionMapDeliriumStacks",
            "AfflictionMapReward",
            "InfectedMap",
            "MapForceCorruptedSideArea",
            "MapGainsRandomZanaMod",
            "MapDoesntConsumeSextantCharge",
            "MapEnchant",
            "Enchantment",
            "MapBossSurroundedByTormentedSpirits",
            "MapZanaSubAreaMissionDetails",
        };

        public class StyledText
        {
            public string Text { get; set; }
            public Vector4 Color { get; set; }
            public bool Bricking { get; set; }
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
                ActiveBadMods = new List<StyledText>();
                Update();
            }



            public NormalInventoryItem Item { get; }
            public ServerInventory.InventSlotItem ItemS { get; }
            public Entity Entity { get; }
            public List<StyledText> ActiveWarnings { get; set; }
            public List<StyledText> ActiveBadMods { get; set; }
            public StyledText ZanaMod { get; set; }
            public ObjectiveType ZanaMissionType { get; set; }
            public nuVector4 ItemColor { get; set; }
            public string MapName { get; set; }
            public string MapRegion { get; set; }
            public string ClassID { get; set; }
            public int PackSize { get; set; }
            public int Quantity { get; set; }
            public int ModCount { get; set; }
            public bool NeedsPadding { get; set; }
            public bool LacksCompletion { get; set; }
            public bool Bonus { get; set; }
            public bool Awakened { get; set; }
            public bool Completed { get; set; }
            public bool Bricked { get; set; }
            public bool Corrupted { get; set; }
            public MavenDetails MavenDetails { get; set; }
            public int Tier { get; set; }

            public void Update()
            {
                var BaseItem = gameController.Files.BaseItemTypes.Translate(Entity.Path);
                var ItemName = BaseItem.BaseName;
                ClassID = BaseItem.ClassName;
                var mavenDetails = new MavenDetails();
                

                var packSize = 0;
                var quantity = Entity.GetComponent<Quality>()?.ItemQuality ?? 0;
                var settings = new MapNotifySettings();
                // get and evaluate mods
                var mapComponent = Entity.GetComponent<Map>() ?? null;
                Tier = mapComponent?.Tier ?? -1;
                NeedsPadding = Tier == -1 ? false : true;
                ZanaMissionType = ObjectiveType.None;
                Bricked = false;
                Corrupted = Entity.GetComponent<Base>()?.isCorrupted ?? false;

                var modsComponent = Entity.GetComponent<Mods>() ?? null;
                ModCount = modsComponent?.ItemMods.Count() ?? 0;
                if (modsComponent != null && ModCount > 0)
                {
                    // add warnings, add quant, add packsize
                    if (modsComponent != null && modsComponent.ItemRarity != ItemRarity.Unique)
                    {
                        foreach (var mod in modsComponent.ItemMods.Where(x =>
                                                !x.Group.Contains("MapAtlasInfluence")))
                        {
                            if(ModNameBlacklist.Any(m => mod.RawName.Contains(m)))
                            {
                                ModCount--;
                                continue;
                            }

                            #region Elder Guardian Maven Areas and Regions
                            if (mod.Group.Contains("MapElderContainsBoss"))
                            {
                                ModCount--;
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

                            UpdateValueIfStatExists("map_pack_size_+%", x => packSize += x);
                            UpdateValueIfStatExists("map_item_drop_quantity_+%", x => quantity += x);

                            if (WarningDictionary.Where(x => mod.RawName.Contains(x.Key)).Any())
                            {
                                var warning = WarningDictionary.Where(x => mod.RawName.Contains(x.Key)).FirstOrDefault().Value;
                                if (warning.Bricking || settings.ShowQuantityPercent && quantity <= settings.MapQuantSetting || settings.ShowPackSizePercent && packSize <= settings.MapPackSetting)
                                {
                                    Bricked = true;
                                }

                                ActiveWarnings.Add(warning);

                                // if (mod.Name.Equals("warningmods"))
                                //  {
                                // }
                            }

                            if (BadModsDictionary.Where(x => mod.RawName.Contains(x.Key)).Any())
                            {
                                var bad = BadModsDictionary.Where(x => mod.RawName.Contains(x.Key)).FirstOrDefault().Value;
                                if (bad.Bricking)
                                {
                                    Bricked = true;
                                }
                                ActiveBadMods.Add(bad);

                                // if (mod.Name.Equals("warningmods"))
                                //  {
                                // }
                            }
                            void UpdateValueIfStatExists(string key, Action<int> updateAction)
                            {
                                var index = mod.ModRecord.StatNames
                                    .Select((value, index) => new { value, index })
                                    .FirstOrDefault(pair => pair.value.Key == key)?.index ?? -1;

                                if (index != -1)
                                {
                                    updateAction(mod.Values[index]);
                                }
                            }
                        }
                    }


                    // Check Zana missions
                    if (!modsComponent.ItemMods.Any(x => x.RawName == "MapZanaSubAreaMissionDetails"))
                    {
                        ZanaMod = null;
                    }
                    else if (ZanaMods.TryGetValue(modsComponent.ItemMods.
                        FirstOrDefault(x => x.RawName == "MapZanaSubAreaMissionDetails").Value2, out var modName))
                    {
                        var textColor = new Vector4(0.9f, 0.85f, 0.65f, 1f);
                        if (modName.Contains("Elder Guardian"))
                            ZanaMissionType = ObjectiveType.ElderGuardian;
                        else if (modName.Contains("Shaper Guardian"))
                            ZanaMissionType = ObjectiveType.ShaperGuardian;
                        else if (modName.Contains("Harvest"))
                            ZanaMissionType = ObjectiveType.Harvest;
                        else if (modName.Contains("Delirium"))
                            ZanaMissionType = ObjectiveType.Delirium;
                        else if (modName.Contains("Blighted Map"))
                            ZanaMissionType = ObjectiveType.Blighted;
                        else if (modName.Contains("Blight Encounter"))
                            ZanaMissionType = ObjectiveType.BlightEncounter;
                        else if (modName.Contains("Legion"))
                            ZanaMissionType = ObjectiveType.Legion;
                        else if (modName.Contains("Metamorph"))
                            ZanaMissionType = ObjectiveType.Metamorph;
                        ZanaMod = new StyledText() { Color = textColor, Text = modName };
                    }
                    else
                    {
                        var textColor = new Vector4(0.9f, 0.85f, 0.65f, 1f);
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
                    var area = mapComponent.Area;
                    var mapTrim = Entity.GetComponent<Base>().Name.Replace(" Map", "");
                    if (modsComponent.ItemRarity == ItemRarity.Unique)
                    {
                        // normal map at inner, 0x18, 0x18
                        var mapUnique = gameController.IngameState.M.Read<long>(mapComponent.Address + 0x10, 0x10, 0x20);
                        area = gameController.Files.WorldAreas.GetByAddress(mapUnique) ?? area;
                        mapTrim = area.Name;
                    }
                    MapName = $"[T{mapComponent.Tier}] {mapTrim}";


                    Bonus = ingameState.ServerData.BonusCompletedAreas.Contains(area) ? true : false;
                    Completed = ingameState.ServerData.CompletedAreas.Contains(area) ? true : false;
                    mavenDetails.MavenCompletion = ingameState.ServerData.MavenWitnessedAreas.Contains(area) ? true : false;

                    if (AreaRegion.TryGetValue(mapTrim, out var region))
                        MapRegion = region;
                    else
                        MapRegion = "Unknown Region";
                }

                if (Entity.Path.Contains("MavenMap"))
                {
                    mavenDetails.MavenInvitation = true;
                    MapName = ItemName;
                    mavenDetails.MavenRegion = RegionReadable.FirstOrDefault(x => Entity.Path.Contains(x.Key)).Value ?? "Uncharted";
                }
                if (ClassID.Contains("MapFragment"))
                {
                    MapName = ItemName;
                    NeedsPadding = true;
                }

                #region Maven Regions & Areas
                if (Entity.Path.Contains("BreachFragmentPhysical"))
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
                else if (ItemName.Contains("Lair of the Hydra") ||
                    ItemName.Contains("Maze of the Minotaur") ||
                    ItemName.Contains("Forge of the Phoenix Map") ||
                    ItemName.Contains("Pit of the Chimera Map"))
                {
                    mavenDetails.MavenRegion = "The Formed";
                    mavenDetails.MavenArea = ItemName.Replace(" Map", "");
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
                    mavenDetails.MavenUncharted = ingameState.ServerData.MavenWitnessedAreas.Any(x => x.Name == mavenDetails.MavenArea) ? true : false;
                    mavenDetails.MavenBosses = MavenBosses(Entity.Path, mavenDetails.MavenRegion);
                }
                #endregion
                MavenDetails = mavenDetails;

                if ((MapName ?? string.Empty) != string.Empty && LacksCompletionList.Any(x => MapName.Contains(x)))
                    LacksCompletion = true;

                // evaluate rarity for colouring item name
                ItemColor = GetRarityColor(modsComponent?.ItemRarity ?? ItemRarity.Normal);
            }
        }
        public static List<(string, bool)> MavenBosses(string path, string region)//NormalInventoryItem item)
        {
            var MavenBosses = new List<(string, bool)>();
            var activeRegion = region;

            var MavenRegionCompletion = new Dictionary<string, List<string>>();
            foreach (var worldArea in ingameState.ServerData.MavenWitnessedAreas)
            {
                if (!AreaRegion.TryGetValue(worldArea.Name, out var regionName))
                    regionName = "Uncharted";
                if (!MavenRegionCompletion.ContainsKey(regionName)) 
                    MavenRegionCompletion[regionName] = new List<string>() { worldArea.Name };
                else 
                    MavenRegionCompletion[regionName].Add(worldArea.Name);
            }

            if (path.Contains("MavenMapVoid5"))
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
                foreach (var rArea in RegionArea[activeRegion])
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
            else if(MavenRegionCompletion.ContainsKey(activeRegion))
                foreach (var cArea in MavenRegionCompletion[activeRegion])
                    MavenBosses.Add((cArea, true));


            return MavenBosses;
        }
    }
}
