using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MSS_Gen.TechAdvancingCompat;

public class DisableForTechLevelDef: Def
{
    public TechLevel techLevel;
    public List<RecipeDef> recipes;
    public List<ThingDef> things;
    public List<QuestScriptDef> quests;
    public List<IncidentDef> incidents;

    public static Dictionary<TechLevel, List<DisableForTechLevelDef>> _disabledForTechLevels = new();
    public static Dictionary<TechLevel, List<RecipeDef>> _disabledRecipiesForTechLevels = new();

    public static TechLevel GetTechLevel()
    {
        if (Find.World == null || Find.FactionManager == null || Find.FactionManager.OfPlayer == null || Find.FactionManager.OfPlayer.def == null) return TechLevel.Undefined;
        return Find.FactionManager.OfPlayer.def.techLevel;
    }

    public static IEnumerable<DisableForTechLevelDef> DisabledForThisTechLevel()
    {
        TechLevel tl = GetTechLevel();

        if (tl == TechLevel.Undefined) return [];

        if (!_disabledForTechLevels.ContainsKey(tl))
        {
            _disabledForTechLevels[tl] = DefDatabase<DisableForTechLevelDef>.AllDefsListForReading.Where(def => def.techLevel == tl).ToList();
        }

        return _disabledForTechLevels[tl];
    }

    public static IEnumerable<RecipeDef> DisabledRecipiesForTechLevels()
    {
        TechLevel tl = GetTechLevel();

        if (tl == TechLevel.Undefined) return [];

        if (!_disabledRecipiesForTechLevels.ContainsKey(tl))
        {
            IEnumerable<DisableForTechLevelDef> defs = DisabledForThisTechLevel();
            _disabledRecipiesForTechLevels[tl] = defs.SelectMany(def => def.recipes).ToList();
        }

        return _disabledRecipiesForTechLevels[tl];
    }
}
