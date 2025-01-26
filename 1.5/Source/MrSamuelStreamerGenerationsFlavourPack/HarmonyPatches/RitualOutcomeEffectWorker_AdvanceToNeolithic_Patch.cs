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
        ModLog.Log("Setting new configs to neolithic");
        comp?.SetNewConfigs(TechLevel.Neolithic);
    }
}
