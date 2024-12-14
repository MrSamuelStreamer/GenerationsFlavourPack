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

    public static IEnumerable<DisableForTechLevelDef> DisabledForThisTechLevel()
    {
        if (Find.World == null || Find.FactionManager == null) return Enumerable.Empty<DisableForTechLevelDef>();
        TechLevel tl = Find.FactionManager.OfPlayer.def.techLevel;

        return tl == TechLevel.Undefined ? [] : DefDatabase<DisableForTechLevelDef>.AllDefsListForReading.Where(def => def.techLevel == tl);
    }
}
