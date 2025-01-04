using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(Pawn))]
public static class Pawn_Patch
{
    public static ReformationPointsWorldComponent Comp => Find.World.GetComponent<ReformationPointsWorldComponent>();
    public static Storyteller storyteller => Find.Storyteller;

    [HarmonyPatch("DoKillSideEffects")]
    [HarmonyPostfix]
    public static void DoKillSideEffects(Pawn __instance)
    {
        if(Comp == null) return;
        if(!__instance.IsColonist) return;

        int ticksInColony = __instance.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal);
        float yearsInColony = (float)ticksInColony / GenDate.TicksPerYear;

        float adjustedYearsInColony = yearsInColony * storyteller.difficulty.adultAgingRate;

        if(adjustedYearsInColony <= 50) return;
        int points = Mathf.FloorToInt(adjustedYearsInColony / 50);

        if(Comp.AddPoints(points))
            Messages.Message("MSS_Gen_ReformationPointsYearsInColony".Translate(__instance.Name.ToString(), Mathf.FloorToInt(adjustedYearsInColony), points), MessageTypeDefOf.PositiveEvent, true);

    }
}
