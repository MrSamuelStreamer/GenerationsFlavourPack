using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace MSS_Gen;

public class JobDriver_BecomeArcho : JobDriver
{
    private Mote mote;
    private Sustainer sound;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(
            Nexus,
            job,
            errorOnFailed: errorOnFailed);
    }

    public Thing Nexus => job.targetA.Thing;

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell);
        Toil waitToil = Toils_General.Wait(5000);
        waitToil.AddPreInitAction((Action) (() => Messages.Message("MSSGen_ConversionBegins".Translate(pawn.Named("PAWN")), (Thing) pawn, MessageTypeDefOf.PositiveEvent)));
        waitToil.AddPreInitAction((Action) (() => SoundDefOf.Bestowing_Start.PlayOneShot((SoundInfo) (Thing) pawn)));
        waitToil.tickAction = (Action) (() =>
        {
            pawn.rotationTracker.FaceTarget((LocalTargetInfo) Nexus);
            if (mote == null || mote.Destroyed)
                mote = MoteMaker.MakeStaticMote((pawn.TrueCenter() + Nexus.TrueCenter()) / 2f, pawn.Map, ThingDefOf.Mote_Bestow);
            mote?.Maintain();
            if ((sound == null || sound.Ended) && waitToil.actor.jobs.curDriver.ticksLeftThisToil <= 307)
                sound = SoundDefOf.Bestowing_Warmup.TrySpawnSustainer(SoundInfo.InMap(new TargetInfo(pawn.Position, pawn.Map), MaintenanceType.PerTick));
            if (sound == null)
                return;
            sound.Maintain();
        });
        waitToil.handlingFacing = false;
        waitToil.socialMode = RandomSocialMode.Off;
        waitToil.WithProgressBarToilDelay(TargetIndex.A);
        yield return waitToil;
        yield return Toils_General.Do((Action) (() =>
        {
            pawn.genes.SetXenotype(MSS_GenDefOf.MSS_Gen_Archoseed);
            FleckMaker.Static((pawn.TrueCenter() + Nexus.TrueCenter()) / 2f, pawn.Map, FleckDefOf.PsycastAreaEffect, 2f);
            SoundDefOf.Bestowing_Finished.PlayOneShot((SoundInfo) (Thing) pawn);
        }));

    }
}
