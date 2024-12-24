using HarmonyLib;
using RimWorld;
using Verse;
using VFETribals;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(RitualOutcomeEffectWorker_AdvanceToUltra))]
public static class RitualOutcomeEffectWorker_AdvanceToUltra_Patch
{
    [HarmonyPatch(nameof(RitualOutcomeEffectWorker_AdvanceToUltra.Apply))]
    [HarmonyPostfix]
    public static void Apply()
    {
        TechConfigWorldComponent comp = Find.World.GetComponent<TechConfigWorldComponent>();

        comp?.SetNewConfigs(TechLevel.Ultra);
    }
}
