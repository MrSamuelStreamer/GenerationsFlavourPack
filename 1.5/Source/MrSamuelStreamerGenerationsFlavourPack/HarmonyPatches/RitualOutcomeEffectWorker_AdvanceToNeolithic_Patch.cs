using HarmonyLib;
using RimWorld;
using Verse;
using VFETribals;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(RitualOutcomeEffectWorker_AdvanceToNeolithic))]
public static class RitualOutcomeEffectWorker_AdvanceToNeolithic_Patch
{
    [HarmonyPatch(nameof(RitualOutcomeEffectWorker_AdvanceToNeolithic.Apply))]
    [HarmonyPostfix]
    public static void Apply()
    {
        TechConfigWorldComponent comp = Find.World.GetComponent<TechConfigWorldComponent>();

        comp?.SetNewConfigs(TechLevel.Neolithic);
    }
}
