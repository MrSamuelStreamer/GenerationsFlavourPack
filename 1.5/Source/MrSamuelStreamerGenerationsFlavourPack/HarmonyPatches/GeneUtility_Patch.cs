using HarmonyLib;
using RimWorld;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(GeneUtility))]
public static class GeneUtility_Patch
{
    [HarmonyPatch(nameof(GeneUtility.CanDeathrest))]
    [HarmonyPostfix]
    public static void CanDeathrestPostfix(Pawn pawn, ref bool __result)
    {
        if(__result) return;

        // __result = ModsConfig.BiotechActive && pawn.genes != null && pawn.genes.GetFirstGeneOfType<Gene_VoidsEmbrace>() != null;
    }
}
