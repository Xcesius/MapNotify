using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using SharpDX;
using System.Collections.Generic;
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
        private static GameController gameController;
        private static IngameState ingameState;
        public static Dictionary<string, StyledText> WarningDictionary;
        
        public override bool Initialise()
        {
            base.Initialise();
            Name = "Map Mod Notifications";
            windowArea = GameController.Window.GetWindowRectangle();
            WarningDictionary = LoadConfigs();
            gameController = GameController;
            ingameState = gameController.IngameState;
            BuildRegions();
            return true;
        }

        public int mPad;
        public void RenderItem(NormalInventoryItem Item, Entity Entity, bool isInventory)
        {
            var entity = Entity;
            var item = Item;
            if (entity.Address != 0 && entity.IsValid)
            {
                // Base and Class ID
                var baseType = gameController.Files.BaseItemTypes.Translate(entity.Path);
                var classID = baseType.ClassName;
                // Not map, heist or watchstone or normal rarity heist
                if ((!entity.HasComponent<ExileCore.PoEMemory.Components.Map>() 
                    && !classID.Contains("HeistContract") && !classID.Contains("HeistBlueprint") && !classID.Contains("AtlasRegionUpgradeItem")) && !entity.Path.Contains("MavenMap") || 
                    (classID.Contains("HeistContract") || classID.Contains("HeistBlueprint")) && entity.GetComponent<Mods>()?.ItemRarity == ItemRarity.Normal) return;

                // Evaluate
                var ItemDetails = Entity.GetHudComponent<ItemDetails>() ?? new ItemDetails(Item, Entity);
                if (Settings.AlwaysShowTooltip || ItemDetails.ActiveWarnings.Count > 0)
                {
                    // get alerts, watchstones and heists with no warned mods have no name to show
                    if ((classID.Contains("AtlasRegionUpgradeItem") || 
                        classID.Contains("HeistContract") ||
                        classID.Contains("HeistBlueprint") ||
                        classID.Contains("AtlasRegionUpgradeItem")) && 
                        ItemDetails.ActiveWarnings.Count == 0) return;

                    // Get mouse position
                    nuVector2 mousePos = new nuVector2(MouseLite.GetCursorPositionVector().X + 24, MouseLite.GetCursorPositionVector().Y);
                    // Pad vertically as well if using ninja pricer tooltip
                    if (Settings.PadForNinjaPricer && ItemDetails.NeedsPadding) 
                        mousePos = new nuVector2(MouseLite.GetCursorPositionVector().X + 24, MouseLite.GetCursorPositionVector().Y + 56);
                        // Parsing inventory, don't use mousePos
                        if (isInventory)
                        {
                            var framePos = ingameState.UIHover.Parent.Parent.GetClientRect().TopRight;
                            mousePos = new nuVector2(framePos.X, framePos.Y - 50 + mPad);
                        }

                    // create the imgui faux tooltip
                    var _opened = true;
                    if (ImGui.Begin($"{entity.Address}", ref _opened,
                        ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize |
                        ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoNavInputs))
                    {
                        if (!classID.Contains("HeistContract") && 
                            !classID.Contains("HeistBlueprint") && 
                            !classID.Contains("AtlasRegionUpgradeItem") &&
                            !classID.Contains("QuestItem") && !classID.Contains("MiscMapItem") )
                        {
                            // map only stuff, zana always needs to show name for ease
                            if (isInventory || Settings.ShowMapName)
                            {
                                if (!Settings.ShowCompletion)
                                    ImGui.TextColored(ItemDetails.ItemColour, $"{ItemDetails.MapName}");
                                else
                                {
                                    ImGui.TextColored(ItemDetails.ItemColour, $"{ItemDetails.MapName}");
                                    if (!ItemDetails.Awakened)
                                    {
                                        ImGui.SameLine(); ImGui.TextColored(new nuVector4(1f, 0f, 0f, 1f), $"A");
                                    }
                                    if (!ItemDetails.Bonus)
                                    {
                                        ImGui.SameLine(); ImGui.TextColored(new nuVector4(1f, 0f, 0f, 1f), $"B");
                                    }
                                    if (!ItemDetails.Completed)
                                    {
                                        ImGui.SameLine(); ImGui.TextColored(new nuVector4(1f, 0f, 0f, 1f), $"C");
                                    }
                                    if (ItemDetails.Maven)
                                    {
                                        ImGui.SameLine(); ImGui.TextColored(new nuVector4(0.9f, 0f, 0.77f, 1f), $"M");
                                    }
                                    ImGui.PushStyleColor(ImGuiCol.Separator, new nuVector4(1f, 1f, 1f, 0.2f));
                                }
                            }
                        }
                        if (classID.Contains("QuestItem") || classID.Contains("MiscMapItem"))
                        {
                            ImGui.TextColored(new nuVector4(0.9f, 0f, 0.77f, 1f), $"{ItemDetails.MapName}");
                            ImGui.Text($"{ItemDetails.MavenBosses}");
                        }
                        // Zana Mod
                        if (isInventory)
                            ImGui.TextColored(SharpToNu(ItemDetails.ZanaMod.Color), $"{ItemDetails.ZanaMod?.Text ?? "Zana Mod was null!"}");

                        // Quantity and Packsize for maps
                        if (!classID.Contains("HeistContract") && !classID.Contains("HeistBlueprint") && !classID.Contains("AtlasRegionUpgradeItem"))
                        {
                            // Quantity and Pack Size
                            nuVector4 qCol = new nuVector4(1f, 1f, 1f, 1f);
                            if (Settings.ColourQuantityPercent)
                                if (ItemDetails.Quantity < Settings.ColourQuantity) qCol = new nuVector4(1f, 0.4f, 0.4f, 1f);
                                else qCol = new nuVector4(0.4f, 1f, 0.4f, 1f);
                            if (Settings.ShowQuantityPercent && ItemDetails.Quantity != 0 && Settings.ShowPackSizePercent && ItemDetails.PackSize != 0)
                            {
                                ImGui.TextColored(qCol, $"{ItemDetails.Quantity}%% Quant");
                                ImGui.SameLine();
                                ImGui.TextColored(new nuVector4(1f, 1f, 1f, 1f), $"{ItemDetails.PackSize}%% Pack Size");
                            }
                            else if (Settings.ShowQuantityPercent && ItemDetails.Quantity != 0)
                                ImGui.TextColored(qCol, $"{ItemDetails.Quantity}%% Quantity");
                            else if (Settings.ShowPackSizePercent && ItemDetails.PackSize != 0)
                                ImGui.TextColored(new nuVector4(1f, 1f, 1f, 1f), $"{ItemDetails.PackSize}%% Pack Size");

                            // Separator
                            if (Settings.HorizontalLines && ItemDetails.ActiveWarnings.Count > 0 && (Settings.ShowModCount || Settings.ShowModWarnings))
                            {
                                if (Settings.ShowLineForZanaMaps && isInventory || !isInventory)
                                    ImGui.Separator();
                            }
                        }
                        // Count Mods
                        if (Settings.ShowModCount && ItemDetails.ModCount != 0 && !classID.Contains("AtlasRegionUpgradeItem"))
                            if (entity.GetComponent<Base>().isCorrupted) ImGui.TextColored(new nuVector4(1f, 0.33f, 0.33f, 1f), $"{ItemDetails.ModCount} Mods");
                            else ImGui.TextColored(new nuVector4(1f, 1f, 1f, 1f), $"{ItemDetails.ModCount} Mods");

                        // Mod StyledTexts
                        if (Settings.ShowModWarnings)
                            foreach (StyledText StyledText in ItemDetails.ActiveWarnings.OrderBy(x => x.Color.ToString()).ToList())
                                ImGui.TextColored(SharpToNu(StyledText.Color), StyledText.Text);

                        // Color background
                        ImGui.PushStyleColor(ImGuiCol.WindowBg, 0xFF3F3F3F);

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
                        if (isInventory) mPad += (int)size.Y + 2;

                    }
                    ImGui.End();
                }
                
            }
        }


        public override void Render()
        {



            if (!Settings.Enable) return;
            var uiHover = ingameState.UIHover;
            if (ingameState.UIHover?.IsVisible ?? false)
            {
                var itemType = uiHover.AsObject<HoverItemIcon>()?.ToolTipType ?? null;
                // render hovered item
                if (itemType != null && itemType != ToolTipType.ItemInChat && itemType != ToolTipType.None)
                {
                    var hoverItem = uiHover.AsObject<NormalInventoryItem>();
                    if (hoverItem.Item?.Path != null && (hoverItem.Tooltip?.IsValid ?? false))
                    {
                        RenderItem(hoverItem, hoverItem.Item, false);
                    }
                }
                // render NPC inventory if relevant
                else if (Settings.ShowForZanaMaps && itemType != null && itemType == ToolTipType.None)
                {
                    var serverData = ingameState.ServerData;
                    var npcInv = serverData.NPCInventories;
                    if (npcInv == null || npcInv.Count == 0) return;
                    foreach (var inv in npcInv)
                        if (uiHover.Parent.ChildCount == inv.Inventory.Items.Count)
                        {
                            mPad = 0;
                            foreach (var item in inv.Inventory.Items)
                                RenderItem(null, item, true);
                        }
                }
            }
        }
    }
}