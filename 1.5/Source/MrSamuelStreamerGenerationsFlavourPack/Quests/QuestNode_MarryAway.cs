using RimWorld.QuestGen;
using Verse;

namespace MSS_Gen.Quests;

public class QuestNode_MarryAway : QuestNode
{
    public SlateRef<string> inSignal;
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

        QuestPart_MarryAway part = new();
        part.inSignal = QuestGenUtility.HardcodedSignalWithQuestID(this.inSignal.GetValue(slate)) ?? QuestGen.slate.Get<string>("inSignal");
        part.proposee = p;
        part.asker = a;
        QuestGen.quest.AddPart(part);
    }
}
