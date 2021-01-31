using ExileCore;
using ImGuiNET;
using System.Numerics;

namespace MapNotify
{
    public partial class MapNotify : BaseSettingsPlugin<MapNotifySettings>
    {
        public void AtlasRender()
        {
            if (Settings.TargetRegions)
            {
                ImGui.SetNextWindowPos(new Vector2(Settings.AtlasX, Settings.AtlasY), ImGuiCond.Once, Vector2.Zero);
                ImGui.Begin("TargettedRegions",
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoFocusOnAppearing |
                    ImGuiWindowFlags.AlwaysAutoResize |
                    ImGuiWindowFlags.NoScrollWithMouse |
                    ImGuiWindowFlags.NoCollapse);
                var pos = ImGui.GetWindowPos();
                Settings.AtlasX = (int)pos.X;
                Settings.AtlasY = (int)pos.Y;
                ImGui.Text("Targetted Regions");
                ImGui.Separator();
                ImGui.Checkbox("Haewark Hamlet", ref Settings.TargettingHaewarkHamlet);
                ImGui.Checkbox("Tirn's End", ref Settings.TargettingTirnsEnd);
                ImGui.Checkbox("Lex Proxima", ref Settings.TargettingLexProxima);
                ImGui.Checkbox("Lex Ejoris", ref Settings.TargettingLexEjoris);
                ImGui.Checkbox("New Vastir", ref Settings.TargettingNewVastir);
                ImGui.Checkbox("Glennach Cairns", ref Settings.TargettingGlennachCairns);
                ImGui.Checkbox("Valdo's Rest", ref Settings.TargettingValdosRest);
                ImGui.Checkbox("Lira Arthain", ref Settings.TargettingLiraArthain);
                ImGui.End();
            }
        }
        public bool CheckRegionTarget(string region)
        {
            switch (region)
            {
                case "Haewark Hamlet":
                    return Settings.TargettingHaewarkHamlet;
                case "Tirn's End":
                    return Settings.TargettingTirnsEnd;
                case "Lex Proxima":
                    return Settings.TargettingLexProxima;
                case "Lex Ejoris":
                    return Settings.TargettingLexEjoris;
                case "New Vastir":
                    return Settings.TargettingNewVastir;
                case "Glennach Cairns":
                    return Settings.TargettingGlennachCairns;
                case "Valdo's Rest":
                    return Settings.TargettingValdosRest;
                case "Lira Arthain":
                    return Settings.TargettingLiraArthain;
                default:
                    return false;
            }
        }
    }
}
