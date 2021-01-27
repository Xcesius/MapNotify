using ExileCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MapNotify
{
    public partial class MapNotify : BaseSettingsPlugin<MapNotifySettings>
    {

        public void ResetConfigs()
        {
            LogMessage("Deleting existing config files...");
            File.Delete(Path.Combine(DirectoryFullName, "ModWarnings.txt"));
            File.Delete(Path.Combine(DirectoryFullName, "SextantWarnings.txt"));
            File.Delete(Path.Combine(DirectoryFullName, "HeistWarnings.txt"));
            File.Delete(Path.Combine(DirectoryFullName, "WatchstoneWarnings.txt"));
            WarningDictionary = LoadConfigs();
        }
        public Dictionary<string, StyledText> LoadConfigs()
        {
            Dictionary<string, StyledText> FullDict = new Dictionary<string, StyledText>();
            LoadConfig(Path.Combine(DirectoryFullName, "ModWarnings.txt")).ToList().ForEach(x => FullDict.Add(x.Key, x.Value));
            LoadConfig(Path.Combine(DirectoryFullName, "SextantWarnings.txt")).ToList().ForEach(x => FullDict.Add(x.Key, x.Value));
            LoadConfig(Path.Combine(DirectoryFullName, "HeistWarnings.txt")).ToList().ForEach(x => FullDict.Add(x.Key, x.Value));
            LoadConfig(Path.Combine(DirectoryFullName, "WatchstoneWarnings.txt")).ToList().ForEach(x => FullDict.Add(x.Key, x.Value));
            LogMessage("Loaded config files...");
            return FullDict;
        }
        public Dictionary<string, StyledText> LoadConfig(string path)
        {
            if (!File.Exists(path))
                if (path.Contains("ModWarnings"))
                    CreateModConfig(path);
                else if (path.Contains("SextantWarnings"))
                    CreateSextantConfig(path);
                else if (path.Contains("WatchstoneWarnings"))
                    CreateWatchstonesConfig(path);
                else if (path.Contains("HeistWarnings"))
                    CreateHeistConfig(path);

            return GenDictionary(path).ToDictionary(line => line[0], line =>
            {
                var preloadAlerConfigLine = new StyledText { Text = line[1], Color = HexToSDXVector4(line[2]) };
                return preloadAlerConfigLine;
            });
        }

        public IEnumerable<string[]> GenDictionary(string path)
        {
            return File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)
            && line.IndexOf(';') >= 0
            && !line.StartsWith("#")).Select(line => line.Split('#')[0].Split(new[] { ';' }, 3).Select(parts => parts.Trim()).ToArray());
        }

        public void CreateModConfig(string path)
        {
            #region Create Default Map ModConfig
            new FileInfo(path).Directory.Create();
            string outFile =
@"#Contains;Name in tooltip;RGBA colour code
# REFLECT
ElementalReflect;Elemental Reflect;FF0000FF
PhysicalReflect;Physical Reflect;FF0000FF
# LEECH
MapMonsterLifeLeechImmunity;Cannot Leech Life or Mana;FF0000FF
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
            LogMessage("Created ModWarnings.txt...");
            #endregion
        }

        public void CreateHeistConfig(string path)
        {
            #region Create Default Heist Config
            new FileInfo(path).Directory.Create();
            string outFile =
@"# CONTRACT MODS
HeistContractNemesisModOnRares;Nemesis;FF7F00FF
HeistContractMonsterPhysicalResistance;Monster Physical Resistance;FF007FFF
HeistContractMonsterCannotBeStunned;Monsters cannot be Stunned;FF007FFF
HeistContractMonsterLife;Monster Life;FF007FFF
HeistContractMonsterDamage;Monster damage;FF007FFF
HeistContractMonsterFast;Monster Speed;FF007FFF
HeistContractMonsterPhysicalReflection;Physical Reflect;FF0000FF
HeistContractMonsterElementalReflection;Elemental Reflect;FF0000FF
HeistContractMonsterMultipleProjectiles;Multiple Projectiles;FF007FFF
HeistContractMonsterChain;Chaining;FF007FFF
HeistContractPlayerManaReducedRegenAlertLevel;Less Mana Regen with Alert;FF007FFF
HeistContractPlayerLifeReducedRegenAlertLevel;Less Life Regen with Alert;FF007FFF
HeistContractPlayerEnergyShieldReducedRegenAlertLevel;Less ES Regen with Alert;FF007FFF
HeistContractPlayerReducedRegen;Less Regen;FF007FFF
HeistContractPlayerNoLifeESRegen_;No Regen;FF007FFF
HeistContractPlayerElementalEquilibrium;Ele Equilibirum;FF00FFFF
HeistContractPlayerMaxResists;Max Res Down;FF007FFF
HeistContractPlayerMaxResistsAlertLevel;Max Res Down with Alert;FF007FFF
HeistContractDesecratedGround;Desecrated Ground;CCCC00FF
HeistContractBurningGround;Burning Ground;CCCC00FF
HeistContractShockedGround;Shocked Ground;CCCC00FF
HeistContractChilledGround;Chilled Ground;CCCC00FF
HeistContractMonstersCurseEffectOnSelfFinal;Less Curse Effectiveness;FF007FFF
HeistContractHexproof;Hexproof;FF007FFF
HeistContractPoisoning;Monsters Poison on Hit;FF007FFF
HeistContractMonstersAllResistances;Monster Res;FF007FFF
HeistContractBloodlinesModOnMagics_;Bloodlines;FF7F00FF
HeistContractMonstersAllDamageAlwaysIgnites_;All Hits Ignite;FF007FFF
HeistContractMonsterAreaOfEffect;Monster AoE;FF007FFF
HeistContractPlayerCurseElementalWeakness;Ele Weakness;FF00FFFF
HeistContractPlayerCurseVulnerability;Vulnerability;FF00FFFF
HeistContractPlayerCurseEnfeeblement;Enfeeblement;FF00FFFF
HeistContractPlayerCurseTemporalChains;Temporal Chains;FF00FFFF
HeistContractPlayersPointBlank;Player has Point Blank;FF00FFFF
HeistContractPlayersBlockAndArmour;Less Armour and Blocking;FF007FFF
HeistContractPlayerArmourAlertLevel;Less Armor with Alert;FF007FFF
HeistContractMonsterAccuracyPlayersUnlockyDodge;Unlucky Dodging;FF007FFF
HeistContractPlayerEvasionAlertLevel;Less Evasion with Alert;FF007FFF
HeistContractPlayersAoERadius;Player Less AoE;FF007FFF
HeistContractMonsterPatrolAdditionalElite;Elite Spawn Chance;FF007FFF
HeistContractObjectiveSpeed;Reduced Job Speed;FF007FFF
HeistContractMonsterRewardRoomDamage;Reward Room Damage;FF007FFF
HeistContractSideAreaIncreasedMonsters;More Reward Room Monsters;FF007FFF
HeistContractReinforcementsFast;Reinforcement Speed;FF007FFF
HeistContractLockdownInstant;Lockdown is Instant;FF0000FF
HeistContractTotalCost;Raised Heist Fee;FF0000FF
HeistContractChestAlertLevel;High Alert from Chests;FF0000FF
HeistContractMonsterAlertLevel;High Alert from Monsters;FF0000FF
HeistContractNoGangCut;No Cut Taken;00FF00FF
HeistContractNoTravelCost;No Travel Cost;00FF00FF
HeistContractGainAlertLevelPassive;Alert Increases over Time;FF0000FF
HeistContractGuardsLoseAlertLevel;Killing Lowers Alert;00FF00FF
HeistContractPlayerFireResistAlertLevel;Less Player Fire Res with Alert;FF0000FF
HeistContractPlayerColdResistAlertLevel;Less Player Cold Res with Alert;FF007FFF
HeistContractPlayerLightningResistAlertLevel;Less Player Lightning Res with Alert;FF007FFF
HeistContractMonsterFireDamage;Extra Fire Damage;FF007FFF
HeistContractMonsterColdDamage;Extra Cold Damage;FF007FFF
HeistContractMonsterLightningDamage;Extra Lightning Damage;FF007FFF";
            File.WriteAllText(path, outFile);
            LogMessage("Created HeistWarnings.txt...");
            #endregion
        }
        public void CreateWatchstonesConfig(string path)
        {
            #region Create Default Watchstones Config
            new FileInfo(path).Directory.Create();
            string outFile =
@"#Contains;Name in tooltip;RGBA colour code
WatchstoneItemQuantity1_;T3 Item Quantity;00FF00FF# (1 - 2)% increased Quantity of Items found in Areas
WatchstoneItemQuantity2___;T2 Item Quantity;00FF00FF# (3 - 4)% increased Quantity of Items found in Areas
WatchstoneItemQuantity3;T1 Item Quantity;00FF00FF# (5 - 6)% increased Quantity of Items found in Areas
WatchstoneLegionFragmentChance1;T4 Splinters to Emblems;00FF00FF# Timeless Splinters dropped by Legion Monsters in Areas have (0.02 - 0.03)% chance to drop as Timeless Emblems instead
WatchstoneLegionFragmentChance2_;T3 Splinters to Emblems;00FF00FF# Timeless Splinters dropped by Legion Monsters in Areas have (0.04 - 0.05)% chance to drop as Timeless Emblems instead
WatchstoneLegionFragmentChance3;T2 Splinters to Emblems;00FF00FF# Timeless Splinters dropped by Legion Monsters in Areas have (0.06 - 0.07)% chance to drop as Timeless Emblems instead
WatchstoneLegionFragmentChance4;T1 Splinters to Emblems;00FF00FF# Timeless Splinters dropped by Legion Monsters in Areas have (0.08 - 0.1)% chance to drop as Timeless Emblems instead
WatchstoneFullyLinkedChance1;T3 Fully Linked Drops;00FF00FF# Items have 0.1% chance to drop fully Linked in Areas
WatchstoneFullyLinkedChance2;T2 Fully Linked Drops;00FF00FF# Items have 0.2% chance to drop fully Linked in Areas
WatchstoneFullyLinkedChance3_;T1 Fully Linked Drops;00FF00FF# Items have 0.3% chance to drop fully Linked in Areas
WatchstoneMaximumSocketsChance1_;T3 Max Socket Drops;00FF00FF# Items found in Areas have (0.1 - 0.2)% chance to have the maximum number of Sockets
WatchstoneMaximumSocketsChance2;T2 Max Socket Drops;00FF00FF# Items found in Areas have (0.3 - 0.4)% chance to have the maximum number of Sockets
WatchstoneMaximumSocketsChance3;T1 Max Socket Drops;00FF00FF# Items found in Areas have 0.5% chance to have the maximum number of Sockets
WatchstoneWhiteSocketChance1__;T3 White Socket Drops;00FF00FF# Items found in Areas have (0.16 - 0.2)% chance to have a White Socket
WatchstoneWhiteSocketChance2;T2 White Socket Drops;00FF00FF# Items found in Areas have (0.21 - 0.25)% chance to have a White Socket
WatchstoneWhiteSocketChance3;T1 White Socket Drops;00FF00FF# Items found in Areas have (0.26 - 0.35)% chance to have a White Socket
WatchstoneBossCurrencyDrops1;T1 Boss Currency;00FF00FF# Unique Bosses drop an additional Basic Currency Item
WatchstoneRareMapCorruptedChance1;T3 Maps Drop Corrupted Chance;00FF00FF# Rare Maps found in Areas have (4 - 6)% chance to be Corrupted
WatchstoneRareMapCorruptedChance2;T2 Maps Drop Corrupted Chance;00FF00FF# Rare Maps found in Areas have (7 - 10)% chance to be Corrupted
WatchstoneRareMapCorruptedChance3_;T1 Maps Drop Corrupted Chance;00FF00FF# Rare Maps found in Areas have (11 - 15)% chance to be Corrupted
WatchstoneInfluenceItemChance1;T3 Influenced Items;00FF00FF# Items found in Areas have (5 - 15)% increased chance to be Influenced
WatchstoneInfluenceItemChance2_;T2 Influenced Items;00FF00FF# Items found in Areas have (16 - 30)% increased chance to be Influenced
WatchstoneInfluenceItemChance3_;T1 Influenced Items;00FF00FF# Items found in Areas have (31 - 50)% increased chance to be Influenced
WatchstoneCorruptedItemChance1;T2 Corrupted Items;00FF00FF# Found Items have 1% chance to drop Corrupted in Areas
WatchstoneCorruptedItemChance2;T1 Corrupted Items;00FF00FF# Found Items have 2% chance to drop Corrupted in Areas
WatchstoneInfluenceMonsterCurrencyChance1;T3 Influenced Monster Basic Currency;00FF00FF# Influenced Monsters in Areas have 1% chance to drop an additional Basic Currency Item
WatchstoneInfluenceMonsterCurrencyChance2_;T2 Influenced Monster Basic Currency;00FF00FF# Influenced Monsters in Areas have 2% chance to drop an additional Basic Currency Item
WatchstoneInfluenceMonsterCurrencyChance3;T1 Influenced Monster Basic Currency;00FF00FF# Influenced Monsters in Areas have 3% chance to drop an additional Basic Currency Item
WatchstoneInfluenceMonsterGemChance1;T3 Influenced Monster Quality Gem;00FF00FF# Influenced Monsters in Areas have 1% chance to drop an additional Gem with Quality
WatchstoneInfluenceMonsterGemChance2_;T2 Influenced Monster Quality Gem;00FF00FF# Influenced Monsters in Areas have 2% chance to drop an additional Gem with Quality
WatchstoneInfluenceMonsterGemChance3_;T1 Influenced Monster Quality Gem;00FF00FF# Influenced Monsters in Areas have 3% chance to drop an additional Gem with Quality
WatchstoneMagicMonsterCurrencyChance1;T3 Magic Monster Basic Currency;00FF00FF# Magic Monsters in Areas have (0.4 - 0.5)% chance to drop an additional Basic Currency Item
WatchstoneMagicMonsterCurrencyChance2;T2 Magic Monster Basic Currency;00FF00FF# Magic Monsters in Areas have (0.6 - 0.7)% chance to drop an additional Basic Currency Item
WatchstoneMagicMonsterCurrencyChance3_;T1 Magic Monster Basic Currency;00FF00FF# Magic Monsters in Areas have (0.8 - 1)% chance to drop an additional Basic Currency Item
WatchstoneMagicMonsterGemChance1__;T3 Magic Monster Quality Gem;00FF00FF# Magic Monsters in Areas have (0.4 - 0.5)% chance to drop an additional Gem with Quality
WatchstoneMagicMonsterGemChance2;T2 Magic Monster Quality Gem;00FF00FF# Magic Monsters in Areas have (0.6 - 0.7)% chance to drop an additional Gem with Quality
WatchstoneMagicMonsterGemChance3;T1 Magic Monster Quality Gem;00FF00FF# Magic Monsters in Areas have (0.8 - 1)% chance to drop an additional Gem with Quality
WatchstoneRareMonsterCurrencyChance1;T3 Rare Monster Basic Currency;00FF00FF# Rare Monsters in Areas have (4 - 5)% chance to drop an additional Basic Currency Item
WatchstoneRareMonsterCurrencyChance2_;T2 Rare Monster Basic Currency;00FF00FF# Rare Monsters in Areas have (6 - 7)% chance to drop an additional Basic Currency Item
WatchstoneRareMonsterCurrencyChance3__;T1 Rare Monster Basic Currency;00FF00FF# Rare Monsters in Areas have (8 - 10)% chance to drop an additional Basic Currency Item
WatchstoneRareMonsterGemChance1;T3 Rare Monster Quality Gem;00FF00FF# Rare Monsters in Areas have (4 - 5)% chance to drop an additional Gem with Quality
WatchstoneRareMonsterGemChance2;T2 Rare Monster Quality Gem;00FF00FF# Rare Monsters in Areas have (6 - 7)% chance to drop an additional Gem with Quality
WatchstoneRareMonsterGemChance3;T1 Rare Monster Quality Gem;00FF00FF# Rare Monsters in Areas have (8 - 10)% chance to drop an additional Gem with Quality
WatchstoneKaruiLegionChance1;T4 Karui Legion Chance;00FF00FF# Legion Encounters in Areas have (15 - 30)% increased chance to include a Karui army
WatchstoneKaruiLegionChance2;T3 Karui Legion Chance;00FF00FF# Legion Encounters in Areas have (31 - 45)% increased chance to include a Karui army
WatchstoneKaruiLegionChance3__;T2 Karui Legion Chance;00FF00FF# Legion Encounters in Areas have (46 - 65)% increased chance to include a Karui army
WatchstoneKaruiLegionChance4__;T1 Karui Legion Chance;00FF00FF# Legion Encounters in Areas have (66 - 100)% increased chance to include a Karui army
WatchstoneEmpireLegionChance1;T4 Eternal Legion Chance;00FF00FF# Legion Encounters in Areas have (15 - 30)% increased chance to include an Eternal Empire army
WatchstoneEmpireLegionChance2;T3 Eternal Legion Chance;00FF00FF# Legion Encounters in Areas have (31 - 45)% increased chance to include an Eternal Empire army
WatchstoneEmpireLegionChance3;T2 Eternal Legion Chance;00FF00FF# Legion Encounters in Areas have (46 - 65)% increased chance to include an Eternal Empire army
WatchstoneEmpireLegionChance4_;T1 Eternal Legion Chance;00FF00FF# Legion Encounters in Areas have (66 - 100)% increased chance to include an Eternal Empire army
WatchstoneVaalLegionChance1;T4 Vaal Legion Chance;00FF00FF# Legion Encounters in Areas have (15 - 30)% increased chance to include a Vaal army
WatchstoneVaalLegionChance2;T3 Vaal Legion Chance;00FF00FF# Legion Encounters in Areas have (31 - 45)% increased chance to include a Vaal army
WatchstoneVaalLegionChance3_;T2 Vaal Legion Chance;00FF00FF# Legion Encounters in Areas have (46 - 65)% increased chance to include a Vaal army
WatchstoneVaalLegionChance4_;T1 Vaal Legion Chance;00FF00FF# Legion Encounters in Areas have (66 - 100)% increased chance to include a Vaal army
WatchstoneTemplarLegionChance1;T4 Templar Legion Chance;00FF00FF# Legion Encounters in Areas have (15 - 30)% increased chance to include a Templar army
WatchstoneTemplarLegionChance2_;T3 Templar Legion Chance;00FF00FF# Legion Encounters in Areas have (31 - 45)% increased chance to include a Templar army
WatchstoneTemplarLegionChance3;T2 Templar Legion Chance;00FF00FF# Legion Encounters in Areas have (46 - 65)% increased chance to include a Templar army
WatchstoneTemplarLegionChance4;T1 Templar Legion Chance;00FF00FF# Legion Encounters in Areas have (66 - 100)% increased chance to include a Templar army
WatchstoneMarakethLegionChance1;T4 Maraketh Legion Chance;00FF00FF# Legion Encounters in Areas have (15 - 30)% increased chance to include a Maraketh army
WatchstoneMarakethLegionChance2___;T3 Maraketh Legion Chance;00FF00FF# Legion Encounters in Areas have (31 - 45)% increased chance to include a Maraketh army
WatchstoneMarakethLegionChance3;T2 Maraketh Legion Chance;00FF00FF# Legion Encounters in Areas have (46 - 65)% increased chance to include a Maraketh army
WatchstoneMarakethLegionChance4;T1 Maraketh Legion Chance;00FF00FF# Legion Encounters in Areas have (66 - 100)% increased chance to include a Maraketh army
WatchstoneChaosBreachChance1_;T4 Chayula Breach Chance;00FF00FF# Breaches in Areas have (15 - 30)% increased chance to belong to Chayula
WatchstoneChaosBreachChance2;T3 Chayula Breach Chance;00FF00FF# Breaches in Areas have (31 - 45)% increased chance to belong to Chayula
WatchstoneChaosBreachChance3___;T2 Chayula Breach Chance;00FF00FF# Breaches in Areas have (46 - 65)% increased chance to belong to Chayula
WatchstoneChaosBreachChance4;T1 Chayula Breach Chance;00FF00FF# Breaches in Areas have (66 - 100)% increased chance to belong to Chayula
WatchstoneHigherTierOilChance1__;T3 Oil Tier Boost;00FF00FF# Oils found in Areas have (5 - 8)% chance to be 1 tier higher
WatchstoneHigherTierOilChance2__;T2 Oil Tier Boost;00FF00FF# Oils found in Areas have (9 - 12)% chance to be 1 tier higher
WatchstoneHigherTierOilChance3_;T1 Oil Tier Boost;00FF00FF# Oils found in Areas have (13 - 16)% chance to be 1 tier higher
WatchstoneBreachstoneChance1____;T4 Splinter to Breachstone Chance;00FF00FF# Breach Splinters have (0.02 - 0.03)% chance to drop as Breachstones instead in Areas
WatchstoneBreachstoneChance2;T3 Splinter to Breachstone Chance;00FF00FF# Breach Splinters have (0.04 - 0.05)% chance to drop as Breachstones instead in Areas
WatchstoneBreachstoneChance3;T2 Splinter to Breachstone Chance;00FF00FF# Breach Splinters have (0.06 - 0.07)% chance to drop as Breachstones instead in Areas
WatchstoneBreachstoneChance4_;T1 Splinter to Breachstone Chance;00FF00FF# Breach Splinters have (0.08 - 0.1)% chance to drop as Breachstones instead in Areas
WatchstoneHarbingerLuckyDrops1__;T1 Harbinger Rarer Shard Drops;00FF00FF# Harbingers in Areas drop rarer value Currency Shards
WatchstoneAbyssalDepthsChance1;T2 Increased Abyssal Depths Chance;00FF00FF# Abysses in Areas have (10 - 20)% increased chance to lead to an Abyssal Depths
WatchstoneAbyssalDepthsChance2;T1 Increased Abyssal Depths Chance;00FF00FF# Abysses in Areas have (21 - 30)% increased chance to lead to an Abyssal Depths
WatchstoneWildsBeastChance1;T2 Increased Chance of Beasts from The Wilds;00FF00FF# Red Beasts in Areas have (25 - 35)% increased chance to be from The Wilds
WatchstoneWildsBeastChance2;T1 Increased Chance of Beasts from The Wilds;00FF00FF# Red Beasts in Areas have (36 - 50)% increased chance to be from The Wilds
WatchstoneCavernsBeastChance1;T2 Increased Chance of Beasts from The Caverns;00FF00FF# Red Beasts in Areas have (25 - 35)% increased chance to be from The Caverns
WatchstoneCavernsBeastChance2;T1 Increased Chance of Beasts from The Caverns;00FF00FF# Red Beasts in Areas have (36 - 50)% increased chance to be from The Caverns
WatchstoneMoreSulphite1;T3 Chance of Double Sulphite;00FF00FF# Voltaxic Sulphite Veins and Chests in Areas have (2 - 3)% chance to contain double Sulphite
WatchstoneMoreSulphite2_;T2 Chance of Double Sulphite;00FF00FF# Voltaxic Sulphite Veins and Chests in Areas have (4 - 5)% chance to contain double Sulphite
WatchstoneMoreSulphite3;T1 Chance of Double Sulphite;00FF00FF# Voltaxic Sulphite Veins and Chests in Areas have 6% chance to contain double Sulphite
WatchstoneBlightChestDuplicateRewards1__;T3 Blight Chests Duplicate Chance;00FF00FF# Blight Chests in Areas have (1 - 2)% chance to Duplicate contained Items
WatchstoneBlightChestDuplicateRewards2__;T2 Blight Chests Duplicate Chance;00FF00FF# Blight Chests in Areas have (2.1 - 3.5)% chance to Duplicate contained Items
WatchstoneBlightChestDuplicateRewards3;T1 Blight Chests Duplicate Chance;00FF00FF# Blight Chests in Areas have (3.6 - 5)% chance to Duplicate contained Items
WatchstoneDeliriumAdditionalRewardChance1;T3 Deliirum Chance of Additional Reward Type;00FF00FF# Delirium Encounters in Areas have (2 - 3)% chance to generate an additional Reward type
WatchstoneDeliriumAdditionalRewardChance2;T2 Deliirum Chance of Additional Reward Type;00FF00FF# Delirium Encounters in Areas have (4 - 5)% chance to generate an additional Reward type
WatchstoneDeliriumAdditionalRewardChance3;T1 Deliirum Chance of Additional Reward Type;00FF00FF# Delirium Encounters in Areas have (6 - 7)% chance to generate an additional Reward type
WatchstoneDeliriumSplinterQuantity1;T4 Increased Simulacrum Splinters Drops;00FF00FF# (1 - 2)% increased Stack size of Simulacrum Splinters found in Areas
WatchstoneDeliriumSplinterQuantity2_;T3 Increased Simulacrum Splinters Drops;00FF00FF# (3 - 4)% increased Stack size of Simulacrum Splinters found in Areas
WatchstoneDeliriumSplinterQuantity3;T2 Increased Simulacrum Splinters Drops;00FF00FF# (5 - 6)% increased Stack size of Simulacrum Splinters found in Areas
WatchstoneDeliriumSplinterQuantity4;T1 Increased Simulacrum Splinters Drops;00FF00FF# (7 - 8)% increased Stack size of Simulacrum Splinters found in Areas
WatchstoneDeliriumMonsterClusterJewelDropChance1;T2 Increased Cluster Jewel Drops;00FF00FF# Delirium Monsters in Areas have (5 - 10)% increased chance to drop Cluster Jewels
WatchstoneDeliriumMonsterClusterJewelDropChance2;T1 Increased Cluster Jewel Drops;00FF00FF# Delirium Monsters in Areas have (11 - 15)% increased chance to drop Cluster Jewels
WatchstoneHarvestAdditionalCraftChance1;T3 Additional Harvest Craft Chance;00FF00FF# Plants Harvested in Areas have (1.1 - 1.5)% chance to give an additional Crafting option
WatchstoneHarvestAdditionalCraftChance2;T2 Additional Harvest Craft Chance;00FF00FF# Plants Harvested in Areas have (1.6 - 2.5)% chance to give an additional Crafting option
WatchstoneHarvestAdditionalCraftChance3;T1 Additional Harvest Craft Chance;00FF00FF# Plants Harvested in Areas have (2.6 - 4)% chance to give an additional Crafting option
WatchstoneHarvestHigherTierSeedRolls1;T1 Harvest Tier Boost Chance;00FF00FF# Harvest Crops in Areas have an extra chance to grow higher Tier Plants
WatchstoneHarvestBossChance1;T2 Harvest Tier 4 Plant Chance;00FF00FF# Harvest Crops in Areas have (5 - 10)% increased chance to contain a Tier 4 Plant
WatchstoneHarvestBossChance2_;T1 Harvest Tier 4 Plant Chance;00FF00FF# Harvest Crops in Areas have (11 - 15)% increased chance to contain a Tier 4 Plant
WatchstoneInfluenceDoubleTickChance1__;T2 Double Conqueror Citadel Progression Chance;00FF00FF# (3 - 4)% chance on completing a Map influenced by a Conqueror of the Atlas<br/>to gain double progress towards locating their Citadel
WatchstoneInfluenceDoubleTickChance2;T1 Double Conqueror Citadel Progression Chance;00FF00FF# (5 - 6)% chance on completing a Map influenced by a Conqueror of the Atlas<br/>to gain double progress towards locating their Citadel
WatchstoneVaalSideAreaChance1;T3 Corrupted Zone Chance;00FF00FF# Areas have +(4 - 5)% chance to contain a Vaal Side Area
WatchstoneVaalSideAreaChance2;T2 Corrupted Drops Chance;00FF00FF# Found Items have 1% chance to drop Corrupted in Areas
WatchstoneVaalSideAreaChance3;T1 Corrupted Drops Chance;00FF00FF# Found Items have 2% chance to drop Corrupted in Areas
WatchstoneTrialChance1;T2 Trial Spawn Chance;00FF00FF# Areas have +(6 - 7)% chance to contain a Trial of Ascendancy
WatchstoneTrialChance2_;T1 Trial Spawn Chance;00FF00FF# Areas have +(8 - 10)% chance to contain a Trial of Ascendancy
WatchstoneMapBossDivinationCardChance1;T3 Boss Divination Card Drop Chance;00FF00FF# Unique Bosses have (10 - 15)% chance to drop an additional Divination Card
WatchstoneMapBossDivinationCardChance2_;T2 Boss Divination Card Drop Chance;00FF00FF# Unique Bosses have (16 - 20)% chance to drop an additional Divination Card
WatchstoneMapBossDivinationCardChance3;T1 Boss Divination Card Drop Chance;00FF00FF# Unique Bosses have (21 - 25)% chance to drop an additional Divination Card
WatchstoneMapBossInfluencedItemDropChance1___;T3 Boss Influenced Item Drop Chance;00FF00FF# Unique Bosses have (10 - 15)% chance to drop an additional Item with random Influence
WatchstoneMapBossInfluencedItemDropChance2;T2 Boss Influenced Item Drop Chance;00FF00FF# Unique Bosses have (16 - 20)% chance to drop an additional Item with random Influence
WatchstoneMapBossInfluencedItemDropChance3;T1 Boss Influenced Item Drop Chance;00FF00FF# Unique Bosses have (21 - 25)% chance to drop an additional Item with random Influence
WatchstoneExaltAsStackOfThree1;T1 Triple Exalt Chance;00FF00FF# 1% chance in Areas for Exalted Orbs to drop as 3 Exalted Orbs instead
WatchstoneMasterFavour1_;T3 More Master Favour;00FF00FF# Missions in Areas grant (10 - 15)% increased Favour
WatchstoneMasterFavour2;T2 More Master Favour;00FF00FF# Missions in Areas grant (16 - 25)% increased Favour
WatchstoneMasterFavour3;T1 More Master Favour;00FF00FF# Missions in Areas grant (26 - 40)% increased Favour
WatchstoneMagicMonstersPercent1__;T3 More Magic Monsters;00FF00FF# (1 - 2)% more Magic Monsters
WatchstoneMagicMonstersPercent2_;T2 More Magic Monsters;00FF00FF# (3 - 4)% more Magic Monsters
WatchstoneMagicMonstersPercent3;T1 More Magic Monsters;00FF00FF# 5% more Magic Monsters
WatchstoneRareMonstersPercent1;T3 More Rare Monsters;00FF00FF# (1 - 2)% more Rare Monsters
WatchstoneRareMonstersPercent2;T2 More Rare Monsters;00FF00FF# (3 - 4)% more Rare Monsters
WatchstoneRareMonstersPercent3___;T1 More Rare Monsters;00FF00FF# 5% more Rare Monsters
WatchstoneMagicMonstersFlat1;T1 Additional Magic Pack;00FF00FF# Areas contain an additional Magic Monster pack
WatchstoneRareMonstersFlat1_;T1 Additional Pack and Rare;00FF00FF# Areas contain an additional pack with a Rare monster
WatchstoneExperience1;T1 Experience Gain;00FF00FF# 1% increased Experience gain
WatchstonePackSize1;T3 Pack Size;00FF00FF# 1% increased Pack size
WatchstonePackSize2;T2 Pack Size;00FF00FF# 2% increased Pack size
WatchstonePackSize3_;T1 Pack Size;00FF00FF# 3% increased Pack size
WatchstoneMapModEffect1;T3 Increased Modifier Effect;00FF00FF# 2% increased effect of Modifiers on non-unique Maps
WatchstoneMapModEffect2__;T2 Increased Modifier Effect;00FF00FF# 3% increased effect of Modifiers on non-unique Maps
WatchstoneMapModEffect3_;T1 Increased Modifier Effect;00FF00FF# 4% increased effect of Modifiers on non-unique Maps
WatchstoneRogueExileChance1__;T3 Additional Rogue Exile Chance;00FF00FF# Areas have a (11 - 15)% chance to contain an additional Rogue Exile
WatchstoneRogueExileChance2__;T2 Additional Rogue Exile Chance;00FF00FF# Areas have a (16 - 20)% chance to contain an additional Rogue Exile
WatchstoneRogueExileChance3;T1 Additional Rogue Exile Chance;00FF00FF# Areas have a (21 - 25)% chance to contain an additional Rogue Exile
WatchstoneShrineChance1;T3 Additional Shrine Chance;00FF00FF# Areas have a (11 - 15)% chance to contain an additional Shrine
WatchstoneShrineChance2;T2 Additional Shrine Chance;00FF00FF# Areas have a (16 - 20)% chance to contain an additional Shrine
WatchstoneShrineChance3;T1 Additional Shrine Chance;00FF00FF# Areas have a (21 - 25)% chance to contain an additional Shrine
WatchstoneStrongboxUniqueChance1_;T3 Increased Unique Strongbox Chance;00FF00FF# (35 - 50)% increased chance for Strongboxes in Areas to be Unique
WatchstoneStrongboxUniqueChance2;T2 Increased Unique Strongbox Chance;00FF00FF# (60 - 75)% increased chance for Strongboxes in Areas to be Unique
WatchstoneStrongboxUniqueChance3;T1 Increased Unique Strongbox Chance;00FF00FF# (80 - 100)% increased chance for Strongboxes in Areas to be Unique
WatchstoneHarbingerShardChance1;T2 Harbingers Additional Shards Stack Chance;00FF00FF# Harbingers in Areas have (5 - 10)% chance to drop an additional Stack of Currency Shards
WatchstoneHarbingerShardChance2;T1 Harbingers Additional Shards Stack Chance;00FF00FF# Harbingers in Areas have (11 - 15)% chance to drop an additional Stack of Currency Shards
WatchstoneHarbingerChance1;T3 Additional Harbinger Chance;00FF00FF# Areas have a (5 - 7)% chance to contain an additional Harbinger
WatchstoneHarbingerChance2;T2 Additional Harbinger Chance;00FF00FF# Areas have a (8 - 10)% chance to contain an additional Harbinger
WatchstoneHarbingerChance3;T1 Additional Harbinger Chance;00FF00FF# Areas have a (11 - 15)% chance to contain an additional Harbinger
WatchstoneBeyondPortalChance1_;T1 Beyond;00FF00FF# Slaying Enemies close together in Areas has a 1% chance to attract monsters from Beyond
WatchstoneBeyondPortalAdditionalDemonChance1;T2 Beyond Additional Demon Spawn;00FF00FF# Beyond Portals in Areas have (3 - 4)% chance to spawn an additional Beyond Demon
WatchstoneBeyondPortalAdditionalDemonChance2;T1 Beyond Additional Demon Spawn;00FF00FF# Beyond Portals in Areas have (5 - 6)% chance to spawn an additional Beyond Demon
WatchstoneBeyondExperience1_;T2 Beyond Increased Experience;00FF00FF# Beyond Demons in Areas grant (3 - 4)% increased Experience
WatchstoneBeyondExperience2;T1 Beyond Increased Experience;00FF00FF# Beyond Demons in Areas grant (5 - 6)% increased Experience
WatchstonePerandusCadiroChance1;T2 Contains Cadiro;00FF00FF# Areas have (2 - 3)% chance to contain Cadiro Perandus
WatchstonePerandusCadiroChance2_;T1 Contains Cadiro;00FF00FF# Areas have (4 - 5)% chance to contain Cadiro Perandus
WatchstoneBreachExperience1;T2 Breach Increased Experience;00FF00FF# Breach Monsters in Areas grant (3 - 4)% increased Experience
WatchstoneBreachExperience2;T1 Breach Increased Experience;00FF00FF# Breach Monsters in Areas grant (5 - 6)% increased Experience
WatchstoneAbyssExperience1_;T2 Abyss Increased Experience;00FF00FF# Abyss Monsters in Areas grant (3 - 4)% increased Experience
WatchstoneAbyssExperience2_;T1 Abyss Increased Experience;00FF00FF# Abyss Monsters in Areas grant (5 - 6)% increased Experience
WatchstoneAbyssTroveAbyssalSocketChance1;T1 Abyssal Troves Abyssal Socket Rare Chance;00FF00FF# Abyssal Troves in Areas have 2% chance to drop a Rare Item with an Abyssal Socket
WatchstoneNikoChance1__;T4 Niko Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (15 - 30)% increased chance to be Niko<br/>Master Missions from completing Maps have {0}% increased chance to be Niko
WatchstoneNikoChance2_;T3 Niko Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (31 - 45)% increased chance to be Niko<br/>Master Missions from completing Maps have {0}% increased chance to be Niko
WatchstoneNikoChance3_;T2 Niko Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (46 - 65)% increased chance to be Niko<br/>Master Missions from completing Maps have {0}% increased chance to be Niko
WatchstoneNikoChance4_;T1 Niko Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (66 - 100)% increased chance to be Niko<br/>Master Missions from completing Maps have {0}% increased chance to be Niko
WatchstoneZanaChance1_;T4 Zana Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (15 - 30)% increased chance to be Zana<br/>Master Missions from completing Maps have {0}% increased chance to be Zana
WatchstoneZanaChance2;T3 Zana Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (31 - 45)% increased chance to be Zana<br/>Master Missions from completing Maps have {0}% increased chance to be Zana
WatchstoneZanaChance3;T2 Zana Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (46 - 65)% increased chance to be Zana<br/>Master Missions from completing Maps have {0}% increased chance to be Zana
WatchstoneZanaChance4_;T1 Zana Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (66 - 100)% increased chance to be Zana<br/>Master Missions from completing Maps have {0}% increased chance to be Zana
WatchstoneJunChance1_;T4 Jun Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (15 - 30)% increased chance to be Jun<br/>Master Missions from completing Maps have {0}% increased chance to be Jun
WatchstoneJunChance2_;T3 Jun Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (31 - 45)% increased chance to be Jun<br/>Master Missions from completing Maps have {0}% increased chance to be Jun
WatchstoneJunChance3;T2 Jun Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (46 - 65)% increased chance to be Jun<br/>Master Missions from completing Maps have {0}% increased chance to be Jun
WatchstoneJunChance4__;T1 Jun Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (66 - 100)% increased chance to be Jun<br/>Master Missions from completing Maps have {0}% increased chance to be Jun
WatchstoneAlvaChance1;T4 Alva Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (15 - 30)% increased chance to be Alva<br/>Master Missions from completing Maps have {0}% increased chance to be Alva
WatchstoneAlvaChance2;T3 Alva Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (31 - 45)% increased chance to be Alva<br/>Master Missions from completing Maps have {0}% increased chance to be Alva
WatchstoneAlvaChance3;T2 Alva Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (46 - 65)% increased chance to be Alva<br/>Master Missions from completing Maps have {0}% increased chance to be Alva
WatchstoneAlvaChance4__;T1 Alva Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (66 - 100)% increased chance to be Alva<br/>Master Missions from completing Maps have {0}% increased chance to be Alva
WatchstoneEinharChance1;T4 Einhar Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (15 - 30)% increased chance to be Einhar<br/>Master Missions from completing Maps have {0}% increased chance to be Einhar
WatchstoneEinharChance2;T3 Einhar Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (31 - 45)% increased chance to be Einhar<br/>Master Missions from completing Maps have {0}% increased chance to be Einhar
WatchstoneEinharChance3;T2 Einhar Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (46 - 65)% increased chance to be Einhar<br/>Master Missions from completing Maps have {0}% increased chance to be Einhar
WatchstoneEinharChance4;T1 Einhar Mission Chance;00FF00FF# Randomly encountered Masters in Areas have (66 - 100)% increased chance to be Einhar<br/>Master Missions from completing Maps have {0}% increased chance to be Einhar
WatchstoneMasterChance1;T3 Additional Mission on Completion;00FF00FF# Areas have (1 - 2)% chance to grant an additional Mission on Completion
WatchstoneMasterChance2_;T2 Additional Mission on Completion;00FF00FF# Areas have (3 - 4)% chance to grant an additional Mission on Completion
WatchstoneMasterChance3_;T1 Additional Mission on Completion;00FF00FF# Areas have (2 - 3)% chance to grant an additional Mission on Completion
WatchstoneRedBeastChance1;T3 Additional Red Beast Chance;00FF00FF# Areas that contain capturable Beasts have (6 - 8)% chance to contain an additional Red Beast
WatchstoneRedBeastChance2_;T2 Additional Red Beast Chance;00FF00FF# Areas that contain capturable Beasts have (9 - 12)% chance to contain an additional Red Beast
WatchstoneRedBeastChance3;T1 Additional Red Beast Chance;00FF00FF# Areas that contain capturable Beasts have (13 - 16)% chance to contain an additional Red Beast
WatchstoneTwiceCaptureChance1_;T2 Duplicate Beast Chance;00FF00FF# (4 - 6)% chance create a copy of Beasts Captured in Areas
WatchstoneTwiceCaptureChance2;T1 Duplicate Beast Chance;00FF00FF# (7 - 10)% chance create a copy of Beasts Captured in Areas
WatchstoneLuckyBeastChance1;T2 Less Common Beast Chance;00FF00FF# (25 - 35)% chance for Red Beasts in Areas to be the less common of two varieties
WatchstoneLuckyBeastChance2;T1 Less Common Beast Chance;00FF00FF# (36 - 50)% chance for Red Beasts in Areas to be the less common of two varieties
WatchstoneIncursionItemQuantity1;T3 Increased Incursion Item Quantity;00FF00FF# (5 - 6)% increased Quantity of Items dropped in Incursions in Areas
WatchstoneIncursionItemQuantity2;T2 Increased Incursion Item Quantity;00FF00FF# (7 - 8)% increased Quantity of Items dropped in Incursions in Areas
WatchstoneIncursionItemQuantity3_;T1 Increased Incursion Item Quantity;00FF00FF# (9 - 10)% increased Quantity of Items dropped in Incursions in Areas
WatchstoneBetrayalExtraTarget1;T3 Syndicate Reinforcements More Likely;00FF00FF# Immortal Syndicate Members in Areas are (3 - 4)% more<br/>likely to be accompanied by reinforcements
WatchstoneBetrayalExtraTarget2;T2 Syndicate Reinforcements More Likely;00FF00FF# Immortal Syndicate Members in Areas are (5 - 6)% more<br/>likely to be accompanied by reinforcements
WatchstoneBetrayalExtraTarget3__;T1 Syndicate Reinforcements More Likely;00FF00FF# Immortal Syndicate Members in Areas are (7 - 8)% more<br/>likely to be accompanied by reinforcements
WatchstoneBetrayalExtraRankChance1_;T3 Syndicate Double Rank on Execution Chance;00FF00FF# Immortal Syndicate Members Executed in Areas have (6 - 8)% chance to gain an additional Rank
WatchstoneBetrayalExtraRankChance2_;T2 Syndicate Double Rank on Execution Chance;00FF00FF# Immortal Syndicate Members Executed in Areas have (9 - 12)% chance to gain an additional Rank
WatchstoneBetrayalExtraRankChance3_;T1 Syndicate Double Rank on Execution Chance;00FF00FF# Immortal Syndicate Members Executed in Areas have (13 - 16)% chance to gain an additional Rank
WatchstoneBetrayalAdditionalVeiledItemChance1;T3 Syndicate Additional Veiled Item;00FF00FF# Immortal Syndicate Members have (6 - 8)% chance to drop an additional Veiled Item
WatchstoneBetrayalAdditionalVeiledItemChance2;T2 Syndicate Additional Veiled Item;00FF00FF# Immortal Syndicate Members have (9 - 12)% chance to drop an additional Veiled Item
WatchstoneBetrayalAdditionalVeiledItemChance3;T1 Syndicate Additional Veiled Item;00FF00FF# Immortal Syndicate Members have (13 - 16)% chance to drop an additional Veiled Item
WatchstoneSulphiteExtraPackChance1__;T3 Veins and Chests Sulphite Hoarding Monsters Chance;00FF00FF# (6 - 8)% chance for Sulphite Veins and Chests in Areas to be guarded by Sulphite-hoarding Monsters
WatchstoneSulphiteExtraPackChance2_;T2 Veins and Chests Sulphite Hoarding Monsters Chance;00FF00FF# (9 - 12)% chance for Sulphite Veins and Chests in Areas to be guarded by Sulphite-hoarding Monsters
WatchstoneSulphiteExtraPackChance3;T1 Veins and Chests Sulphite Hoarding Monsters Chance;00FF00FF# (13 - 16)% chance for Sulphite Veins and Chests in Areas to be guarded by Sulphite-hoarding Monsters
WatchstoneZanaExtraOptions1_;T2 Additional Zana Mission Option;00FF00FF# Zana Missions in Areas have 1 additional Map option
WatchstoneZanaExtraOptions2;T1 Additional Zana Mission Option;00FF00FF# Zana Missions in Areas have 2 additional Map options
WatchstoneZanaMapQuality1_;T2 Additional Zana Map Mission Quality;00FF00FF# +(3 - 4)% to Quality of Maps offered by Zana Missions in Areas
WatchstoneZanaMapQuality2___;T1 Additional Zana Map Mission Quality;00FF00FF# +(5 - 6)% to Quality of Maps offered by Zana Missions in Areas
WatchstoneZanaUniqueMapChance1;T3 Zana Missions Increased Unique Maps;00FF00FF# Map options offered by Zana Missions in Areas have (20 - 25)% increased chance to be Unique
WatchstoneZanaUniqueMapChance2;T2 Zana Missions Increased Unique Maps;00FF00FF# Map options offered by Zana Missions in Areas have (26 - 35)% increased chance to be Unique
WatchstoneZanaUniqueMapChance3;T1 Zana Missions Increased Unique Maps;00FF00FF# Map options offered by Zana Missions in Areas have (36 - 50)% increased chance to be Unique
WatchstoneZanaSynthesisMapChance1;T3 Zana Missions Increased Synthesis Maps;00FF00FF# Map options offered by Zana Missions in Areas have (20 - 25)% increased chance to be Synthesis Maps
WatchstoneZanaSynthesisMapChance2_;T2 Zana Missions Increased Synthesis Maps;00FF00FF# Map options offered by Zana Missions in Areas have (26 - 35)% increased chance to be Synthesis Maps
WatchstoneZanaSynthesisMapChance3_;T1 Zana Missions Increased Synthesis Maps;00FF00FF# Map options offered by Zana Missions in Areas have (36 - 50)% increased chance to be Synthesis Maps
WatchstoneZanaCorruptedMapChance1_;T3 Zana Missions Increased 8 Mod Chance;00FF00FF# Map options offered by Zana Missions in Areas have (20 - 25)% increased chance to have 8 Modifiers
WatchstoneZanaCorruptedMapChance2;T2 Zana Missions Increased 8 Mod Chance;00FF00FF# Map options offered by Zana Missions in Areas have (26 - 35)% increased chance to have 8 Modifiers
WatchstoneZanaCorruptedMapChance3_;T1 Zana Missions Increased 8 Mod Chance;00FF00FF# Map options offered by Zana Missions in Areas have (36 - 50)% increased chance to have 8 Modifiers
WatchstoneMapBossScarabChance1_;T3 Unique Bosses Additional Scarab;00FF00FF# Unique Bosses have (3 - 4.5)% chance to drop an additional Scarab
WatchstoneMapBossScarabChance2;T2 Unique Bosses Additional Scarab;00FF00FF# Unique Bosses have (5 - 6.5)% chance to drop an additional Scarab
WatchstoneMapBossScarabChance3;T1 Unique Bosses Additional Scarab;00FF00FF# Unique Bosses have (7 - 9)% chance to drop an additional Scarab
WatchstoneMapBossUniqueChance1_;T3 Unique Bosses Additional Unique;00FF00FF# Unique Map Bosses have (5 - 7)% chance to drop an additional Unique Item
WatchstoneMapBossUniqueChance2;T2 Unique Bosses Additional Unique;00FF00FF# Unique Map Bosses have (8 - 10)% chance to drop an additional Unique Item
WatchstoneMapBossUniqueChance3;T1 Unique Bosses Additional Unique;00FF00FF# Unique Map Bosses have (11 - 15)% chance to drop an additional Unique Item
WatchstoneMapBossMapCurrencyChance1;T3 Unique Bosses Additional Map Currency;00FF00FF# Unique Map Bosses have (5 - 7)% chance to drop additional Map Currency Items
WatchstoneMapBossMapCurrencyChance2;T2 Unique Bosses Additional Map Currency;00FF00FF# Unique Map Bosses have (8 - 10)% chance to drop additional Map Currency Items
WatchstoneMapBossMapCurrencyChance3;T1 Unique Bosses Additional Map Currency;00FF00FF# Unique Map Bosses have (11 - 15)% chance to drop additional Map Currency Items
WatchstoneMapBossMapChance1;T3 Unique Bosses Additional Map;00FF00FF# Unique Bosses have (3 - 4)% chance to drop an additional Map Item
WatchstoneMapBossMapChance2_;T2 Unique Bosses Additional Map;00FF00FF# Unique Bosses have (5 - 6)% chance to drop an additional Map Item
WatchstoneMapBossMapChance3;T1 Unique Bosses Additional Map;00FF00FF# Unique Bosses have (7 - 10)% chance to drop an additional Map Item
WatchstoneMapBossFragmentChance1;T3 Unique Bosses Additional Sacrifice Fragment;00FF00FF# Unique Bosses have (10 - 15)% chance to drop an additional Sacrifice Fragment
WatchstoneMapBossFragmentChance2;T2 Unique Bosses Additional Sacrifice Fragment;00FF00FF# Unique Bosses have (16 - 20)% chance to drop an additional Sacrifice Fragment
WatchstoneMapBossFragmentChance3;T1 Unique Bosses Additional Sacrifice Fragment;00FF00FF# Unique Bosses have (21 - 25)% chance to drop an additional Sacrifice Fragment
WatchstoneAtlasBaseChance1_;T3 Unique Bosses Additional Atlas Base Type;00FF00FF# Unique Map Bosses have (2 - 3)% chance to drop an additional Atlas Base Type
WatchstoneAtlasBaseChance2_;T2 Unique Bosses Additional Atlas Base Type;00FF00FF# Unique Map Bosses have (4 - 5)% chance to drop an additional Atlas Base Type
WatchstoneAtlasBaseChance3_;T1 Unique Bosses Additional Atlas Base Type;00FF00FF# Unique Map Bosses have (5 - 6)% chance to drop an additional Atlas Base Type
# WatchstoneAdditionalEssenceChance1;T3 map_essence_monsters_have_an_additional_essence_%_chance;00FF00FF# Imprisoned Monsters in Areas have (4 - 6)% chance to have an additional Essence
# WatchstoneAdditionalEssenceChance2;T2 map_essence_monsters_have_an_additional_essence_%_chance;00FF00FF# Imprisoned Monsters in Areas have (7 - 10)% chance to have an additional Essence
# WatchstoneAdditionalEssenceChance3;T1 map_essence_monsters_have_an_additional_essence_%_chance;00FF00FF# Imprisoned Monsters in Areas have (11 - 15)% chance to have an additional Essence
# WatchstoneWeaponQualityChance1_;T3 map_weapons_drop_20%_quality_%_chance;00FF00FF# Weapons found in Areas have (2 - 3)% chance to drop with 20% Quality
# WatchstoneWeaponQualityChance2;T2 map_weapons_drop_20%_quality_%_chance;00FF00FF# Weapons found in Areas have (4 - 5)% chance to drop with 20% Quality
# WatchstoneWeaponQualityChance3_;T1 map_weapons_drop_20%_quality_%_chance;00FF00FF# Weapons found in Areas have (6 - 7)% chance to drop with 20% Quality
# WatchstoneArmourQualityChance1;T3 map_armours_drop_20%_quality_%_chance;00FF00FF# Armour Items found in Areas have (2 - 3)% chance to drop with 20% Quality
# WatchstoneArmourQualityChance2;T2 map_armours_drop_20%_quality_%_chance;00FF00FF# Armour Items found in Areas have (4 - 5)% chance to drop with 20% Quality
# WatchstoneArmourQualityChance3;T1 map_armours_drop_20%_quality_%_chance;00FF00FF# Armour Items found in Areas have (6 - 7)% chance to drop with 20% Quality
# WatchstoneGemQualityChance1_;T1 map_gems_drop_20%_quality_%_chance;00FF00FF# Gems found in Areas have 1% chance to drop with 20% Quality
# WatchstoneFlaskQualityChance1;T3 map_flasks_drop_with_max_quality_chance_%;00FF00FF# Flasks found in Areas have (2 - 3)% chance to drop with 20% Quality
# WatchstoneFlaskQualityChance2__;T2 map_flasks_drop_with_max_quality_chance_%;00FF00FF# Flasks found in Areas have (4 - 5)% chance to drop with 20% Quality
# WatchstoneFlaskQualityChance3;T1 map_flasks_drop_with_max_quality_chance_%;00FF00FF# Flasks found in Areas have (6 - 7)% chance to drop with 20% Quality
# WatchstoneChestItemQuantity1;T3 map_chest_item_quantity_+%;00FF00FF# Chests have (10 - 15)% increased Item Quantity
# WatchstoneChestItemQuantity2;T2 map_chest_item_quantity_+%;00FF00FF# Chests have (16 - 25)% increased Item Quantity
# WatchstoneChestItemQuantity3;T1 map_chest_item_quantity_+%;00FF00FF# Chests have (26 - 35)% increased Item Quantity
# WatchstoneInfluenceMonsterWeaponChance1_;T3 influence_monster_chance_to_drop_rare_weapon_%;00FF00FF# Influenced Monsters in Areas have (3 - 4)% chance to drop an additional Rare Weapon
# WatchstoneInfluenceMonsterWeaponChance2_;T2 influence_monster_chance_to_drop_rare_weapon_%;00FF00FF# Influenced Monsters in Areas have (5 - 6)% chance to drop an additional Rare Weapon
# WatchstoneInfluenceMonsterWeaponChance3__;T1 influence_monster_chance_to_drop_rare_weapon_%;00FF00FF# Influenced Monsters in Areas have (7 - 8)% chance to drop an additional Rare Weapon
# WatchstoneInfluenceMonsterArmourChance1__;T3 influence_monster_chance_to_drop_rare_armour_%;00FF00FF# Influenced Monsters in Areas have (3 - 4)% chance to drop an additional Rare Armour Item
# WatchstoneInfluenceMonsterArmourChance2;T2 influence_monster_chance_to_drop_rare_armour_%;00FF00FF# Influenced Monsters in Areas have (5 - 6)% chance to drop an additional Rare Armour Item
# WatchstoneInfluenceMonsterArmourChance3;T1 influence_monster_chance_to_drop_rare_armour_%;00FF00FF# Influenced Monsters in Areas have (7 - 8)% chance to drop an additional Rare Armour Item
# WatchstoneInfluenceMonsterJewelleryChance1_;T3 influence_monster_chance_to_drop_rare_jewellery_%;00FF00FF# Influenced Monsters in Areas have (3 - 4)% chance to drop an additional Rare Jewellery Item
# WatchstoneInfluenceMonsterJewelleryChance2__;T2 influence_monster_chance_to_drop_rare_jewellery_%;00FF00FF# Influenced Monsters in Areas have (5 - 6)% chance to drop an additional Rare Jewellery Item
# WatchstoneInfluenceMonsterJewelleryChance3;T1 influence_monster_chance_to_drop_rare_jewellery_%;00FF00FF# Influenced Monsters in Areas have (7 - 8)% chance to drop an additional Rare Jewellery Item
# WatchstoneMagicMonsterWeaponChance1_;T3 magic_monster_chance_to_drop_rare_weapon_permillage;00FF00FF# Magic Monsters in Areas have (0.5 - 1)% chance to drop an additional Rare Weapon
# WatchstoneMagicMonsterWeaponChance2;T2 magic_monster_chance_to_drop_rare_weapon_permillage;00FF00FF# Magic Monsters in Areas have (1.1 - 1.5)% chance to drop an additional Rare Weapon
# WatchstoneMagicMonsterWeaponChance3;T1 magic_monster_chance_to_drop_rare_weapon_permillage;00FF00FF# Magic Monsters in Areas have (1.6 - 2)% chance to drop an additional Rare Weapon
# WatchstoneMagicMonsterArmourChance1_;T3 magic_monster_chance_to_drop_rare_armour_permillage;00FF00FF# Magic Monsters in Areas have (0.5 - 1)% chance to drop an additional Rare Armour Item
# WatchstoneMagicMonsterArmourChance2__;T2 magic_monster_chance_to_drop_rare_armour_permillage;00FF00FF# Magic Monsters in Areas have (1.1 - 1.5)% chance to drop an additional Rare Armour Item
# WatchstoneMagicMonsterArmourChance3;T1 magic_monster_chance_to_drop_rare_armour_permillage;00FF00FF# Magic Monsters in Areas have (1.6 - 2)% chance to drop an additional Rare Armour Item
# WatchstoneMagicMonsterJewelleryChance1;T3 magic_monster_chance_to_drop_rare_jewellery_permillage;00FF00FF# Magic Monsters in Areas have (0.5 - 1)% chance to drop an additional Rare Jewellery Item
# WatchstoneMagicMonsterJewelleryChance2;T2 magic_monster_chance_to_drop_rare_jewellery_permillage;00FF00FF# Magic Monsters in Areas have (1.1 - 1.5)% chance to drop an additional Rare Jewellery Item
# WatchstoneMagicMonsterJewelleryChance3__;T1 magic_monster_chance_to_drop_rare_jewellery_permillage;00FF00FF# Magic Monsters in Areas have (1.6 - 2)% chance to drop an additional Rare Jewellery Item
# WatchstoneRareMonsterWeaponChance1_;T3 rare_monster_chance_to_drop_rare_weapon_%;00FF00FF# Rare Monsters in Areas have (5 - 10)% chance to drop an additional Rare Weapon
# WatchstoneRareMonsterWeaponChance2;T2 rare_monster_chance_to_drop_rare_weapon_%;00FF00FF# Rare Monsters in Areas have (11 - 15)% chance to drop an additional Rare Weapon
# WatchstoneRareMonsterWeaponChance3;T1 rare_monster_chance_to_drop_rare_weapon_%;00FF00FF# Rare Monsters in Areas have (16 - 20)% chance to drop an additional Rare Weapon
# WatchstoneRareMonsterArmourChance1;T3 rare_monster_chance_to_drop_rare_armour_%;00FF00FF# Rare Monsters in Areas have (5 - 10)% chance to drop an additional Rare Armour Item
# WatchstoneRareMonsterArmourChance2___;T2 rare_monster_chance_to_drop_rare_armour_%;00FF00FF# Rare Monsters in Areas have (11 - 15)% chance to drop an additional Rare Armour Item
# WatchstoneRareMonsterArmourChance3;T1 rare_monster_chance_to_drop_rare_armour_%;00FF00FF# Rare Monsters in Areas have (16 - 20)% chance to drop an additional Rare Armour Item
# WatchstoneRareMonsterJewelleryChance1_;T3 rare_monster_chance_to_drop_rare_jewellery_%;00FF00FF# Rare Monsters in Areas have (5 - 10)% chance to drop an additional Rare Jewellery Item
# WatchstoneRareMonsterJewelleryChance2______;T2 rare_monster_chance_to_drop_rare_jewellery_%;00FF00FF# Rare Monsters in Areas have (11 - 15)% chance to drop an additional Rare Jewellery Item
# WatchstoneRareMonsterJewelleryChance3;T1 rare_monster_chance_to_drop_rare_jewellery_%;00FF00FF# Rare Monsters in Areas have (16 - 20)% chance to drop an additional Rare Jewellery Item
# WatchstoneFireBreachChance1;T4 map_breach_chance_to_be_xoph_+%;00FF00FF# Breaches in Areas have (15 - 30)% increased chance to belong to Xoph
# WatchstoneFireBreachChance2;T3 map_breach_chance_to_be_xoph_+%;00FF00FF# Breaches in Areas have (31 - 45)% increased chance to belong to Xoph
# WatchstoneFireBreachChance3;T2 map_breach_chance_to_be_xoph_+%;00FF00FF# Breaches in Areas have (46 - 65)% increased chance to belong to Xoph
# WatchstoneFireBreachChance4;T1 map_breach_chance_to_be_xoph_+%;00FF00FF# Breaches in Areas have (66 - 100)% increased chance to belong to Xoph
# WatchstoneColdBreachChance1__;T4 map_breach_chance_to_be_tul_+%;00FF00FF# Breaches in Areas have (15 - 30)% increased chance to belong to Tul
# WatchstoneColdBreachChance2_;T3 map_breach_chance_to_be_tul_+%;00FF00FF# Breaches in Areas have (31 - 45)% increased chance to belong to Tul
# WatchstoneColdBreachChance3;T2 map_breach_chance_to_be_tul_+%;00FF00FF# Breaches in Areas have (46 - 65)% increased chance to belong to Tul
# WatchstoneColdBreachChance4___;T1 map_breach_chance_to_be_tul_+%;00FF00FF# Breaches in Areas have (66 - 100)% increased chance to belong to Tul
# WatchstoneLightningBreachChance1;T4 map_breach_chance_to_be_esh_+%;00FF00FF# Breaches in Areas have (15 - 30)% increased chance to belong to Esh
# WatchstoneLightningBreachChance2_;T3 map_breach_chance_to_be_esh_+%;00FF00FF# Breaches in Areas have (31 - 45)% increased chance to belong to Esh
# WatchstoneLightningBreachChance3;T2 map_breach_chance_to_be_esh_+%;00FF00FF# Breaches in Areas have (46 - 65)% increased chance to belong to Esh
# WatchstoneLightningBreachChance4;T1 map_breach_chance_to_be_esh_+%;00FF00FF# Breaches in Areas have (66 - 100)% increased chance to belong to Esh
# WatchstonePhysicalBreachChance1_;T4 map_breach_chance_to_be_uul_netol_+%;00FF00FF# Breaches in Areas have (15 - 30)% increased chance to belong to Uul-Netol
# WatchstonePhysicalBreachChance2_;T3 map_breach_chance_to_be_uul_netol_+%;00FF00FF# Breaches in Areas have (31 - 45)% increased chance to belong to Uul-Netol
# WatchstonePhysicalBreachChance3;T2 map_breach_chance_to_be_uul_netol_+%;00FF00FF# Breaches in Areas have (46 - 65)% increased chance to belong to Uul-Netol
# WatchstonePhysicalBreachChance4;T1 map_breach_chance_to_be_uul_netol_+%;00FF00FF# Breaches in Areas have (66 - 100)% increased chance to belong to Uul-Netol
# WatchstoneUniqueMapReplacementChance1;T3 map_dropped_map_as_random_unique_map_permilliage;00FF00FF# Map Items have a (0.1 - 0.3)% chance to drop as a random Unique Map instead in Areas
# WatchstoneUniqueMapReplacementChance2;T2 map_dropped_map_as_random_unique_map_permilliage;00FF00FF# Map Items have a (0.4 - 0.6)% chance to drop as a random Unique Map instead in Areas
# WatchstoneUniqueMapReplacementChance3;T1 map_dropped_map_as_random_unique_map_permilliage;00FF00FF# Map Items have a (0.7 - 1)% chance to drop as a random Unique Map instead in Areas
# WatchstoneDeepBeastChance1;T2 map_red_beast_from_deep_chance_+%;00FF00FF# Red Beasts in Areas have (25 - 35)% increased chance to be from The Deep
# WatchstoneDeepBeastChance2;T1 map_red_beast_from_deep_chance_+%;00FF00FF# Red Beasts in Areas have (36 - 50)% increased chance to be from The Deep
# WatchstoneSandsBeastChance1;T2 map_red_beast_from_sands_chance_+%;00FF00FF# Red Beasts in Areas have (25 - 35)% increased chance to be from The Sands
# WatchstoneSandsBeastChance2;T1 map_red_beast_from_sands_chance_+%;00FF00FF# Red Beasts in Areas have (36 - 50)% increased chance to be from The Sands
# WatchstoneMetamorphSampleRewardChance1_;T3 map_metamorph_sample_has_reward_chance_+%;00FF00FF# Metamorph Samples dropped in Areas have (5 - 10)% increased chance to have a Reward
# WatchstoneMetamorphSampleRewardChance2;T2 map_metamorph_sample_has_reward_chance_+%;00FF00FF# Metamorph Samples dropped in Areas have (11 - 15)% increased chance to have a Reward
# WatchstoneMetamorphSampleRewardChance3__;T1 map_metamorph_sample_has_reward_chance_+%;00FF00FF# Metamorph Samples dropped in Areas have (16 - 20)% increased chance to have a Reward
# WatchstoneHeistCacheChance1;T3 map_heist_smugglers_cache_encounter_chance_+%;00FF00FF# Areas have (5 - 10)% increased chance to contain a Smuggler’s Cache
# WatchstoneHeistCacheChance2;T2 map_heist_smugglers_cache_encounter_chance_+%;00FF00FF# Areas have (11 - 15)% increased chance to contain a Smuggler’s Cache
# WatchstoneHeistCacheChance3;T1 map_heist_smugglers_cache_encounter_chance_+%;00FF00FF# Areas have (16 - 20)% increased chance to contain a Smuggler’s Cache
# WatchstoneHeistBlueprintDropChance1;T3 map_heist_blueprint_drop_chance_+%;00FF00FF# (3 - 4)% more Blueprints found in Areas
# WatchstoneHeistBlueprintDropChance2;T2 map_heist_blueprint_drop_chance_+%;00FF00FF# (5 - 6)% more Blueprints found in Areas
# WatchstoneHeistBlueprintDropChance3;T1 map_heist_blueprint_drop_chance_+%;00FF00FF# (7 - 10)% more Blueprints found in Areas
# WatchstoneHeistCoinStackSize1;T2 map_rogue_markers_stack_size_+%;00FF00FF# (5 - 15)% increased Stack size of Rogue's Markers found in Areas
# WatchstoneHeistCoinStackSize2;T1 map_rogue_markers_stack_size_+%;00FF00FF# (16 - 25)% increased Stack size of Rogue's Markers found in Areas
# WatchstonePlusOneDropLevelChance1_;T3 map_+1_item_level_drops_%_chance;00FF00FF# Monsters and Chests in Areas have (2 - 3)% chance to drop Items with +1 to Item Level
# WatchstonePlusOneDropLevelChance2;T2 map_+1_item_level_drops_%_chance;00FF00FF# Monsters and Chests in Areas have (4 - 5)% chance to drop Items with +1 to Item Level
# WatchstonePlusOneDropLevelChance3_;T1 map_+1_item_level_drops_%_chance;00FF00FF# Monsters and Chests in Areas have (6 - 7)% chance to drop Items with +1 to Item Level
# WatchstoneRogueExilePossessionChance1;T2 map_rogue_exiles_possessed_%_chance;00FF00FF# Rogue Exiles in Areas have (10 - 15)% chance to be Possessed by a Tormented Spirit
# WatchstoneRogueExilePossessionChance2__;T1 map_rogue_exiles_possessed_%_chance;00FF00FF# Rogue Exiles in Areas have (16 - 25)% chance to be Possessed by a Tormented Spirit
# WatchstoneStrongboxRareChance1;T3 map_strongboxes_chance_to_be_rare_+%;00FF00FF# Strongboxes in Areas have (35 - 50)% increased chance to be Rare
# WatchstoneStrongboxRareChance2;T2 map_strongboxes_chance_to_be_rare_+%;00FF00FF# Strongboxes in Areas have (60 - 75)% increased chance to be Rare
# WatchstoneStrongboxRareChance3;T1 map_strongboxes_chance_to_be_rare_+%;00FF00FF# Strongboxes in Areas have (80 - 100)% increased chance to be Rare
# WatchstonePerandusChestItemQuantity1;T2 map_perandus_chest_item_quantity_+%;00FF00FF# (21 - 35)% increased Quantity of Items found in Perandus Chests in Areas
# WatchstonePerandusChestItemQuantity2;T1 map_perandus_chest_item_quantity_+%;00FF00FF# (36 - 50)% increased Quantity of Items found in Perandus Chests in Areas
# WatchstonePerandusHoardChance1;T2 map_perandus_hoard_chance_+%;00FF00FF# Perandus Chests in Areas have (21 - 35)% increased chance to be Hoards
# WatchstonePerandusHoardChance2_;T1 map_perandus_hoard_chance_+%;00FF00FF# Perandus Chests in Areas have (36 - 50)% increased chance to be Hoards
# WatchstonePerandusCatalogueChance1;T2 map_perandus_catalogue_chance_+%;00FF00FF# Perandus Chests in Areas have (21 - 35)% increased chance to be Catalogues
# WatchstonePerandusCatalogueChance2;T1 map_perandus_catalogue_chance_+%;00FF00FF# Perandus Chests in Areas have (36 - 50)% increased chance to be Catalogues
# WatchstonePerandusArchiveChance1;T2 map_perandus_archive_chance_+%;00FF00FF# Perandus Chests in Areas have (21 - 35)% increased chance to be Archives
# WatchstonePerandusArchiveChance2_;T1 map_perandus_archive_chance_+%;00FF00FF# Perandus Chests in Areas have (36 - 50)% increased chance to be Archives
# WatchstonePerandusCofferChance1;T2 map_perandus_coffer_chance_+%;00FF00FF# Perandus Chests in Areas have (21 - 35)% increased chance to be Coffers
# WatchstonePerandusCofferChance2;T1 map_perandus_coffer_chance_+%;00FF00FF# Perandus Chests in Areas have (36 - 50)% increased chance to be Coffers
# WatchstonePerandusTreasuryChance1;T2 map_perandus_treasury_chance_+%;00FF00FF# Perandus Chests in Areas have (21 - 35)% increased chance to be Treasuries
# WatchstonePerandusTreasuryChance2__;T1 map_perandus_treasury_chance_+%;00FF00FF# Perandus Chests in Areas have (36 - 50)% increased chance to be Treasuries
# WatchstoneEssenceModdedItemChance1;T1 map_essence_monsters_drop_rare_item_with_random_essence_mod_%_chance;00FF00FF# Imprisoned Monsters in Areas have (35 - 50)% chance to drop an additional Rare Item with an Essence Modifier
# WatchstoneTormentedSpiritDuration1_;T1 map_tormented_spirits_duration_+%;00FF00FF# Tormented Spirits in Areas have (35 - 50)% increased Duration
# WatchstoneItemRarity1;T3 map_item_drop_rarity_+%;00FF00FF# (4 - 7)% increased Rarity of Items found in Areas
# WatchstoneItemRarity2__;T2 map_item_drop_rarity_+%;00FF00FF# (8 - 11)% increased Rarity of Items found in Areas
# WatchstoneItemRarity3__;T1 map_item_drop_rarity_+%;00FF00FF# (12 - 15)% increased Rarity of Items found in Areas
# WatchstoneShrineDuration1_;T2 map_player_shrine_effect_duration_+%;00FF00FF# (25 - 35)% increased Duration of Shrine Effects on Players
# WatchstoneShrineDuration2___;T1 map_player_shrine_effect_duration_+%;00FF00FF# (36 - 50)% increased Duration of Shrine Effects on Players
# WatchstoneTormentedTouchedItemQuantity1;T2 map_possessed_and_touched_monsters_item_quantity_+%;00FF00FF# (3 - 5)% increased Quantity of Items dropped by Possessed or Touched Monsters in Areas
# WatchstoneTormentedTouchedItemQuantity2;T1 map_possessed_and_touched_monsters_item_quantity_+%;00FF00FF# (6 - 8)% increased Quantity of Items dropped by Possessed or Touched Monsters in Areas
# WatchstoneTormentedSpiritChance1;T3 map_additional_tormented_spirit_chance_%;00FF00FF# Areas have a (11 - 15)% chance to contain an additional Tormented Spirit
# WatchstoneTormentedSpiritChance2__;T2 map_additional_tormented_spirit_chance_%;00FF00FF# Areas have a (16 - 20)% chance to contain an additional Tormented Spirit
# WatchstoneTormentedSpiritChance3;T1 map_additional_tormented_spirit_chance_%;00FF00FF# Areas have a (21 - 25)% chance to contain an additional Tormented Spirit
# WatchstoneInvasionBossChance1___;T3 map_additional_invasion_boss_chance_%;00FF00FF# Areas have a (11 - 15)% chance to contain an additional Invasion Boss
# WatchstoneInvasionBossChance2;T2 map_additional_invasion_boss_chance_%;00FF00FF# Areas have a (16 - 20)% chance to contain an additional Invasion Boss
# WatchstoneInvasionBossChance3____;T1 map_additional_invasion_boss_chance_%;00FF00FF# Areas have a (21 - 25)% chance to contain an additional Invasion Boss
# WatchstoneBlightRewardTypeChance1__;T2 map_blight_encounter_blight_reward_type_chance_+%;00FF00FF# Blight Chests in Areas have (10 - 14)% increased chance to contain Blighted Maps or Oils
# WatchstoneBlightRewardTypeChance2_;T1 map_blight_encounter_blight_reward_type_chance_+%;00FF00FF# Blight Chests in Areas have (15 - 20)% increased chance to contain Blighted Maps or Oils
# WatchstoneMetamorphExtraSamples1;T2 map_metamorphosis_additional_sample_rewards;00FF00FF# +1 Metamorph Monster Samples in Areas have Rewards
# WatchstoneMetamorphExtraSamples2_;T1 map_metamorphosis_additional_sample_rewards;00FF00FF# +2 Metamorph Monster Samples in Areas have Rewards
# WatchstoneMetamorphDoubleRewardChance1;T1 map_metamorphosis_double_reward_chance_%;00FF00FF# 2% chance for Rewards from Metamorphs in Areas to be Doubled
# WatchstoneStackFusing1_;T2 map_currency_fusing_drops_full_stack_permillage_chance;00FF00FF# Orbs of Fusing found in Areas have (0.3 - 0.4)% chance to drop as a full stack
# WatchstoneStackFusing2;T1 map_currency_fusing_drops_full_stack_permillage_chance;00FF00FF# Orbs of Fusing found in Areas have (0.5 - 0.6)% chance to drop as a full stack
# WatchstoneStackChromatic1;T2 map_currency_chromatic_drops_full_stack_permillage_chance;00FF00FF# Chromatic Orbs found in Areas have (0.3 - 0.4)% chance to drop as a full stack
# WatchstoneStackChromatic2;T1 map_currency_chromatic_drops_full_stack_permillage_chance;00FF00FF# Chromatic Orbs found in Areas have (0.5 - 0.6)% chance to drop as a full stack
# WatchstoneStackJeweller1___;T2 map_currency_jeweller_drops_full_stack_permillage_chance;00FF00FF# Jeweller's Orbs found in Areas have (0.3 - 0.4)% chance to drop as a full stack
# WatchstoneStackJeweller2;T1 map_currency_jeweller_drops_full_stack_permillage_chance;00FF00FF# Jeweller's Orbs found in Areas have (0.5 - 0.6)% chance to drop as a full stack
# WatchstoneStackChaos1;T2 map_currency_chaos_drops_full_stack_permillage_chance;00FF00FF# Chaos Orbs found in Areas have (0.4 - 0.5)% chance to drop as a full stack
# WatchstoneStackChaos2;T1 map_currency_chaos_drops_full_stack_permillage_chance;00FF00FF# Chaos Orbs found in Areas have (0.6 - 0.7)% chance to drop as a full stack
# WatchstoneStackAlchemy1;T2 map_currency_alchemy_drops_full_stack_permillage_chance;00FF00FF# Orbs of Alchemy found in Areas have (0.5 - 0.7)% chance to drop as a full stack
# WatchstoneStackAlchemy2;T1 map_currency_alchemy_drops_full_stack_permillage_chance;00FF00FF# Orbs of Alchemy found in Areas have (0.8 - 1.2)% chance to drop as a full stack
# WatchstoneStackAlteration1;T2 map_currency_alteration_drops_full_stack_permillage_chance;00FF00FF# Orbs of Alteration found in Areas have (0.3 - 0.4)% chance to drop as a full stack
# WatchstoneStackAlteration2;T1 map_currency_alteration_drops_full_stack_permillage_chance;00FF00FF# Orbs of Alteration found in Areas have (0.5 - 0.6)% chance to drop as a full stack
# WatchstoneStackScouring1;T2 map_currency_scouring_drops_full_stack_permillage_chance;00FF00FF# Orbs of Scouring found in Areas have 0.2% chance to drop as a full stack
# WatchstoneStackScouring2_;T1 map_currency_scouring_drops_full_stack_permillage_chance;00FF00FF# Orbs of Scouring found in Areas have 0.3% chance to drop as a full stack
# WatchstoneStackChance1;T2 map_currency_chance_drops_full_stack_permillage_chance;00FF00FF# Orbs of Chance found in Areas have (0.3 - 0.4)% chance to drop as a full stack
# WatchstoneStackChance2;T1 map_currency_chance_drops_full_stack_permillage_chance;00FF00FF# Orbs of Chance found in Areas have (0.5 - 0.6)% chance to drop as a full stack
# WatchstoneStackChisel1;T2 map_currency_chisel_drops_full_stack_permillage_chance;00FF00FF# Cartographer's Chisels found in Areas have (0.3 - 0.4)% chance to drop as a full stack
# WatchstoneStackChisel2__;T1 map_currency_chisel_drops_full_stack_permillage_chance;00FF00FF# Cartographer's Chisels found in Areas have (0.5 - 0.6)% chance to drop as a full stack
# WatchstoneStackTransmuatation1___;T2 map_currency_transumation_drops_full_stack_permillage_chance;00FF00FF# Orbs of Transmutation found in Areas have (0.3 - 0.4)% chance to drop as a full stack
# WatchstoneStackTransmuatation2;T1 map_currency_transumation_drops_full_stack_permillage_chance;00FF00FF# Orbs of Transmutation found in Areas have (0.5 - 0.6)% chance to drop as a full stack
# WatchstoneStackWhetstone1;T2 map_currency_whetstone_drops_full_stack_permillage_chance;00FF00FF# Blacksmith's Whetstones found in Areas have (0.5 - 0.7)% chance to drop as a full stack
# WatchstoneStackWhetstone2;T1 map_currency_whetstone_drops_full_stack_permillage_chance;00FF00FF# Blacksmith's Whetstones found in Areas have (0.8 - 1.2)% chance to drop as a full stack
# WatchstoneStackArmourers1_;T2 map_currency_armourers_drops_full_stack_permillage_chance;00FF00FF# Armourer's Scraps found in Areas have (0.3 - 0.4)% chance to drop as a full stack
# WatchstoneStackArmourers2;T1 map_currency_armourers_drops_full_stack_permillage_chance;00FF00FF# Armourer's Scraps found in Areas have (0.5 - 0.6)% chance to drop as a full stack
# WatchstoneStackGemcutter1;T2 map_currency_gemcutter_stack_6_permillage_chance;00FF00FF# (0.8 - 1.2)% chance in Areas for Gemcutter's Prisms to drop as 6 Gemcutter's Prisms instead
# WatchstoneStackGemcutter2__;T1 map_currency_gemcutter_stack_6_permillage_chance;00FF00FF# (1.3 - 1.6)% chance in Areas for Gemcutter's Prisms to drop as 6 Gemcutter's Prisms instead
# WatchstoneStackRegal1;T2 map_currency_regal_stack_5_permillage_chance;00FF00FF# (0.8 - 1.2)% chance in Areas for Regal Orbs to drop as 5 Regal Orbs instead
# WatchstoneStackRegal2___;T1 map_currency_regal_stack_5_permillage_chance;00FF00FF# (1.3 - 1.6)% chance in Areas for Regal Orbs to drop as 5 Regal Orbs instead
# WatchstoneStackDivine1;T1 map_currency_divine_stack_4_permillage_chance;00FF00FF# (1.5 - 2)% chance in Areas for Divine Orbs to drop as 4 Divine Orbs instead
# WatchstoneStackAnnulment1;T1 map_currency_annulment_stack_3_permillage_chance;00FF00FF# (2 - 2.5)% chance in Areas for Orbs of Annulment to drop as 3 Orbs of Annulment instead
# WatchstoneStackRegret1;T2 map_currency_regret_stack_20_permillage_chance;00FF00FF# (0.2 - 0.3)% chance in Areas for Orbs of Regret to drop as 20 Orbs of Regret instead
# WatchstoneStackRegret2;T1 map_currency_regret_stack_20_permillage_chance;00FF00FF# (0.4 - 0.5)% chance in Areas for Orbs of Regret to drop as 20 Orbs of Regret instead
# WatchstoneStackBlessed1;T2 map_currency_blessed_stack_8_permillage_chance;00FF00FF# (0.5 - 0.7)% chance in Areas for Blessed Orbs to drop as 8 Blessed Orbs instead
# WatchstoneStackBlessed2;T1 map_currency_blessed_stack_8_permillage_chance;00FF00FF# (0.8 - 1.2)% chance in Areas for Blessed Orbs to drop as 8 Blessed Orbs instead
# WatchstoneStackMirror1____;T1 map_currency_mirror_stack_2_permillage_chance;00FF00FF# (2 - 2.5)% chance in Areas for Mirrors of Kalandra to drop as 2 Mirrors of Kalandra instead
# WatchstoneStackVaal1___;T2 map_currency_vaal_stack_7_permillage_chance;00FF00FF# (0.5 - 0.7)% chance in Areas for Vaal Orbs to drop as 7 Vaal Orbs instead
# WatchstoneStackVaal2__;T1 map_currency_vaal_stack_7_permillage_chance;00FF00FF# (0.8 - 1.2)% chance in Areas for Vaal Orbs to drop as 7 Vaal Orbs instead
# WatchstoneAmuletAsTalisman1;T1 map_amulet_to_talisman_permillage;00FF00FF# (0.2 - 0.4)% chance in Areas for Amulets to drop as Anointed Talismans instead
# WatchstoneRedBeastExperience1___;T3 map_red_beast_double_experience_chance_%;00FF00FF# Red Beasts in Areas have (10 - 15)% chance to grant double Experience
# WatchstoneRedBeastExperience2_;T2 map_red_beast_double_experience_chance_%;00FF00FF# Red Beasts in Areas have (16 - 20)% chance to grant double Experience
# WatchstoneRedBeastExperience3;T1 map_red_beast_double_experience_chance_%;00FF00FF# Red Beasts in Areas have (21 - 25)% chance to grant double Experience
# WatchstoneIncursionArchitectExperience1_;T3 map_incursion_architect_double_experience_chance_%;00FF00FF# Incursion Architects in Areas have (15 - 25)% chance to grant double Experience
# WatchstoneIncursionArchitectExperience2;T2 map_incursion_architect_double_experience_chance_%;00FF00FF# Incursion Architects in Areas have (26 - 35)% chance to grant double Experience
# WatchstoneIncursionArchitectExperience3;T1 map_incursion_architect_double_experience_chance_%;00FF00FF# Incursion Architects in Areas have (36 - 50)% chance to grant double Experience
# WatchstoneIncursionArchitectIncursionRareChance1;T3 map_incursion_architects_drop_incursion_rare_chance_%;00FF00FF# Incursion Architects in Areas have a (6 - 8)% chance to drop an additional Rare Incursion Item
# WatchstoneIncursionArchitectIncursionRareChance2;T2 map_incursion_architects_drop_incursion_rare_chance_%;00FF00FF# Incursion Architects in Areas have a (9 - 12)% chance to drop an additional Rare Incursion Item
# WatchstoneIncursionArchitectIncursionRareChance3_;T1 map_incursion_architects_drop_incursion_rare_chance_%;00FF00FF# Incursion Architects in Areas have a (13 - 16)% chance to drop an additional Rare Incursion Item
# WatchstoneIncursionPossessionChance1;T3 map_incursion_architects_are_possessed_chance_%;00FF00FF# (6 - 8)% chance for Incursion Architects in Areas to be Possessed by a Tormented Spirit
# WatchstoneIncursionPossessionChance2;T2 map_incursion_architects_are_possessed_chance_%;00FF00FF# (9 - 12)% chance for Incursion Architects in Areas to be Possessed by a Tormented Spirit
# WatchstoneIncursionPossessionChance3;T1 map_incursion_architects_are_possessed_chance_%;00FF00FF# (13 - 16)% chance for Incursion Architects in Areas to be Possessed by a Tormented Spirit
# WatchstoneBetrayalTargetExperience1__;T3 map_betrayal_target_double_experience_chance_%;00FF00FF# Immortal Syndicate Members in Areas have (4 - 7)% chance to grant double Experience
# WatchstoneBetrayalTargetExperience2;T2 map_betrayal_target_double_experience_chance_%;00FF00FF# Immortal Syndicate Members in Areas have (8 - 10)% chance to grant double Experience
# WatchstoneBetrayalTargetExperience3;T1 map_betrayal_target_double_experience_chance_%;00FF00FF# Immortal Syndicate Members in Areas have (11 - 15)% chance to grant double Experience
# WatchstoneMapBossTalismanChance1;T3 map_boss_chance_to_drop_additional_talisman_permillage;00FF00FF# Unique Bosses have (5 - 7)% chance to drop an additional Talisman
# WatchstoneMapBossTalismanChance2;T2 map_boss_chance_to_drop_additional_talisman_permillage;00FF00FF# Unique Bosses have (7.5 - 10)% chance to drop an additional Talisman
# WatchstoneMapBossTalismanChance3_;T1 map_boss_chance_to_drop_additional_talisman_permillage;00FF00FF# Unique Bosses have (11 - 15)% chance to drop an additional Talisman
# WatchstoneMapBossEssenceChance1;T3 map_boss_chance_to_drop_additional_essence_permillage;00FF00FF# Unique Bosses have (10 - 15)% chance to drop an additional Essence
# WatchstoneMapBossEssenceChance2;T2 map_boss_chance_to_drop_additional_essence_permillage;00FF00FF# Unique Bosses have (16 - 20)% chance to drop an additional Essence
# WatchstoneMapBossEssenceChance3;T1 map_boss_chance_to_drop_additional_essence_permillage;00FF00FF# Unique Bosses have (21 - 25)% chance to drop an additional Essence
# WatchstoneMapBossSilverCoinChance1;T3 map_boss_chance_to_drop_additional_silver_coin_permillage;00FF00FF# Unique Bosses have (10 - 15)% chance to drop an additional Silver Coin
# WatchstoneMapBossSilverCoinChance2;T2 map_boss_chance_to_drop_additional_silver_coin_permillage;00FF00FF# Unique Bosses have (16 - 20)% chance to drop an additional Silver Coin
# WatchstoneMapBossSilverCoinChance3___;T1 map_boss_chance_to_drop_additional_silver_coin_permillage;00FF00FF# Unique Bosses have (21 - 25)% chance to drop an additional Silver Coin";
            File.WriteAllText(path, outFile);
            LogMessage("Created WatchstoneWarnings.txt...");
            #endregion
        }
        public void CreateSextantConfig(string path)
        {
            #region Create Default Sextant Mods Config
            new FileInfo(path).Directory.Create();
            string outFile =
@"#Contains;Name in tooltip;RGBA colour code
# SEXTANTS T1
MapAtlasBeyondAndExtraBeyondDemonChance;Beyond;FF00FFFF
MapAtlasBloodlinesDropAdditionalCurrency;Nemesis Currency Drops;FF00FFFF
MapAtlasContainsAlvaMission;Alva;FF00FFFF
MapAtlasContainsEinharMission__;Einhar;FF00FFFF
MapAtlasContainsJunMission;Jun;FF00FFFF
MapAtlasContainsMaster;Master;FF00FFFF
MapAtlasContainsNikoMission;Niko;FF00FFFF
MapAtlasContainsZanaMission;Zana;FF00FFFF
MapAtlasFishy;Alluring;FF00FFFF
MapAtlasHarbingerAndBossAdditionalCurrencyShards;Boss Currency Shards, Extra Shards, Extra Harbinger;FF00FFFF
MapAtlasLegion;Additional Legion;FF00FFFF
MapAtlasStrongboxesAreCorruptedAndUnided;Corrupted Rare Strongboxes, 1 Extra Strongbox;FF00FFFF
MapAtlasStrongboxesAreCorruptedAndUnided2_;Corrupted Rare Strongboxes, 2 Extra Strongboxes;FF00FFFF
MapAtlasStrongboxMonstersEnrageAndDrops;Strongbox Quantity, Enraged Mobs, Extra Strongbox;FF00FFFF
MapAtlasUnIDedMapsGrantQuantity;20% qty and 9% Packsize in unid maps, identified drops in ided maps;FF00FFFF
MapAtlasUnIDedMapsGrantQuantity2;25% qty and 12% Packsize in unid maps, identified drops in ided maps;FF00FFFF
MapAtlasUnIDedMapsGrantQuantity3;30% qty and 15% Packsize in unid maps, identified drops in ided maps;FF00FFFF
# SEXTANTS T2
MapAtlasAdditionalBreaches;Additional Breach;078D36FF
MapAtlasAdditionalBreaches2;2 Additional Breaches;078D36FF
MapAtlasBodyguardsAndBossMapDrops;Boss Bodyguards and Extra Map;078D36FF
MapAtlasBodyguardsAndBossMapDrops2;Boss Bodyguards and 2 Extra Maps;078D36FF
MapAtlasClustersOfBeaconBarrels;15 Beacon Barrels;078D36FF
MapAtlasClustersOfBeaconBarrels2_;25 Beacon Barrels;078D36FF
MapAtlasClustersOfBeaconBarrels3;35 Beacon Barrels;078D36FF
MapAtlasClustersOfBloodwormBarrels;15 Bloodworm Barrels;078D36FF
MapAtlasClustersOfBloodwormBarrels2;25 Bloodworm Barrels;078D36FF
MapAtlasClustersOfBloodwormBarrels3__;35 Bloodworm Barrels;078D36FF
MapAtlasClustersOfParasiteBarrels;15 Parasite Barrels;078D36FF
MapAtlasClustersOfParasiteBarrels2_;25 Parasite Barrels;078D36FF
MapAtlasClustersOfParasiteBarrels3;35 Parasite Barrels;078D36FF
MapAtlasClustersOfVolatileBarrels;15 Volatile Barrels;078D36FF
MapAtlasClustersOfVolatileBarrels2__;25 Volatile Barrels;078D36FF
MapAtlasClustersOfVolatileBarrels3__;35 Volatile Barrels;078D36FF
MapAtlasClustersOfWealthyBarrels;15 Wealthy Barrels;078D36FF
MapAtlasClustersOfWealthyBarrels2;25 Wealthy Barrels;078D36FF
MapAtlasClustersOfWealthyBarrels3;35 Wealthy Barrels;078D36FF
MapAtlasCorruptedMapBossesDropAdditionalVaalItems;5% Corrupted Drops, Vaal Item From Corrupt Boss;078D36FF
MapAtlasHighValueEnemiesOnOwnTeam;Hunted Traitor Mobs;078D36FF
MapAtlasMagicPackSize;20% increased Magic Pack Size;078D36FF
MapAtlasMagicPackSize2;25% increased Magic Pack Size;078D36FF
MapAtlasMagicPackSize3;30% increased Magic Pack Size;078D36FF
MapAtlasReducedReflectedDamageTakenMirroredRares;No Reflect, 3 Additional Packs;078D36FF
MapAtlasReducedReflectedDamageTakenMirroredRares2;No Reflect, 4 Additional Packs;078D36FF
MapAtlasReducedReflectedDamageTakenMirroredRares3;No Reflect, 5 Additional Packs;078D36FF
MapAtlasTormentedBetrayerAndGildedScarabDropChance;Tormented Betrayer, Gilded Scarabs from 3 Possessed Mobs;078D36FF
MapAtlasTormentedBetrayerAndPolishedScarabDropChance;Tormented Betrayer, Polished Scarabs from 3 Possessed Mobs;078D36FF
MapAtlasUniqueMonstersDropCorruptedItems;Unique Monsters drop Corrupted Items;078D36FF
# SEXTANTS T3
MapAtlasAdditionalAbysses;Additional Abyss;FFFFFFFF
MapAtlasAdditionalAbysses2;2 Additional Abysses;FFFFFFFF
MapAtlasChaosDamageAndPacks_;4 Chaos Packs, Increased Chaos Damage;FFFFFFFF
MapAtlasChaosDamageAndPacks2;6 Chaos Packs, Increased Chaos Damage;FFFFFFFF
MapAtlasChaosDamageAndPacks3;8 Chaos Packs, Increased Chaos Damage;FFFFFFFF
MapAtlasColdDamageAndPacks;4 Cold Packs, Increased Cold Damage;FFFFFFFF
MapAtlasColdDamageAndPacks2;6 Cold Packs, Increased Cold Damage;FFFFFFFF
MapAtlasColdDamageAndPacks3;8 Cold Packs, Increased Cold Damage;FFFFFFFF
MapAtlasCorruptedDropWithExtraVaalPacks;4 Vaal Packs with 15% Corrupted Drops;FFFFFFFF
MapAtlasCorruptedDropWithExtraVaalPacks2;4 Vaal Packs with 20% Corrupted Drops;FFFFFFFF
MapAtlasCorruptedDropWithExtraVaalPacks3____;4 Vaal Packs with 25% Corrupted Drops;FFFFFFFF
MapAtlasFireDamageAndPacks;4 Fire Packs, Increased Fire Damage;FFFFFFFF
MapAtlasFireDamageAndPacks2_;6 Fire Packs, Increased Fire Damage;FFFFFFFF
MapAtlasFireDamageAndPacks3;8 Fire Packs, Increased Fire Damage;FFFFFFFF
MapAtlasGloomShrineEffectAndDuration;Shrine Duration, Extra Gloom Shrine;FFFFFFFF
MapAtlasGloomShrineEffectAndDuration2;Shrine Duration, Extra Gloom and Other Shrine;FFFFFFFF
MapAtlasInstantFlasksAndHealingMonsters;4 Healing packs, Instant Flasks;FFFFFFFF
MapAtlasInstantFlasksAndHealingMonsters2_;6 Healing packs, Instant Flasks;FFFFFFFF
MapAtlasInstantFlasksAndHealingMonsters3;8 Healing packs, Instant Flasks;FFFFFFFF
MapAtlasLightningDamageAndPacks;4 Lightning Packs, Increased Lightning Damage;FFFFFFFF
MapAtlasLightningDamageAndPacks2;6 Lightning Packs, Increased Lightning Damage;FFFFFFFF
MapAtlasLightningDamageAndPacks3;8 Lightning Packs, Increased Lightning Damage;FFFFFFFF
MapAtlasMagicAndRareMapsHaveAdditionalPacks;3 Additional Packs of Map Rarity;FFFFFFFF
MapAtlasMagicAndRareMapsHaveAdditionalPacks2;4 Additional Packs of Map Rarity;FFFFFFFF
MapAtlasMagicAndRareMapsHaveAdditionalPacks3;5 Additional Packs of Map Rarity;FFFFFFFF
MapAtlasMontersThatConvertOnDeath__;4 Conversion Packs;FFFFFFFF
MapAtlasMontersThatConvertOnDeath2__;6 Conversion Packs;FFFFFFFF
MapAtlasMontersThatConvertOnDeath3;8 Conversion Packs;FFFFFFFF
MapAtlasNoSoulGainPreventionWithExtraVaalPacks;4 Vaal Packs, No  Soul Gain Prevention;FFFFFFFF
MapAtlasPhysicalDamageAndPacks_;4 Physical Packs, Increased Physical Damage;FFFFFFFF
MapAtlasPhysicalDamageAndPacks2__;6 Physical Packs, Increased Physical Damage;FFFFFFFF
MapAtlasPhysicalDamageAndPacks3_;8 Physical Packs, Increased Physical Damage;FFFFFFFF
MapAtlasPoisonEffectsAndParasites;Low Tier Buffs for Poison Stacks;FFFFFFFF
MapAtlasPoisonEffectsAndParasites2_;Mid Tier Buffs for Poison Stacks;FFFFFFFF
MapAtlasPoisonEffectsAndParasites3;High Tier Buffs for Poison Stacks;FFFFFFFF
MapAtlasResonatingShrineEffectAndDuration_;Shrine Duration, Extra Resonating Shrine;FFFFFFFF
MapAtlasResonatingShrineEffectAndDuration2;Shrine Duration, Extra Resonating and Other Shrine;FFFFFFFF
MapAtlasTormentedBetrayerAndScarabDropChance;Tormented Betrayer, Rusted Scarabs from 3 Possessed Mobs;FFFFFFFF
MapAtlasUniqueStrongboxChanceWithExtraVaalPacks;4 Vaal Packs, Sacrifice or Mortal Frag Bonuses;FFFFFFFF
MapAtlasUniqueStrongboxChanceWithExtraVaalPacks2;6 Vaal Packs, Sacrifice or Mortal Frag Bonuses;FFFFFFFF
MapAtlasUniqueStrongboxChanceWithExtraVaalPacks3_;8 Vaal Packs, Sacrifice or Mortal Frag Bonuses;FFFFFFFF
MapAtlasVaalSoulsOnKillAndExtraVaalPacks;4 Vaal Packs, Soul Gain Buffed;FFFFFFFF
MapAtlasVaalSoulsOnKillAndExtraVaalPacks2;6 Vaal Packs, Soul Gain Buffed;FFFFFFFF
MapAtlasVaalSoulsOnKillAndExtraVaalPacks3;8 Vaal Packs, Soul Gain Buffed;FFFFFFFF
# SEXTANTS T4
MapAtlasAdditionalEssencesAndRemnantChance_;Extra Essences;643232FF
MapAtlasBossAdditionalUnique;Bosses Drop Additional Unique;643232FF
MapAtlasBossDropsCorruptedItems;Boss Drops are Corrupted;643232FF
MapAtlasRogueExilesDamageAndDropAdditionalJewels;2 Extra Rogue Exiles, Buff exiles that drop 2 Jewels;643232FF
MapAtlasTormentedGraverobberAndUniqueDropChance;Tormented Graverobber, Uniques from 3 Possessed Mobs;643232FF
MapAtlasTormentedHereticAndMapDropChance;Tormented Heretic, Maps from 3 Possessed Mobs;643232FF
# SEXTANTS T5
MapAtlasContainsAdditionalRandomBoss;Invasion Boss;FF0000FF
MapAtlasMapQualityBonusAlsoAppliesToRarity;Quality also applies to Rarity;FF0000FF
MapAtlasMapsHave20PercentQuality;Maps have 20% Quality;FF0000FF";
            File.WriteAllText(path, outFile);
            LogMessage("Created SextantWarnings.txt...");
            #endregion
        }
    }
}