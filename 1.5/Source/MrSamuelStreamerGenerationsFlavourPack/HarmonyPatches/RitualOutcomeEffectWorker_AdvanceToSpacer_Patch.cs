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

        comp?.SetNewConfigs(TechLevel.Spacer);
    }
}
