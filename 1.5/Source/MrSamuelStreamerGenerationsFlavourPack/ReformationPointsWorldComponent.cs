using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MSS_Gen;

public class ReformationPointsWorldComponent(World world) : WorldComponent(world), ISignalReceiver
{
    public int NextYearTick = GenDate.TicksPerYear;
    public int TechsCompletedSinceLastGivingPoints = 0;

    public override void WorldComponentTick()
    {
        base.WorldComponentTick();
        if (Find.TickManager.TicksGame > NextYearTick)
        {
            NextYearTick = Find.TickManager.TicksGame + GenDate.TicksPerYear;
            if(AddPoints(MSS_GenMod.settings.ReformationPointsPerYear))
                Messages.Message("MSS_Gen_NewYear".Translate(MSS_GenMod.settings.ReformationPointsPerDefeatedFaction), MessageTypeDefOf.PositiveEvent, true);

            int yearsPassed = Find.TickManager.TicksGame / GenDate.TicksPerYear;

            if (yearsPassed % 5 == 0)
            {
                if(AddPoints(yearsPassed))
                    Messages.Message("MSS_Gen_YearsPassed".Translate(yearsPassed), MessageTypeDefOf.PositiveEvent, true);
            }
        }
    }

    public bool AddPoints(int points)
    {
        if (Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.Fluid)
        {
            if (!Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.development.TryAddDevelopmentPoints(points))
            {
                ModLog.Warn($"Couldn't add reformation points to ideo {Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.name}");
                return false;
            }
        }
        else
        {
            ModLog.Warn("Couldn't find fluid ideo to add development points to.");
            return false;
        }

        return true;
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
        Scribe_Values.Look(ref TechsCompletedSinceLastGivingPoints, "TechsCompletedSinceLastGivingPoints", 0);
    }

    public void Notify_SignalReceived(Signal signal)
    {
        switch (signal.tag)
        {
            case Signals.MSS_Gen_BabyAddedToFaction:
                if(AddPoints(MSS_GenMod.settings.ReformationPointsPerBaby))
                    Messages.Message("MSS_Gen_BabyAddedToFaction".Translate(MSS_GenMod.settings.ReformationPointsPerBaby), MessageTypeDefOf.PositiveEvent, true);
                break;
            case Signals.MSS_Gen_SettlementDefeated:
                break;
                if(AddPoints(MSS_GenMod.settings.ReformationPointsPerDefeatedSettlement))
                    Messages.Message("MSS_Gen_SettlementDefeated".Translate(signal.args.GetArg(1),signal.args.GetArg(0), MSS_GenMod.settings.ReformationPointsPerDefeatedSettlement), MessageTypeDefOf.PositiveEvent, true);
                break;
            case Signals.MSS_Gen_FactionDefeated:
                if(AddPoints(MSS_GenMod.settings.ReformationPointsPerDefeatedFaction))
                    Messages.Message("MSS_Gen_FactionDefeated".Translate(signal.args.GetArg(0), MSS_GenMod.settings.ReformationPointsPerDefeatedFaction), MessageTypeDefOf.PositiveEvent, true);
                break;
            case Signals.MSS_Gen_TechLevelChanged:
                if(AddPoints(MSS_GenMod.settings.ReformationPointsPerTechLevel))
                    Messages.Message("MSS_Gen_TechLeve".Translate(signal.args.GetArg(0), MSS_GenMod.settings.ReformationPointsPerDefeatedFaction), MessageTypeDefOf.PositiveEvent, true);
                break;
            case "ResearchCompleted":
                TechsCompletedSinceLastGivingPoints++;
                if(TechsCompletedSinceLastGivingPoints < MSS_GenMod.settings.TechsToGetPoints) break;
                TechsCompletedSinceLastGivingPoints = 0;

                if(AddPoints(MSS_GenMod.settings.ReformationPointsForTechs))
                    Messages.Message("MSS_Gen_ReformationPointsForTechs".Translate(MSS_GenMod.settings.TechsToGetPoints, MSS_GenMod.settings.ReformationPointsForTechs), MessageTypeDefOf.PositiveEvent, true);
                break;
        }
    }
}
