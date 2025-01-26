using HarmonyLib;
using RimWorld;
using Verse;
using VFETribals;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(RitualOutcomeEffectWorker_AdvanceToIndustrial))]
public static class RitualOutcomeEffectWorker_AdvanceToIndustrial_Patch
{
    [HarmonyPatch(nameof(RitualOutcomeEffectWorker_AdvanceToIndustrial.Apply))]
    [HarmonyPostfix]
    public static void Apply()
    {
        TechConfigWorldComponent comp = Find.World.GetComponent<TechConfigWorldComponent>();

        ModLog.Log("Setting new configs to industrial");
        comp?.SetNewConfigs(TechLevel.Industrial);
    }
}
