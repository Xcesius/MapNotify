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

        public static nuVector2 boxSize;
        public static float maxSize;
        public static float rowSize;
        public void RenderItem(NormalInventoryItem Item, Entity Entity, bool isInventory = false, int mapNum = 0)
        {
            var entity = Entity;
            var item = Item;
            if (entity.Address != 0 && entity.IsValid)
            {
                // Base and Class ID
                var baseType = gameController.Files.BaseItemTypes.Translate(entity.Path) ?? null;
                var classID = baseType.ClassName ?? string.Empty;
                // Not map, heist or watchstone or normal rarity heist
                if ((!entity.HasComponent<ExileCore.PoEMemory.Components.Map>() && !classID.Equals(string.Empty) &&
                    !entity.Path.Contains("BreachFragment") &&
                    !entity.Path.Contains("CurrencyElderFragment") &&
                    !entity.Path.Contains("ShaperFragment") &&
                    !entity.Path.Contains("VaalFragment2_") &&
                    !classID.Contains("HeistContract") && !classID.Contains("HeistBlueprint") &&
                    !classID.Contains("AtlasRegionUpgradeItem")) && 
                    !entity.Path.Contains("MavenMap") ||
                    (classID.Contains("HeistContract") || classID.Contains("HeistBlueprint")) && entity.GetComponent<Mods>()?.ItemRarity == ItemRarity.Normal) return;

                if (!Settings.ShowForHeist && (classID.Contains("HeistContract") || classID.Contains("HeistBlueprint"))) return;
                if (!Settings.ShowForWatchstones && classID.Contains("AtlasRegionUpgradeItem")) return;
                if (!Settings.ShowForInvitations && (classID.Contains("MavenMap") || classID.Contains("MiscMapItem"))) return;

                // Evaluate
                var ItemDetails = Entity.GetHudComponent<ItemDetails>() ?? new ItemDetails(Item, Entity);
                if (ItemDetails != null && Settings.AlwaysShowTooltip || ItemDetails.ActiveWarnings.Count > 0)
                {
                    // get alerts, watchstones and heists with no warned mods have no name to show
                    if ((classID.Contains("AtlasRegionUpgradeItem") ||
                        classID.Contains("HeistContract") ||
                        classID.Contains("HeistBlueprint")) &&
                        ItemDetails.ActiveWarnings.Count == 0) return;
                    // Get mouse position
                    nuVector2 boxOrigin = new nuVector2(MouseLite.GetCursorPositionVector().X + 24, MouseLite.GetCursorPositionVector().Y);

                    // Pad vertically as well if using ninja pricer tooltip
                    if (Settings.PadForNinjaPricer && ItemDetails.NeedsPadding)
                        boxOrigin = new nuVector2(MouseLite.GetCursorPositionVector().X + 24, MouseLite.GetCursorPositionVector().Y + 56);
                    // Personal pricer
                    if (Settings.PadForAltPricer && ItemDetails.NeedsPadding)
                        boxOrigin = new nuVector2(MouseLite.GetCursorPositionVector().X + 24, MouseLite.GetCursorPositionVector().Y + 30);

                    // Parsing inventory, don't use boxOrigin
                    if (isInventory)
                    {
                        // wrap on fourth
                        if ((float)mapNum % (float)4 == (float)0)
                        {
                            boxSize = new nuVector2(0, 0);
                            rowSize += maxSize + 2;
                            maxSize = 0;
                        }
                        var framePos = ingameState.UIHover.Parent.GetClientRect().TopRight;
                        framePos.X += 10 + boxSize.X;
                        framePos.Y -= 200;
                        boxOrigin = new nuVector2(framePos.X, framePos.Y + rowSize);

                    }

                    // create the imgui faux tooltip
                    var _opened = true;
                    // Color background
                    ImGui.PushStyleColor(ImGuiCol.WindowBg, 0xFF3F3F3F);
                    if (ImGui.Begin($"{entity.Address}", ref _opened,
                        ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize |
                        ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoNavInputs))
                    {
                        ImGui.BeginGroup();
                        if (!classID.Contains("HeistContract") &&
                            !classID.Contains("MapFragment") &&
                            !classID.Contains("HeistBlueprint") &&
                            !classID.Contains("AtlasRegionUpgradeItem") &&
                            !classID.Contains("QuestItem") && !classID.Contains("MiscMapItem"))
                        {
                            // map only stuff, zana always needs to show name for ease
                            if (isInventory || Settings.ShowMapName)
                            {
                                if (ItemDetails.LacksCompletion || !Settings.ShowCompletion)
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
                                    if (ItemDetails.MavenDetails.MavenCompletion)
                                    {
                                        ImGui.SameLine(); ImGui.TextColored(new nuVector4(0.9f, 0f, 0.77f, 1f), $"M");
                                    }
                                    if (ItemDetails.MavenDetails.MavenUncharted)
                                    {
                                        ImGui.SameLine(); ImGui.TextColored(new nuVector4(0.0f, 0.9f, 0.77f, 1f), $"U");
                                    }
                                    ImGui.PushStyleColor(ImGuiCol.Separator, new nuVector4(1f, 1f, 1f, 0.2f));
                                }
                                if (Settings.ShowMapRegion)
                                {
                                    nuVector4 regionColor = new nuVector4(1f, 1f, 1f, 1f);
                                    if (Settings.TargetRegions && CheckRegionTarget(ItemDetails.MapRegion))
                                        regionColor = new nuVector4(1f, 0f, 1f, 1f);
                                    ImGui.TextColored(regionColor, $"{ItemDetails.MapRegion}");
                                }
                            }
                        }

                        // Maven boss list
                        if (classID.Contains("QuestItem") || classID.Contains("MiscMapItem") || classID.Contains("MapFragment"))
                        {
                            ImGui.TextColored(new nuVector4(0.9f, 0f, 0.77f, 1f), $"{ItemDetails.MapName}");
                            foreach (var boss in ItemDetails.MavenDetails.MavenBosses)
                                if(boss.Complete)
                                    ImGui.TextColored(new nuVector4(0f, 1f, 0f, 1f), $"{boss.Boss}");
                                else
                                    ImGui.TextColored(new nuVector4(1f, 0.8f, 0.8f, 1f), $"{boss.Boss}");
                        } else if (ItemDetails.MavenDetails.MavenRegion != string.Empty && Input.GetKeyState(System.Windows.Forms.Keys.Menu))
                            foreach (var boss in ItemDetails.MavenDetails.MavenBosses)
                                if (boss.Complete)
                                    ImGui.TextColored(new nuVector4(0f, 1f, 0f, 1f), $"{boss.Boss}");
                                else
                                    ImGui.TextColored(new nuVector4(1f, 0.8f, 0.8f, 1f), $"{boss.Boss}");

                        // Zana Mod
                        if (isInventory)
                        {
                            ImGui.TextColored(SharpToNu(ItemDetails.ZanaMod.Color), $"{ItemDetails.ZanaMod?.Text ?? "Zana Mod was null!"}");

                        }

                        // Quantity and Packsize for maps
                        if (!classID.Contains("HeistContract") && !classID.Contains("HeistBlueprint") && !classID.Contains("AtlasRegionUpgradeItem"))
                        {
                            // Quantity and Pack Size
                            nuVector4 qCol = new nuVector4(1f, 1f, 1f, 1f);
                            if (Settings.ColourQuantityPercent)
                                if
                                    (ItemDetails.Quantity < Settings.ColourQuantity) qCol = new nuVector4(1f, 0.4f, 0.4f, 1f);
                                else
                                    qCol = new nuVector4(0.4f, 1f, 0.4f, 1f);
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
                            if (entity.GetComponent<Base>().isCorrupted)
                                ImGui.TextColored(new nuVector4(1f, 0.33f, 0.33f, 1f), $"{ItemDetails.ModCount} Mods");
                            else
                                ImGui.TextColored(new nuVector4(1f, 1f, 1f, 1f), $"{ItemDetails.ModCount} Mods");

                        // Mod StyledTexts
                        if (Settings.ShowModWarnings)
                            foreach (StyledText StyledText in ItemDetails.ActiveWarnings.OrderBy(x => x.Color.ToString()).ToList())
                                ImGui.TextColored(SharpToNu(StyledText.Color), StyledText.Text);
                        ImGui.EndGroup();

                        // border for most notable maps in inventory
                        if (isInventory && ItemDetails.ZanaBorder)
                        {
                            nuVector2 min = ImGui.GetItemRectMin();
                            min.X -= 8;
                            min.Y -= 8;
                            nuVector2 max = ImGui.GetItemRectMax();
                            max.X += 8;
                            max.Y += 8;
                            ImGui.GetForegroundDrawList().AddRect(min, max, 0xFF00CC00);
                        }

                        // Detect and adjust for edges
                        var size = ImGui.GetWindowSize();
                        var pos = ImGui.GetWindowPos();
                        if ((boxOrigin.X + size.X) > windowArea.Width)
                            ImGui.SetWindowPos(new nuVector2(boxOrigin.X - ((boxOrigin.X + size.X) - windowArea.Width) - 4, boxOrigin.Y + 24), ImGuiCond.Always);
                        else
                            ImGui.SetWindowPos(boxOrigin, ImGuiCond.Always);

                        // padding when parsing an inventory
                        if (isInventory)
                        {
                            boxSize.X += (int)size.X + 2;
                            if (maxSize < size.Y)
                                maxSize = size.Y;
                        }
                    }
                    ImGui.End();
                }
            }
        }
        
        public override void Render()
        {
            if (!Settings.Enable) return;

            if (ingameState.IngameUi.Atlas.IsVisible)
                AtlasRender();

            var uiHover = ingameState.UIHover;
            if (ingameState.UIHover?.IsVisible ?? false)
            {
                var itemType = uiHover.AsObject<HoverItemIcon>()?.ToolTipType ?? null;
                // render hovered item
                if (itemType != null && itemType != ToolTipType.ItemInChat && itemType != ToolTipType.None)
                {
                    var hoverItem = uiHover.AsObject<NormalInventoryItem>();
                    if (hoverItem.Item?.Path != null && (hoverItem.Tooltip?.IsValid ?? false))
                        RenderItem(hoverItem, hoverItem.Item);
                }
                // render NPC inventory if relevant
                else if (Settings.ShowForZanaMaps && itemType != null && itemType == ToolTipType.None)
                {
                    var npcInv = ingameState.ServerData.NPCInventories ?? null;
                    if (npcInv == null || npcInv.Count == 0) return;
                    foreach (var inv in npcInv)
                        if (uiHover.Parent.ChildCount == inv.Inventory.Items.Count)
                        {
                            boxSize = new nuVector2(0f, 0f);
                            maxSize = 0;
                            rowSize = 0;
                            int mapNum = 0;
                            foreach (var item in inv.Inventory.InventorySlotItems.OrderBy(x => x.PosY).OrderBy(x => x.PosX))
                            {
                                RenderItem(null, item.Item, true, mapNum);
                                mapNum++;
                            }
                        }
                }
            }
        }
    }
}