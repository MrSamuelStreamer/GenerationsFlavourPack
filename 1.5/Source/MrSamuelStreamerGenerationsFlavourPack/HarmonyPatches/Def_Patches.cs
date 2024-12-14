using System.Linq;
using HarmonyLib;
using MSS_Gen.TechAdvancingCompat;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(Def))]
public static class Def_Patches
{
    [HarmonyPatch(nameof(Def.LabelCap), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool LabelCap_Getter(Def __instance, ref TaggedString __result)
    {
        if (__instance is not ThingDef) return true;
        if (DisableForTechLevelDef.DisabledForThisTechLevel().SelectMany(def => def.things).Any(thingdef => thingdef == __instance))
        {
            __result = "MSS_Gen_TooAdvancedLabelCap".Translate(__instance.label);
            return false;
        }

        return true;
    }
}
