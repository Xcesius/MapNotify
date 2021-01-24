using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using System.Collections.Generic;

namespace MapNotify
{
    public partial class MapNotify : BaseSettingsPlugin<MapNotifySettings>
    {

        public static Dictionary<string, string> AreaRegion = new Dictionary<string, string>()
        {
            {"The Purifier","The Twisted"},
            {"The Constrictor","The Twisted"},
            {"The Enslaver","The Twisted"},
            {"The Eradicator","The Twisted"},
            {"Uul-Netol's Domain","The Hidden"},
            {"Xoph's Domain","The Hidden"},
            {"Tul's Domain","The Hidden"},
            {"Esh's Domain","The Hidden"},
            {"Lair of the Hydra Map","The Formed"},
            {"Maze of the Minotaur Map","The Formed"},
            {"Forge of the Phoenix Map","The Formed"},
            {"Pit of the Chimera Map","The Formed"},
            {"Rewritten Distant Memory","The Forgotten"},
            {"Augmented Distant Memory","The Forgotten"},
            {"Altered Distant Memory","The Forgotten"},
            {"Twisted Distant Memory","The Forgotten"},
            {"Cortex","The Feared"},
            {"Chayula's Domain","The Feared"},
            {"The Alluring Abyss","The Feared"},
            {"The Shaper's Realm","The Feared"},
            {"Absence of Value and Meaning","The Feared"},
        };
        public static Dictionary<string, string> MavenDict = new Dictionary<string, string>();
        public static List<WorldArea> AwakenedAreas => GetAreas(ingameState.ServerData.Address + 0x8510);
        public static List<WorldArea> BonusAreas => GetAreas(ingameState.ServerData.Address + 0x84D0);
        public static List<WorldArea> CompletedAreas => GetAreas(ingameState.ServerData.Address + 0x8490);
        public static List<WorldArea> MavenAreas => GetAreas(ingameState.ServerData.Address + 0x8450);

        private static List<WorldArea> GetAreas(long address)
        {
            if (address == 0)
                return new List<WorldArea>();

            var res = new List<WorldArea>();
            var size = ingameState.M.Read<int>(address);
            var listStart = ingameState.M.Read<long>(address + 0x08);
            var error = 0;

            if (listStart == 0 || size == 0)
                return res;

            for (var addr = ingameState.M.Read<long>(listStart); addr != listStart; addr = ingameState.M.Read<long>(addr))
            {
                if (addr == 0) return res;
                var byAddress = gameController.Files.WorldAreas.GetByAddress(ingameState.M.Read<long>(addr + 0x18));

                if (byAddress != null)
                    res.Add(byAddress);

                if (--size < 0) break;
                error++;

                //Sometimes wrong offsets and read 10000000+ objects
                if (error > 2048)
                {
                    res = new List<WorldArea>();
                    break;
                }
            }

            return res;
        }

        public static Dictionary<string, string> RegionReadable = new Dictionary<string, string>()
        {
            { "OutsideTopLeft", "Haewark Hamlet" },
            { "InsideTopLeft", "Tirn's End" },
            { "InsideTopRight", "Lex Proxima" },
            { "OutsideTopRight", "Lex Ejoris" },
            { "OutsideBottomLeft", "New Vastir" },
            { "InsideBottomLeft", "Glennach Cairns" },
            { "InsideBottomRight", "Valdo's Rest" },
            { "OutsideBottomRight", "Lira Arthain" },
            { "???", "Unknown" },
        };

        public void BuildRegions()
        {
            foreach (var node in gameController.Files.AtlasNodes.EntriesList)//.Where(x => x.Area.Name.Contains("Channel")))
            {
                long regionAddr = ingameState.M.Read<long>(node.Address + 0x4D);
                long regionNameAddr = ingameState.M.Read<long>(regionAddr);
                string regionName = ingameState.M.ReadStringU(regionNameAddr);

                if (RegionReadable.TryGetValue(regionName, out string regionReadable))
                {
                    AreaRegion.Add(node.Area.Name, regionReadable);
                } else
                {
                    LogMessage($"Failed to get readable name for: {regionName}");
                }
            }
        }
    }
}
