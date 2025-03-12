using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(Building_ArchonexusCore))]
public static class Building_ArchonexusCore_Patch
{
    [HarmonyPatch(nameof(Building_ArchonexusCore.GetMultiSelectFloatMenuOptions))]
    [HarmonyPostfix]
    public static void GetMultiSelectFloatMenuOptions(Building_ArchonexusCore __instance, List<Pawn> selPawns, ref IEnumerable<FloatMenuOption> __result)
    {
        List<FloatMenuOption> options = new();
        options.AddRange(__result);

        foreach (Pawn pawn in selPawns.Where(p=>p.CanReach(__instance, PathEndMode.InteractionCell, Danger.Deadly)))
        {
            options.Add(new FloatMenuOption("MSSGen_Convert".Translate(pawn.NameShortColored), () =>
            {
                Job newJob = JobMaker.MakeJob(MSS_GenDefOf.MSSGen_BecomeArcho, (LocalTargetInfo) __instance);
                newJob.count = 1;
                newJob.playerForced = true;
                pawn.jobs.StartJob(newJob, JobCondition.InterruptForced);
            }));
        }

        __result = options;
    }
}
