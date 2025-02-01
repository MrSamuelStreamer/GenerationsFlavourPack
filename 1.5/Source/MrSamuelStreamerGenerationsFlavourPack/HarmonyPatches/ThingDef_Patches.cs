using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MSS_Gen.TechAdvancingCompat;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(ThingDef))]
public static class ThingDef_Patches
{
    // [HarmonyPatch(nameof(ThingDef.AllRecipes), MethodType.Getter)]
    // [HarmonyPrefix]
    // public static bool AllRecipes_Getter(ThingDef __instance, ref List<RecipeDef> __result)
    // {
    //     if (DisableForTechLevelDef.DisabledForThisTechLevel().SelectMany(def => def.things).Any(thingdef => thingdef == __instance))
    //     {
    //         __result = [];
    //         return false;
    //     }
    //
    //     return true;
    // }
    //
    // [HarmonyPatch(nameof(ThingDef.Claimable), MethodType.Getter)]
    // [HarmonyPrefix]
    // public static bool Claimable_Getter(ThingDef __instance, ref bool __result)
    // {
    //     if (DisableForTechLevelDef.DisabledForThisTechLevel().SelectMany(def => def.things).Any(thingdef => thingdef == __instance))
    //     {
    //         __result = false;
    //         return false;
    //     }
    //
    //     return true;
    // }
    //
    // [HarmonyPatch(nameof(ThingDef.DescriptionDetailed), MethodType.Getter)]
    // [HarmonyPrefix]
    // public static bool DescriptionDetailed_Getter(ThingDef __instance, ref string __result)
    // {
    //     if (DisableForTechLevelDef.DisabledForThisTechLevel().SelectMany(def => def.things).Any(thingdef => thingdef == __instance))
    //     {
    //         __result = "MSS_Gen_TooAdvancedDescription".Translate();
    //         return false;
    //     }
    //
    //     return true;
    // }
}
