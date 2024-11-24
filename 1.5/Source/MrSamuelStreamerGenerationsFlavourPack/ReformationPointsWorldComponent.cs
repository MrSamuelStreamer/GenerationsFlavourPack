using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MSS_Gen;

public class ReformationPointsWorldComponent(World world) : WorldComponent(world), ISignalReceiver
{
    public static int OneYear = 3600000;
    public int NextYearTick = OneYear;

    public override void WorldComponentTick()
    {
        base.WorldComponentTick();
        if (Find.TickManager.TicksGame > NextYearTick)
        {
            NextYearTick += OneYear;
            AddPoints(MSS_GenMod.settings.ReformationPointsPerYear);
            Messages.Message("MSS_Gen_NewYear".Translate(MSS_GenMod.settings.ReformationPointsPerDefeatedFaction), MessageTypeDefOf.PositiveEvent, true);
        }
    }

    public void AddPoints(int points)
    {
        if (Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.Fluid)
        {
            if (!Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.development.TryAddDevelopmentPoints(points))
            {
                ModLog.Debug("Couldn't add reformation points");
            }
        }
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        Find.SignalManager.RegisterReceiver(this);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref NextYearTick, "NextYearTick");
    }

    public void Notify_SignalReceived(Signal signal)
    {
        switch (signal.tag)
        {
            case Signals.MSS_Gen_BabyAddedToFaction:
                AddPoints(MSS_GenMod.settings.ReformationPointsPerBaby);
                Messages.Message("MSS_Gen_BabyAddedToFaction".Translate(MSS_GenMod.settings.ReformationPointsPerBaby), MessageTypeDefOf.PositiveEvent, true);
                break;
            case Signals.MSS_Gen_FactionDefeated:
                AddPoints(MSS_GenMod.settings.ReformationPointsPerDefeatedFaction);
                Messages.Message("MSS_Gen_FactionDefeated".Translate(signal.args.GetArg(0), MSS_GenMod.settings.ReformationPointsPerDefeatedFaction), MessageTypeDefOf.PositiveEvent, true);
                break;
        }
    }
}
