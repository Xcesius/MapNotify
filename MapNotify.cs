using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.AtlasHelper;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ImGuiNET;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.Elements.InventoryElements;
using nuVector2 = System.Numerics.Vector2;
using nuVector4 = System.Numerics.Vector4;

namespace MapNotify
{
    public class MapNotify : BaseSettingsPlugin<MapNotifySettings>
    {
        public override bool Initialise()
        {
            base.Initialise();
            Name = "Map Mod Notifications";
            WarningDictionary = LoadConfig(Path.Combine(DirectoryFullName, "ModWarnings.txt"));
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
            return new nuVector4(color.R, color.G, color.B, color.A);
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
# NO REGEN
NoLifeESRegen;No Regen;FF007FFF
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
MapMonsterColdDamage;Extra Phys as Cold;7F00FFFF
MapMonsterFireDamage;Extra Phys as Fire;7F00FFFF
MapMonsterLightningDamage;Extra Phys as Lightning;7F00FFFF
MapMonsterLife;More Monster Life;7F00FFFF
MapMonsterFast;Monster Speed;7F00FFFF";
            File.WriteAllText(path, outFile);
            #endregion
        }
        Dictionary<string, Warning> WarningDictionary = new Dictionary<string, Warning>();
        
        public override void Render()
        {
            if (!Settings.Enable) return;
            var uiHover = GameController.Game.IngameState.UIHover;
            if (GameController.Game.IngameState.UIHover.IsVisible)
            {
                var itemType = uiHover.AsObject<HoverItemIcon>()?.ToolTipType ?? null;
                if (itemType != null && itemType != ToolTipType.ItemInChat)
                {
                    var inventoryItemIcon = uiHover.AsObject<NormalInventoryItem>();
                    var tooltip = inventoryItemIcon.Tooltip;
                    var entity = inventoryItemIcon.Item;
                    if (tooltip != null && entity.Address != 0 && entity.IsValid)
                    {
                        if (!entity.HasComponent<ExileCore.PoEMemory.Components.Map>()) return;
                        var modsComponent = entity.GetComponent<Mods>() ?? null;
                        if (modsComponent != null && modsComponent.ItemRarity != ItemRarity.Unique && modsComponent.ItemMods.Count() > 0)
                        {
                            List<Warning> activeWarnings = new List<Warning>();
                            int packSize = 0;
                            int quantity = entity.GetComponent<Quality>()?.ItemQuality ?? 0;
                            foreach (var mod in modsComponent.ItemMods.Where(x => !x.Group.Contains("MapAtlasInfluence") && !x.Name.Equals("InfectedMap")))
                            {
                                quantity += mod.Value1;
                                packSize += mod.Value3;
                                if (WarningDictionary.Where(x => mod.Name.Contains(x.Key)).Any()) {
                                    activeWarnings.Add(WarningDictionary.Where(x => mod.Name.Contains(x.Key)).FirstOrDefault().Value);
                                }
                            }
                            
                            if(activeWarnings.Count > 0 || Settings.ShowModCount || Settings.ShowQuantityPercent || Settings.ShowPackSizePercent) {
                                nuVector2 mousePos = new nuVector2(MouseLite.GetCursorPositionVector().X + 24, MouseLite.GetCursorPositionVector().Y);
                                if (Settings.PadForNinjaPricer) mousePos = new nuVector2(MouseLite.GetCursorPositionVector().X + 24, MouseLite.GetCursorPositionVector().Y + 56);
                                ImGui.SetNextWindowPos(mousePos, ImGuiCond.Always, nuVector2.Zero);
                                var _opened = true;
                                if (ImGui.Begin($"{Name}", ref _opened,
                                    ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize |
                                    ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoSavedSettings |
                                    ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoNavInputs))
                                if (Settings.ShowModCount) ImGui.TextColored(new nuVector4(1, 1, 1, 1), $"{modsComponent.ItemMods.Count} Total Mods");
                                if (Settings.ShowQuantityPercent) ImGui.TextColored(new nuVector4(1, 1, 1, 1), $"{quantity}%% Quantity");
                                if (Settings.ShowPackSizePercent) ImGui.TextColored(new nuVector4(1, 1, 1, 1), $"{packSize}%% Pack Size");
                                if (Settings.ShowModWarnings) foreach (Warning warning in activeWarnings) ImGui.TextColored(warning.Color, warning.Text);
                                ImGui.PushStyleColor(ImGuiCol.WindowBg, new System.Numerics.Vector4(0.256f, 0.256f, 0.256f, 1f));
                                ImGui.End();
                            }
                        }
                    }
                }
            }
        }
    }
}
