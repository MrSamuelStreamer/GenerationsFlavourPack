using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace MSS_Gen.Quests;

public class QuestNode_GetPlayerPawn : QuestNode
{
    public SlateRef<string> storeAs;
    public bool CannotBeMarried = false;

    public IEnumerable<Pawn> GetCandidates()
    {

        IEnumerable<Pawn> candidates = Find.Maps.Where(map=>map.IsPlayerHome).SelectMany(map=>map.mapPawns.AllHumanlike).Where(pawn => pawn.Faction.IsPlayer && pawn.IsColonist && pawn.IsColonistPlayerControlled);
        if (CannotBeMarried)
        {
            candidates = candidates.Where(pawn => pawn.GetSpouses(false).NullOrEmpty());
        }

        var a = candidates.ToList();

        return a;
    }

    protected override void RunInt()
    {
        QuestGen.slate.Set(storeAs.GetValue(QuestGen.slate), GetCandidates().RandomElement());
    }

    protected override bool TestRunInt(Slate slate)
    {
        return GetCandidates().Any();
    }
}
