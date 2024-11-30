using RimWorld;
using Verse;

namespace MSS_Gen.Quests;

public class QuestPart_MarryStay : QuestPartActivable
{
    public Pawn asker;
    public Pawn proposee;

    public bool CeremonyStarted = false;

    public int NextCheck = 600;

    protected override void Enable(SignalArgs receivedArgs)
    {
        base.Enable(receivedArgs);

        proposee.SetFaction(asker.Faction);

        proposee.relations.AddDirectRelation(PawnRelationDefOf.Fiance, asker);
        asker.relations.AddDirectRelation(PawnRelationDefOf.Fiance, proposee);

        if (proposee.Map.lordsStarter.TryStartMarriageCeremony(proposee, asker))
        {
            CeremonyStarted = true;
        }
    }

    public override void QuestPartTick()
    {
        if (Find.TickManager.TicksGame < NextCheck)
            return;

        if (!CeremonyStarted)
        {
            if (proposee.Map.lordsStarter.TryStartMarriageCeremony(proposee, asker))
            {
                CeremonyStarted = true;
            }
            NextCheck = Find.TickManager.TicksGame + 600;
        }else if (proposee.GetSpouses(false).Contains(asker))
        {
            Complete();
            Find.LetterStack.ReceiveLetter("MSS_Gen_QuestPart_MarryStayEndLetterLabel".Translate(proposee.NameShortColored), "MSS_Gen_QuestPart_MarryStayEndLetterText".Translate(proposee.NameFullColored, asker.NameFullColored, asker.Faction.NameColored), LetterDefOf.PositiveEvent, (LookTargets) proposee, quest: this.quest, playSound: true);
        }

        NextCheck = Find.TickManager.TicksGame + 600;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref asker, "asker");
        Scribe_References.Look(ref proposee, "proposee");
        Scribe_Values.Look(ref CeremonyStarted, "CeremonyStarted");
    }
}
