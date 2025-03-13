using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(FloatMenuMakerMap))]
public static class FloatMenuMakerMap_Patch
{
    [HarmonyPatch("AddHumanlikeOrders")]
    [HarmonyPostfix]
    public static void AddHumanlikeOrders(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
    {
        if(!pawn.NonHumanlikeOrWildMan() || pawn.genes.Xenotype == MSS_GenDefOf.MSS_Gen_Archoseed) return;
        IntVec3 clickCell = IntVec3.FromVector3(clickPos);

        foreach (Building_ArchonexusCore core in pawn.Map.thingGrid.ThingsAt(clickCell).OfType<Building_ArchonexusCore>())
        {
            opts.Add(new FloatMenuOption("MSSGen_Convert".Translate(pawn.NameShortColored), () =>
            {
                Job newJob = JobMaker.MakeJob(MSS_GenDefOf.MSSGen_BecomeArcho, (LocalTargetInfo) core);
                newJob.count = 1;
                newJob.playerForced = true;
                pawn.jobs.StartJob(newJob, JobCondition.InterruptForced);
            }));
        }
    }
}
