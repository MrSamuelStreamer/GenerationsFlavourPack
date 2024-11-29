using RimWorld;
using Verse;

namespace MSS_Gen.Quests;

public class QuestPart_MarryAway : QuestPart
{
    public string inSignal;
    public Pawn asker;
    public Pawn proposee;


    public override void Notify_QuestSignalReceived(Signal signal)
    {
        base.Notify_QuestSignalReceived(signal);
        if (signal.tag != inSignal)
            return;

        proposee.SetFaction(asker.Faction);
        MarriageCeremonyUtility.Married(asker, proposee);

        LeaveQuestPartUtility.MakePawnsLeave([proposee], true, this.quest, true);
        quest.End(QuestEndOutcome.Success, false, true);
        Find.LetterStack.ReceiveLetter("MSS_Gen_QuestPart_MarryAwayEndLetterLabel".Translate(proposee.NameShortColored), "MSS_Gen_QuestPart_MarryAwayEndLetterText".Translate(proposee.NameFullColored, asker.NameFullColored, asker.Faction.NameColored), LetterDefOf.NeutralEvent, (LookTargets) proposee, quest: this.quest, playSound: true);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref asker, "asker");
        Scribe_References.Look(ref proposee, "proposee");
        Scribe_Values.Look(ref inSignal, "inSignal");
    }

    public override void AssignDebugData()
    {
        base.AssignDebugData();
        inSignal = "DebugSignal" + Rand.Int;
    }
}
