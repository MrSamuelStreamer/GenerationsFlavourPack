using RimWorld.QuestGen;
using Verse;

namespace MSS_Gen.Quests;

public class QuestNode_MarryStay : QuestNode
{
    public SlateRef<string> inSignal;
    public SlateRef<string> outSignalComplete;
    public SlateRef<object> proposee;
    public SlateRef<object> asker;

    protected override void RunInt()
    {
        Set(QuestGen.slate);
    }

    protected override bool TestRunInt(Slate slate) => true;

    public void Set(Slate slate)
    {
        Pawn p = proposee.GetValue(slate) as Pawn;
        Pawn a = asker.GetValue(slate) as Pawn;

        if (a == null || p == null) return;

        QuestPart_MarryStay part = new();
        part.inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID(this.inSignal.GetValue(slate)) ?? QuestGen.slate.Get<string>("inSignal");
        part.outSignalsCompleted.Add(QuestGenUtility.HardcodedSignalWithQuestID(this.outSignalComplete.GetValue(slate)) ?? QuestGen.slate.Get<string>("outSignalComplete"));
        part.proposee = p;
        part.asker = a;
        QuestGen.quest.AddPart(part);
    }
}
