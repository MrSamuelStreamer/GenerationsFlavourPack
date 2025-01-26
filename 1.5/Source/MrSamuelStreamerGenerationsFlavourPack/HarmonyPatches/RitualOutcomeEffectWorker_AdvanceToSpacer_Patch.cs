using HarmonyLib;
using RimWorld;
using Verse;
using VFETribals;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(RitualOutcomeEffectWorker_AdvanceToSpacer))]
public static class RitualOutcomeEffectWorker_AdvanceToSpacer_Patch
{
    [HarmonyPatch(nameof(RitualOutcomeEffectWorker_AdvanceToSpacer.Apply))]
    [HarmonyPostfix]
    public static void Apply()
    {
        TechConfigWorldComponent comp = Find.World.GetComponent<TechConfigWorldComponent>();

        ModLog.Log("Setting new configs to spacer");
        comp?.SetNewConfigs(TechLevel.Spacer);
    }
}
