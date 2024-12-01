using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_GeneTracker))]
public static class Pawn_GeneTracker_Patch
{
    [HarmonyPatch(nameof(Pawn_GeneTracker.HasActiveGene))]
    [HarmonyPrefix]
    public static bool HasActiveGene_Patch(Pawn_GeneTracker __instance, GeneDef geneDef, ref bool __result)
    {
        // VoidsEmbrace counts as deathless for any checks
        if (geneDef != GeneDefOf.Deathless) return true;

         __result = OrigHasActiveGene(__instance, GeneDefOf.Deathless) || OrigHasActiveGene(__instance, MSS_GenDefOf.MSS_Gen_VoidsEmbrace);

        return false;
    }

    public static bool OrigHasActiveGene(Pawn_GeneTracker __instance, GeneDef geneDef)
    {
        return Enumerable.Any(__instance.GenesListForReading, t => t.def == geneDef && t.Active);
    }
}
