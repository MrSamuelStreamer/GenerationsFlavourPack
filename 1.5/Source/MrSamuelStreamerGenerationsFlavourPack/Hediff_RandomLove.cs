using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MSS_Gen;

public class Hediff_RandomLove: Hediff_PsychicLove
{
    public override void PostMake()
    {
        if (target == null && pawn != null)
        {
            IEnumerable<Pawn> rels = pawn.GetLoveRelations(false).AsEnumerable().Select(dpr=>dpr.otherPawn);

            Pawn selectedTarget = pawn.Map.mapPawns.AllHumanlike.Except(rels).Where(p => p.Faction == pawn.Faction && p.ageTracker.Adult).RandomElement();

            if (selectedTarget != null)
            {
                target = selectedTarget;
            }
        }
    }
}
