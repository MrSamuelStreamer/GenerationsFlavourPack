using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace MSS_Gen.TechAdvancingCompat.HarmonyPatches;

[HarmonyPatch(typeof(QuestScriptDef))]
public static class QuestScriptDef_Patches
{
    public static bool CanRunPatch(QuestScriptDef __instance, ref bool __result)
    {
        if (DisableForTechLevelDef.DisabledForThisTechLevel().SelectMany(def => def.quests).Any(quest => quest == __instance))
        {
            __result = false;
            return false;
        }

        return true;
    }

    [HarmonyPatch(nameof(QuestScriptDef.CanRun), [typeof(float)])]
    [HarmonyPrefix]
    public static bool CanRun_Patch1(QuestScriptDef __instance, ref bool __result)
    {
        return CanRunPatch(__instance, ref __result);
    }

    [HarmonyPatch(nameof(QuestScriptDef.CanRun), [typeof(Slate)])]
    [HarmonyPrefix]
    public static bool CanRun_Patch2(QuestScriptDef __instance, ref bool __result)
    {
        return CanRunPatch(__instance, ref __result);
    }

}
