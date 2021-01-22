using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using System.Collections.Generic;

namespace MapNotify
{
    public partial class MapNotify : BaseSettingsPlugin<MapNotifySettings>
    {
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
    }
}
