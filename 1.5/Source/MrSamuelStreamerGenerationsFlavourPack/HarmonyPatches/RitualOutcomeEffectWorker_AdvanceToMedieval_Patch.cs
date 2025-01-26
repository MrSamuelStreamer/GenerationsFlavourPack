using HarmonyLib;
using RimWorld;
using Verse;
using VFETribals;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(RitualOutcomeEffectWorker_AdvanceToMedieval))]
public static class RitualOutcomeEffectWorker_AdvanceToMedieval_Patch
{
    [HarmonyPatch(nameof(RitualOutcomeEffectWorker_AdvanceToMedieval.Apply))]
    [HarmonyPostfix]
    public static void Apply()
    {
        TechConfigWorldComponent comp = Find.World.GetComponent<TechConfigWorldComponent>();

        ModLog.Log("Setting new configs to medieval");
        comp?.SetNewConfigs(TechLevel.Medieval);
    }
}
