using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using System.Numerics;
namespace MapNotify
{
    partial class MapNotify
    {
        public bool debug;
        public bool maven;
        public static List<string> hoverMods = new List<string>();
        public static void HelpMarker(string desc)
        {
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }
        public static int IntSlider(string labelString, RangeNode<int> setting)
        {
            var refValue = setting.Value;
            ImGui.SliderInt(labelString, ref refValue, setting.Min, setting.Max);
            return refValue;
        }

        public static bool Checkbox(string labelString, bool boolValue)
        {
            ImGui.Checkbox(labelString, ref boolValue);
            return boolValue;
        }

        public static void DebugHover()
        {
            var uiHover = ingameState.UIHover ?? null;
            if (uiHover == null || !uiHover.IsVisible) return;
            var inventoryItemIcon = uiHover?.AsObject<NormalInventoryItem>() ?? null;
            if (inventoryItemIcon == null) return;
            var tooltip = inventoryItemIcon?.Tooltip ?? null;
            var entity = inventoryItemIcon?.Item ?? null;
            if (tooltip != null && entity.Address != 0 && entity.IsValid)
            {
                var modsComponent = entity.GetComponent<Mods>() ?? null;
                if (modsComponent == null) hoverMods.Clear();
                else if (modsComponent != null && modsComponent.ItemMods.Count() > 0) 
                {
                    hoverMods.Clear();
                    List<ItemMod> itemMods = modsComponent?.ItemMods ?? null;
                    if (itemMods == null || itemMods.Count == 0)
                    {
                        hoverMods.Clear();
                        return;
                    }
                    foreach (var mod in itemMods)
                    {
                        if (!hoverMods.Contains($"{mod.RawName} : {mod.Value1}, {mod.Value2}, {mod.Value3}, {mod.Value4}")) 
                            hoverMods.Add($"{mod.RawName} : {mod.Value1}, {mod.Value2}, {mod.Value3}, {mod.Value4}");
                    }
                }
            } else
            {
                hoverMods.Clear();
            }
        }

