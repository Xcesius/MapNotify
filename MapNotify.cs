using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using SharpDX;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImGuiNET;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.Elements.InventoryElements;
using nuVector2 = System.Numerics.Vector2;
using nuVector4 = System.Numerics.Vector4;

namespace MapNotify
{
    public partial class MapNotify : BaseSettingsPlugin<MapNotifySettings>
    {
        private RectangleF windowArea;
        public static IngameState ingameState;
        public override bool Initialise()
        {
            base.Initialise();
            Name = "Map Mod Notifications";
            windowArea = GameController.Window.GetWindowRectangle();
            WarningDictionary = LoadConfig(Path.Combine(DirectoryFullName, "ModWarnings.txt"));
            ingameState = GameController.Game.IngameState; 
            return true;
        }
        public class Warning
        {
            public string Text { get; set; }
            public nuVector4 Color { get; set; }
        }

        public IEnumerable<string[]> GenDictionary(string path)
        {
            return File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line) 
            && line.IndexOf(';') >= 0 
            && !line.StartsWith("#")).Select(line => line.Split(new[] { ';' }, 3).Select(parts => parts.Trim()).ToArray());
        }

        public nuVector4 HexToVector4(string value)
        {

            uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var abgr);
            Color color = Color.FromAbgr(abgr);
            return new nuVector4((color.R / (float)255), color.G / (float)255, color.B / (float)255, color.A / (float)255);
        }

        public Dictionary<string, Warning> LoadConfig(string path)
        {
            if (!File.Exists(path)) CreateConfig(path);
            return GenDictionary(path).ToDictionary(line => line[0], line =>
            {
                var preloadAlerConfigLine = new Warning { Text = line[1], Color = HexToVector4(line[2]) };
                return preloadAlerConfigLine;
            });
        }

        public void CreateConfig(string path)
        {
            #region Create Default Config
            new FileInfo(path).Directory.Create();
            string outFile =
@"#Mod Contains;Name in tooltip;RGBA colour code
# REFLECT
ElementalReflect;Elemental Reflect;FF0000FF
PhysicalReflect;Physical Reflect;FF0000FF
# REGEN
NoLifeESRegen;No Regen;FF007FFF
MapPlayerReducedRegen;60%% Less Regen;FF007FFF
# MAX RES
MapPlayerMaxResists;Max Res Down;FF007FFF
# CURSES AND EE
MapPlayerCurseEnfeeble;Enfeeble;FF00FFFF
MapPlayerCurseElementalWeak;Elemental Weakness;FF00FFFF
MapPlayerCurseVuln;Vulnerability;FF00FFFF
MapPlayerCurseTemp;Temporal Chains;FF00FFFF
MapPlayerElementalEquilibrium;Elemental Equilibrium;FF00FFFF
# BOSSES
MapTwoBosses;Twinned;00FF00FF
MapDangerousBoss;Boss Damage & Speed;00FF00FF
MapMassiveBoss;Boss AoE & Life;00FF00FF
# MONSTERS
MapMonsterColdDamage;Extra Phys as Cold;FF007FFF
MapMonsterFireDamage;Extra Phys as Fire;FF007FFF
MapMonsterLightningDamage;Extra Phys as Lightning;FF007FFF
MapMonsterLife;More Monster Life;FF007FFF
MapMonsterFast;Monster Speed;FF007FFF
# OTHER
MapBeyondLeague;Beyond;FF7F00FF
#MapBloodlinesModOnMagicsMapWorld;Bloodlines;FF7F00FF
#MapNemesis;Nemesis;FF7F00FF
# GROUND
MapDesecratedGround;Desecrated Ground;CCCC00FF
MapShockedGround;Shocked Ground;CCCC00FF
MapChilledGround;Chilled Ground;CCCC00FF
MapBurningGround;Burning Ground;CCCC00FF";
            File.WriteAllText(path, outFile);
            #endregion
        }

        public nuVector4 GetRarityColor(ItemRarity rarity)
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

        public Dictionary<int, string> ZanaMods = new Dictionary<int, string>()
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
            {33, "Complete Legion Monolith" },
        };

        Dictionary<string, Warning> WarningDictionary = new Dictionary<string, Warning>();
        public int mPad;
        public void RenderItem(Entity entity, byte mode)
        {
            if (entity.Address != 0 && entity.IsValid)
            {
                if (!entity.HasComponent<ExileCore.PoEMemory.Components.Map>()) return;
                
                var serverData = ingameState.ServerData;
                var bonusComp = serverData.BonusCompletedAreas;
                var awakeComp = serverData.AwakenedAreas;
                var comp = serverData.CompletedAreas;
                
                var modsComponent = entity.GetComponent<Mods>() ?? null;
                if (Settings.AlwaysShowTooltip || modsComponent != null && modsComponent.ItemRarity != ItemRarity.Normal && modsComponent.ItemMods.Count() > 0)
                {
                    List<Warning> activeWarnings = new List<Warning>();
                    nuVector4 nameCol = GetRarityColor(entity.GetComponent<Mods>().ItemRarity);
                    var mapComponent = entity.GetComponent<ExileCore.PoEMemory.Components.Map>();
                    int packSize = 0;
                    int quantity = entity.GetComponent<Quality>()?.ItemQuality ?? 0;
                    if(modsComponent != null && modsComponent.ItemRarity != ItemRarity.Unique)
                        foreach (var mod in modsComponent.ItemMods.Where(x =>
                                                            !x.Group.Contains("MapAtlasInfluence")
                                                            && !x.Group.Contains("MapElderContainsBoss")
                                                            && !x.Name.Equals("InfectedMap")
                                                            && !x.Name.Equals("MapZanaSubAreaMissionDetails")
                                                           ))
                        {
                            quantity += mod.Value1;
                            packSize += mod.Value3;
                            if (WarningDictionary.Where(x => mod.Name.Contains(x.Key)).Any())
                                activeWarnings.Add(WarningDictionary.Where(x => mod.Name.Contains(x.Key)).FirstOrDefault().Value);
                        }

                    if (Settings.AlwaysShowTooltip || activeWarnings.Count > 0 || Settings.ShowModCount || Settings.ShowQuantityPercent || Settings.ShowPackSizePercent)
                    {
                        // Get mouse position
                        nuVector2 mousePos = new nuVector2(MouseLite.GetCursorPositionVector().X + 24, MouseLite.GetCursorPositionVector().Y);
                        if (Settings.PadForNinjaPricer) mousePos = new nuVector2(MouseLite.GetCursorPositionVector().X + 24, MouseLite.GetCursorPositionVector().Y + 56);
                        // Parsing inventory, don't use mousePos
                        if(mode == 1) {
                            var framePos = ingameState.UIHover.Parent.Parent.GetClientRect().TopRight;
                            mousePos.X = framePos.X;
                            mousePos.Y = framePos.Y - 50 + mPad;
                        }
                        var _opened = true;
                        if (ImGui.Begin($"{entity.Address}", ref _opened,
                            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize |
                            ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoNavInputs))
                        if(mode == 1 || Settings.ShowMapName)
                            if (!Settings.ShowCompletion) ImGui.TextColored(nameCol, $"[T{mapComponent.Tier}] {entity.GetComponent<Base>().Name.Replace(" Map","")}");
                            else
                            {
                                ImGui.TextColored(nameCol, $"[T{mapComponent.Tier}] {entity.GetComponent<Base>().Name.Replace(" Map","")}");
                                if (!awakeComp.Contains(mapComponent.Area))
                                {
                                    ImGui.SameLine(); ImGui.TextColored(new nuVector4(1f, 0f, 0f, 1f), $"A");
                                }
                                if (!bonusComp.Contains(mapComponent.Area))
                                {
                                    ImGui.SameLine(); ImGui.TextColored(new nuVector4(1f, 0f, 0f, 1f), $"B");
                                }
                                if (!comp.Contains(mapComponent.Area))
                                {
                                    ImGui.SameLine(); ImGui.TextColored(new nuVector4(1f, 0f, 0f, 1f), $"C");
                                }
                                    ImGui.PushStyleColor(ImGuiCol.Separator, new nuVector4(1f, 1f, 1f, 0.2f));
                                    
                                }
                        if (mode == 1)
                        {
                            if(ZanaMods.TryGetValue(modsComponent.ItemMods.FirstOrDefault(x => x.Name == "MapZanaSubAreaMissionDetails").Value2, out string modName))
                                if(modName.Contains("Guardian"))
                                    ImGui.TextColored(new nuVector4(0.5f, 1f, 0.45f, 1f), $"{modName}");
                                else
                                    ImGui.TextColored(new nuVector4(0.9f, 0.85f, 0.65f, 1f), $"{modName}");
                            else 
                                ImGui.TextColored(new nuVector4(0.9f, 0.85f, 0.65f, 1f), $"Unknown Zana Mission: {modsComponent.ItemMods.FirstOrDefault(x => x.Name == "MapZanaSubAreaMissionDetails").Value2}");
                        }
                        // Quantity and Pack Size
                        nuVector4 qCol = new nuVector4(1f, 1f, 1f, 1f);
                        if (Settings.ColourQuantityPercent)
                            if (quantity < Settings.ColourQuantity) qCol = new nuVector4(1f, 0.4f, 0.4f, 1f); 
                            else qCol = new nuVector4(0.4f, 1f, 0.4f, 1f);
                        if (Settings.ShowQuantityPercent && quantity != 0 && Settings.ShowPackSizePercent && packSize != 0)
                        {
                            ImGui.TextColored(qCol, $"{quantity}%% Quant");
                            ImGui.SameLine(); ImGui.TextColored(new nuVector4(1f, 1f, 1f, 1f), $"{packSize}%% Pack Size");
                        }
                        else if (Settings.ShowQuantityPercent && quantity != 0)
                            ImGui.TextColored(qCol, $"{quantity}%% Quantity");
                        else if (Settings.ShowPackSizePercent && packSize != 0)
                            ImGui.TextColored(new nuVector4(1f, 1f, 1f, 1f), $"{packSize}%% Pack Size");
                        
                        // Separator
                        if(Settings.HorizontalLines && modsComponent.ItemMods.Count != 0) ImGui.Separator();
                        // Count Mods
                        if (Settings.ShowModCount && modsComponent.ItemMods.Count != 0)
                            if (entity.GetComponent<Base>().isCorrupted) ImGui.TextColored(new nuVector4(1f, 0.33f, 0.33f, 1f), $"{modsComponent.ItemMods.Count} Mods");
                            else ImGui.TextColored(new nuVector4(1f, 1f, 1f, 1f), $"{modsComponent.ItemMods.Count} Mods");
                        // Mod Warnings
                        if (Settings.ShowModWarnings)
                            foreach (Warning warning in activeWarnings.OrderBy(x => x.Color.ToString()).ToList())
                                ImGui.TextColored(warning.Color, warning.Text);
                        // Color background
                        ImGui.PushStyleColor(ImGuiCol.WindowBg, new System.Numerics.Vector4(0.256f, 0.256f, 0.256f, 1f));
                        // Detect and adjust for edges
                        var size = ImGui.GetWindowSize();
                        var pos = ImGui.GetWindowPos();
                        if ((mousePos.X + size.X) > windowArea.Width)
                        {
                            //ImGui.Text($"Overflow by {(mousePos.X + size.X) - windowArea.Width}");
                            ImGui.SetWindowPos(new nuVector2(mousePos.X - ((mousePos.X + size.X) - windowArea.Width) - 4, mousePos.Y + 24), ImGuiCond.Always);
                        }
                        else ImGui.SetWindowPos(mousePos, ImGuiCond.Always);

                        // padding when parsing an inventory
                        if(mode == 1) mPad += (int)size.Y + 2;
                        ImGui.End();
                    }
                }
            }
        }

        public override void Render()
        {
            if (!Settings.Enable) return;
            var uiHover = ingameState.UIHover;
            if (ingameState.UIHover.IsVisible)
            {
                var itemType = uiHover.AsObject<HoverItemIcon>()?.ToolTipType ?? null;
                // render normal items
                if (itemType != null && itemType != ToolTipType.ItemInChat && itemType != ToolTipType.None)
                {
                    var hoverItem = uiHover.AsObject<NormalInventoryItem>();
                    if(hoverItem.Tooltip.IsValid)
                        RenderItem(uiHover.AsObject<NormalInventoryItem>().Item, 0);
                }
                // render NPC inventory if relevant
                else if (itemType != null && itemType == ToolTipType.None)
                {
                    var serverData = ingameState.ServerData;
                    var npcInv = serverData.NPCInventories;
                    if (npcInv == null || npcInv.Count == 0) return;
                    foreach (var inv in npcInv)
                        if (uiHover.Parent.ChildCount == inv.Inventory.Items.Count)
                        {
                            mPad = 0;
                            foreach (var item in inv.Inventory.InventorySlotItems)
                                RenderItem(item.Item, 1);
                        }
                }
            }
        }
    }
}
