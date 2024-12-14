using System.Linq;
using HarmonyLib;
using MSS_Gen.TechAdvancingCompat;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(RecipeDef))]
public static class RecipeDef_Patches
{
    public static bool Available(RecipeDef __instance, ref bool __result)
    {
        if (DisableForTechLevelDef.DisabledForThisTechLevel().SelectMany(def => def.recipes).Any(rec => rec == __instance))
        {
            __result = false;
            return false;
        }

        return true;
    }

    [HarmonyPatch(nameof(RecipeDef.AvailableNow), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool RecipeDef_AvailableNow_Getter_Patch(RecipeDef __instance, ref bool __result)
    {
        return Available(__instance, ref __result);
    }

    [HarmonyPatch(nameof(RecipeDef.AvailableOnNow))]
    [HarmonyPrefix]
    public static bool RecipeDef_AvailableOnNow_Getter_Patch(RecipeDef __instance, ref bool __result)
    {
        return Available(__instance, ref __result);
    }

}
