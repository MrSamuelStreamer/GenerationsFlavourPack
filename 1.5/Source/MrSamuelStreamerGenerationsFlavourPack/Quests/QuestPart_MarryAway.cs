using RimWorld;
using Verse;

namespace MSS_Gen.Quests;

public class QuestPart_MarryAway : QuestPart
{
    public string inSignal;
    public Pawn asker;
    public Pawn proposee;
    public bool sendLetter;


    public override void Notify_QuestSignalReceived(Signal signal)
    {
        base.Notify_QuestSignalReceived(signal);
        if (signal.tag != inSignal)
            return;

        proposee.SetFaction(asker.Faction);
        MarriageCeremonyUtility.Married(asker, proposee);

        quest.End(QuestEndOutcome.Success, sendLetter, true);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref asker, "asker");
        Scribe_References.Look(ref proposee, "proposee");
        Scribe_Values.Look(ref inSignal, "inSignal");
        Scribe_Values.Look(ref sendLetter, "sendLetter");
    }

    public override void AssignDebugData()
    {
        base.AssignDebugData();
        inSignal = "DebugSignal" + Rand.Int;
    }
}
