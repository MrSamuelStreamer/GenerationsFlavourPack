using HarmonyLib;
using RimWorld;
using VFETribals;

namespace MSS_Gen.TechAdvancingCompat.HarmonyPatches;

[HarmonyPatch(typeof(RitualObligationTargetWorker_AnyGatherSpot))]
public static class RitualObligationTargetWorker_AnyGatherSpotForAdvancement_Patch
{
    [HarmonyPatch("CanUseTargetInternal")]
    [HarmonyPostfix]
    public static void CanUseTargetInternal(RitualObligationTargetWorker_AnyGatherSpot __instance, ref RitualTargetUseReport __result)
    {
        if(__instance is RitualObligationTargetWorker_AnyGatherSpotForAdvancement)
            __result.canUse = false;
    }
}
