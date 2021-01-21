using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using System.Collections.Generic;

namespace MapNotify
{

    public partial class MapNotify
    {

        public List<WorldArea> AwakenedAreas => GetAreas(ingameState.ServerData.Address + 0x8518);
        public List<WorldArea> BonusAreas => GetAreas(ingameState.ServerData.Address + 0x84D8);
        public List<WorldArea> CompletedAreas => GetAreas(ingameState.ServerData.Address + 0x8498);
        public List<WorldArea> MavenAreas => GetAreas(ingameState.ServerData.Address + 0x8458);

        public List<WorldArea> GetAreas(long address)
        {
            if (address == 0)
                return new List<WorldArea>();

            var res = new List<WorldArea>();
            var size = ingameState.M.Read<int>(address - 0x8);
            var listStart = ingameState.M.Read<long>(address);
            var error = 0;

            if (listStart == 0 || size == 0)
                return res;

            for (var addr = ingameState.M.Read<long>(listStart); addr != listStart; addr = ingameState.M.Read<long>(addr))
            {
                if (addr == 0) return res;
                var byAddress = theGame.Files.WorldAreas.GetByAddress(ingameState.M.Read<long>(addr + 0x18));

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
