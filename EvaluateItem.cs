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
            public string MavenBosses { get; set; }
            public string ClassID { get; set; }
            public int PackSize { get; set; }
            public int Quantity { get; set; }
            public int ModCount { get; set; }
            public bool NeedsPadding { get; set; }
            public bool Bonus { get; set; }
            public bool Awakened { get; set; }
            public bool Completed { get; set; }
            public bool Maven { get; set; }
            public int Tier { get; set; }

            public void Update()
            {
                BaseItemType BaseItem = gameController.Files.BaseItemTypes.Translate(Entity.Path);
                string ItemName = BaseItem.BaseName;
                ClassID = BaseItem.ClassName;

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
                                                            && !x.Group.Contains("MapElderContainsBoss")
                                                            && !x.RawName.Equals("InfectedMap")
                                                            && !x.RawName.Equals("MapZanaSubAreaMissionDetails")
                                                            ))
                        {
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
                    } else
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
                    !ClassID.Contains("MiscMapItem"))
                {
                    MapName = $"[T{mapComponent.Tier}] {Entity.GetComponent<Base>().Name.Replace(" Map", "")}";
                    Awakened = AwakenedAreas.Contains(mapComponent.Area) ? true : false;
                    Bonus = BonusAreas.Contains(mapComponent.Area) ? true : false;
                    Completed = CompletedAreas.Contains(mapComponent.Area) ? true : false;
                    Maven = MavenAreas.Contains(mapComponent.Area) ? true : false;
                }

                if (Entity.Path.Contains("MavenMap"))
                {
                    MapName = ItemName;
                    MavenBosses = MavenBosses(Item);
                }

                // evaluate rarity for colouring item name
                ItemColour = GetRarityColor(modsComponent?.ItemRarity ?? ItemRarity.Normal);
            }
        }

        public static string MavenBosses(NormalInventoryItem item)
        {
            MavenDict = new Dictionary<string, string>();
            string activeRegion = string.Empty;
            foreach (WorldArea worldArea in MavenAreas)
            {
                if (AreaRegion.TryGetValue(worldArea.Name, out string region))
                {
                    if (MavenDict.ContainsKey(region)) MavenDict[region] += $", {worldArea.Name}";
                    else MavenDict[region] = $"{worldArea.Name}";
                } else
                {
                    if (MavenDict.ContainsKey("Uncharted")) MavenDict["Uncharted"] += $", {worldArea.Name}";
                    else MavenDict["Uncharted"] = $"{worldArea.Name}";
                }
            }

            if (!item.Item.Path.Contains("MavenMapVoid"))
                activeRegion = RegionReadable.FirstOrDefault(x => item.Item.Path.Contains(x.Key)).Value;
            else if (item.Item.Path.Contains("MavenMapVoid5"))
                activeRegion = "The Feared";
            else if (item.Item.Path.Contains("MavenMapVoid4"))
                activeRegion = "The Hidden";
            else if (item.Item.Path.Contains("MavenMapVoid3"))
                activeRegion = "The Forgotten";
            else if (item.Item.Path.Contains("MavenMapVoid2"))
                activeRegion = "The Twisted";
            else if (item.Item.Path.Contains("MavenMapVoid1"))
                activeRegion = "The Formed";
            else return $"Could not detect a region for {item.Item.Path}";

            if (MavenDict.TryGetValue(activeRegion, out string returnText)) return returnText;
            else return $"You have killed no bosses towards {activeRegion}";
        }
    }
}
