using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MSS_Gen.Comps;

public class CompLegendaryTracker: ThingComp
{
    public class PawnUsageTracker: IExposable
    {
        public int ticksEquipped = 0;
        public int timesUsed = 0;
        public bool diedWhileEquipped = false;

        public int LastEquippedAt = -1;
        public int LastUnequippedAt = -1;

        public int Kills = 0;

        public void ExposeData()
        {
            Scribe_Values.Look(ref ticksEquipped, "ticksEquipped");
            Scribe_Values.Look(ref timesUsed, "timesUsed");
            Scribe_Values.Look(ref diedWhileEquipped, "diedWhileEquipped");
        }
    }

    public TechLevel BecameLegendaryAtTechLevel = TechLevel.Undefined;

    public Dictionary<Pawn, PawnUsageTracker> pawnUsageTrackers = new();

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref BecameLegendaryAtTechLevel, "BecameLegendaryAtTechLevel", TechLevel.Undefined);
        Scribe_Collections.Look(ref pawnUsageTrackers, "pawnUsageTrackers", LookMode.Reference, LookMode.Value);
    }

    public PawnUsageTracker TrackerForPawn(Pawn pawn)
    {
        if (!pawnUsageTrackers.TryGetValue(pawn, out PawnUsageTracker tracker))
        {
            tracker = new PawnUsageTracker();
            pawnUsageTrackers.Add(pawn, tracker);
        }

        return tracker;
    }

    public void AddToEquippedTime(PawnUsageTracker tracker)
    {
        if (tracker.LastEquippedAt > 0 && tracker.LastUnequippedAt > 0 && tracker.LastEquippedAt < tracker.LastUnequippedAt)
        {
            tracker.ticksEquipped += tracker.LastUnequippedAt - tracker.LastEquippedAt;

            tracker.LastEquippedAt = -1;
        }
    }

    public override void Notify_Equipped(Pawn pawn)
    {
        base.Notify_Equipped(pawn);
        PawnUsageTracker tracker = TrackerForPawn(pawn);

        AddToEquippedTime(tracker);

        tracker.LastEquippedAt = Find.TickManager.TicksGame;
    }

    public override void Notify_Unequipped(Pawn pawn)
    {
        base.Notify_Unequipped(pawn);
        PawnUsageTracker tracker = TrackerForPawn(pawn);

        TrackerForPawn(pawn).LastUnequippedAt = Find.TickManager.TicksGame;
        AddToEquippedTime(tracker);
    }

    public override void Notify_UsedVerb(Pawn pawn, Verb verb)
    {
        base.Notify_UsedVerb(pawn, verb);
        TrackerForPawn(pawn).timesUsed++;
    }

    public override void Notify_UsedWeapon(Pawn pawn)
    {
        base.Notify_UsedWeapon(pawn);
        TrackerForPawn(pawn).timesUsed++;
    }

    public override void Notify_KilledPawn(Pawn pawn)
    {
        base.Notify_KilledPawn(pawn);
        TrackerForPawn(pawn).Kills++;
    }

    public override void Notify_WearerDied()
    {
        base.Notify_WearerDied();
        if (parent.ParentHolder is Pawn_EquipmentTracker equipmentTracker)
        {
            TrackerForPawn(equipmentTracker.pawn).diedWhileEquipped = true;
        }
    }

    public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
    {
        base.Notify_Killed(prevMap, dinfo);
        ModLog.Debug("here");
    }

    public override void Notify_KilledLeavingsLeft(List<Thing> leavings)
    {
        base.Notify_KilledLeavingsLeft(leavings);
        ModLog.Debug("here");
    }

}