        public override void DrawSettings()
        {
            ImGui.Text("Plugin by Lachrymatory. https://github.com/Lachrymatory/MapNotify/");
            ImGui.Text("Please give suggestions, report issues, etc. below:");
            if (ImGui.Button("Lachrymatory's GitHub Repo.")) 
                System.Diagnostics.Process.Start("https://github.com/Lachrymatory/MapNotify/");
            ImGui.Separator();


            if (ImGui.TreeNodeEx("Core Settings", ImGuiTreeNodeFlags.CollapsingHeader))
            {
                Settings.AlwaysShowTooltip.Value = Checkbox("Show Tooltip Even Without Warnings", Settings.AlwaysShowTooltip);
                ImGui.SameLine(); HelpMarker("This will show a tooltip even if there are no mods to warn you about on the map.\nThis means you will always be able to see tier, completion, quantity, mod count, etc.");
                Settings.HorizontalLines.Value = Checkbox("Show Horizontal Lines", Settings.HorizontalLines);
                ImGui.SameLine(); HelpMarker("Add a Horizontal Line above actual mod information.");
                Settings.ShowForZanaMaps.Value = Checkbox("Display for Zana Missions", Settings.ShowForZanaMaps);
                Settings.ShowLineForZanaMaps.Value = Checkbox("Display Horizontal Line in Zana Missions Info", Settings.ShowLineForZanaMaps);
                Settings.ShowForWatchstones.Value = Checkbox("Display for Watchstones", Settings.ShowForWatchstones);
                Settings.ShowForHeist.Value = Checkbox("Display for Contracts and Blueprints", Settings.ShowForHeist);
                Settings.ShowForInvitations.Value = Checkbox("Display for Maven Invitations", Settings.ShowForInvitations);
            }

            if (ImGui.TreeNodeEx("Map Hover Settings", ImGuiTreeNodeFlags.CollapsingHeader))
            {
                Settings.ShowMapName.Value = Checkbox("Show Map Name", Settings.ShowMapName);
                Settings.ShowCompletion.Value = Checkbox("Show Completion Status", Settings.ShowCompletion);
                if (Settings.ShowCompletion) Settings.ShowMapName.Value = true;
                ImGui.SameLine(); HelpMarker("Requires map names.\nDisplays a red letter for each missing completion.\nA for Awakened Completion\nB for Bonus Completion\nC for Completion.");
                Settings.ShowMapRegion.Value = Checkbox("Show Region Name", Settings.ShowMapRegion);
                Settings.TargetRegions.Value = Checkbox("Enable Region Targetting ", Settings.TargetRegions);
                ImGui.SameLine(); HelpMarker("Open the Atlas and tick the regions you want to highlight. Requires Show Region Name.");
                if (Settings.TargetRegions) Settings.ShowMapRegion.Value = true;
                Settings.ShowModWarnings.Value = Checkbox("Show Mod Warnings", Settings.ShowModWarnings);
                ImGui.SameLine(); HelpMarker("Configured in 'ModWarnings.txt' in the plugin folder, created if missing.");
                Settings.ShowModCount.Value = Checkbox("Show Number of Mods on Map", Settings.ShowModCount);
                Settings.ShowPackSizePercent.Value = Checkbox("Show Pack Size %", Settings.ShowPackSizePercent);
                Settings.ShowQuantityPercent.Value = Checkbox("Show Item Quantity %", Settings.ShowQuantityPercent);
                Settings.ColourQuantityPercent.Value = Checkbox("Warn Below Quantity Percentage", Settings.ColourQuantityPercent);
                Settings.ColourQuantity.Value = IntSlider("##ColourQuantity", Settings.ColourQuantity);
                ImGui.SameLine(); HelpMarker("The colour of the quantity text will be red below this amount and green above it.");
            }
            
            if (ImGui.TreeNodeEx("Config Files and Other", ImGuiTreeNodeFlags.CollapsingHeader))
            {
                if (ImGui.Button("Reload Warnings Text Files")) WarningDictionary = LoadConfigs();
                if (ImGui.Button("Recreate Default Warnings Text Files")) ResetConfigs();
                ImGui.SameLine(); HelpMarker("This will irreversibly delete all your existing warnings config files!");
                Settings.PadForNinjaPricer.Value = Checkbox("Pad for Ninja Pricer", Settings.PadForNinjaPricer);
                ImGui.SameLine(); HelpMarker("This will move the tooltip down vertically to allow room for the Ninja Pricer tooltip to be rendered. Only needed with that plugin active.");
                Settings.PadForAltPricer.Value = Checkbox("Pad for Personal Pricer", Settings.PadForAltPricer);
                ImGui.SameLine(); HelpMarker("It's unlikely you'll need this.");
                ImGui.Spacing();
                               
                debug = Checkbox("Debug Features", debug);
                ImGui.SameLine(); HelpMarker("Show mod names for quickly adding them to your ModWarnings.txt\nYou only need the start of a mod to match it, for example: 'MapBloodlinesModOnMagicsMapWorlds' would be matched with:\nMapBloodlines;Bloodlines;FF7F00FF");
                if (debug)
                {
                    foreach (var region in RegionArea)
                    {
                        ImGui.Text($"{region.Key} ----");
                        foreach (string name in region.Value)
                            ImGui.Text(name);
                    }
                    maven = Checkbox("Maven Debug", maven);
                    if (maven)
                    {
                        ImGui.Text("Maven Witnessed:");
                        foreach (var map in MavenAreas)
                        {
                            ImGui.TextColored(new Vector4(0.5F, 0.5F, 1.2F, 1F), $"{map.Name}");
                        }
                        ImGui.Text("Maven Regions:");
                        foreach (var region in MavenDict)
                        {

                            ImGui.TextColored(new Vector4(0.5F, 0.5F, 1.2F, 1F), $"{region.Key}");
                            ImGui.SameLine();
                            ImGui.TextColored(new Vector4(1.2F, 0.5F, 0.5F, 1F), $"{region.Value}");
                        }
                    }
                    DebugHover();
                    ImGui.Text("Last Hovered item's mods:");
                    if (hoverMods.Count > 0)
                        foreach (var mod in hoverMods)
                        {
                            ImGui.TextColored(new Vector4(0.5F, 0.5F, 1.2F, 1F), mod);
                        }

                }
            }
        }
    }
}
