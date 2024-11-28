using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MSS_Gen.TechAdvancingCompat;

public class DisableForTechLevelDef: Def
{
    public static Lazy<Type> RulesCls = new(()=> AccessTools.TypeByName("TechAdvancing.Rules"));
    public static Lazy<MethodInfo> GetNewTechLevel = new(()=> AccessTools.Method(RulesCls.Value, "GetNewTechLevel"));

    public TechLevel techLevel;
    public List<RecipeDef> recipes;
    public List<ThingDef> things;
    public List<QuestScriptDef> quests;
    public List<IncidentDef> incidents;

    public static IEnumerable<DisableForTechLevelDef> DisabledForThisTechLevel()
    {
        MethodInfo getNewTechLevel = GetNewTechLevel.Value;

        TechLevel tl = (TechLevel)getNewTechLevel.Invoke(null, []);

        return tl == TechLevel.Undefined ? [] : DefDatabase<DisableForTechLevelDef>.AllDefsListForReading.Where(def => def.techLevel == tl);
    }
}
