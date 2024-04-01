using ExileCore;
using System.Collections.Generic;

namespace MapNotify
{
    public partial class MapNotify : BaseSettingsPlugin<MapNotifySettings>
    {

        public static Dictionary<string, string> MavenDict = new Dictionary<string, string>();
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
            {"Lair of the Hydra","The Formed"},
            {"Maze of the Minotaur","The Formed"},
            {"Forge of the Phoenix","The Formed"},
            {"Pit of the Chimera","The Formed"},
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

        public static List<string> LacksCompletionList = new List<string>()
        {
            {"Lair of the Hydra"},
            {"Maze of the Minotaur"},
            {"Forge of the Phoenix"},
            {"Pit of the Chimera"},
            {"Rewritten Distant Memory"},
            {"Augmented Distant Memory"},
            {"Altered Distant Memory"},
            {"Twisted Distant Memory"},
            {"Cortex"},
            {"Replica Cortex"},
            {"Hall of Grandmasters"},
            {"Infused Beachhead"},
            {"The Beachhead"},
            {"Untainted Paradise"},
        };


        public static Dictionary<string, List<string>> RegionArea = new Dictionary<string, List<string>>()
        {
            { "The Twisted",  new List<string>(){ 
                "The Purifier",
                "The Constrictor",
                "The Enslaver",
                "The Eradicator" }
            },
            { "The Hidden",  new List<string>(){
                "Uul-Netol's Domain",
                "Xoph's Domain",
                "Tul's Domain",
                "Esh's Domain" }
            },
            { "The Formed",  new List<string>(){
                "Lair of the Hydra",
                "Maze of the Minotaur",
                "Forge of the Phoenix",
                "Pit of the Chimera" }
            },
            { "The Forgotten",  new List<string>(){
                "Rewritten Distant Memory",
                "Augmented Distant Memory",
                "Altered Distant Memory",
                "Twisted Distant Memory" }
            },
            { "The Feared",  new List<string>(){
                "Cortex",
                "Chayula's Domain",
                "The Alluring Abyss",
                "Absence of Value and Meaning",
                "The Shaper's Realm" }
            },
        };
        

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
    }
}
