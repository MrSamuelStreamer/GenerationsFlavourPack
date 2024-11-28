using System.Collections.Generic;
using RimWorld.QuestGen;

namespace MSS_Gen.Quests;

public class QuestNode_SingleToList : QuestNode
{
    public SlateRef<object> single;
    public SlateRef<string> list;

    protected override void RunInt()
    {
        Set(QuestGen.slate);
    }

    protected override bool TestRunInt(Slate slate)
    {
        Set(slate);
        return true;
    }

    public void Set(Slate slate)
    {
        object s = single.GetValue(slate);
        slate.Set<List<object>>(list.GetValue(slate), [s]);
    }
}
