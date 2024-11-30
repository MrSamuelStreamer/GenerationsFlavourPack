using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace MSS_Gen.Quests;

public class QuestNode_GetUnmarriedPawn : QuestNode
{
    [NoTranslate] public SlateRef<string> storeAs;
    public SlateRef<bool> canGeneratePawn;
    public SlateRef<float?> hostileWeight;
    public SlateRef<float?> nonHostileWeight;
    public SlateRef<int> maxUsablePawnsToGenerate = 10;

    private IEnumerable<Pawn> ExistingUsablePawns()
    {
        return PawnsFinder.AllMapsWorldAndTemporary_Alive.Where(IsGoodPawn);
    }

    protected override bool TestRunInt(Slate slate)
    {
        if (slate.TryGet(storeAs.GetValue(slate), out Pawn pawn) && IsGoodPawn(pawn))
            return true;

        IEnumerable<Pawn> source = ExistingUsablePawns().ToList();
        if (source.Any())
        {
            slate.Set(storeAs.GetValue(slate), source.RandomElement());
            return true;
        }

        if (!TryFindFactionForPawnGeneration(slate, out Faction _))
            return false;

        return true;
    }

    private bool TryFindFactionForPawnGeneration(Slate slate, out Faction faction)
    {
        return Find.FactionManager.GetFactions(allowNonHumanlike: false).Where(x =>
                !x.def.permanentEnemy)
            .TryRandomElementByWeight(x => x.HostileTo(Faction.OfPlayer) ? hostileWeight.GetValue(slate) ?? 1f : nonHostileWeight.GetValue(slate) ?? 1f, out faction);
    }

    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        if (QuestGen.slate.TryGet(storeAs.GetValue(slate), out Pawn pawn) && IsGoodPawn(pawn))
            return;
        IEnumerable<Pawn> source = ExistingUsablePawns().ToList();
        int num = source.Count();
        Pawn var2 =
            !Rand.Chance(canGeneratePawn.GetValue(slate) ? Mathf.Clamp01((float) (1.0 - num / (double) maxUsablePawnsToGenerate.GetValue(slate))) : 0.0f) ||
            !TryFindFactionForPawnGeneration(slate, out Faction _)
                ? source.RandomElementByWeight(x =>
                    x.Faction != null && x.Faction.HostileTo(Faction.OfPlayer) ? hostileWeight.GetValue(slate) ?? 1f : nonHostileWeight.GetValue(slate) ?? 1f)
                : GeneratePawn(slate);
        if (var2.Faction is { Hidden: false })
            QuestGen.quest.AddPart(new QuestPart_InvolvedFactions { factions = { var2.Faction } });
        QuestGen.slate.Set(storeAs.GetValue(slate), var2);
    }

    private Pawn GeneratePawn(Slate slate, Faction faction = null)
    {
        PawnKindDef result = null;
        if (faction == null)
        {
            if (!TryFindFactionForPawnGeneration(slate, out faction))
                Log.Error("QuestNode_GetPawn tried generating pawn but couldn't find a proper faction for new pawn.");

            result = faction.RandomPawnKind();
        }

        result ??= DefDatabase<PawnKindDef>.AllDefsListForReading.Where(kind => kind.race.race.Humanlike).RandomElement();

        Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(result, faction, forceGenerateNewPawn: true, fixedTitle: null));
        Find.WorldPawns.PassToWorld(pawn);

        if (pawn.royalty != null && pawn.royalty.AllTitlesForReading.Any())
            QuestGen.quest.AddPart(new QuestPart_Hyperlinks { pawns = { pawn } });
        return pawn;
    }

    private bool IsGoodPawn(Pawn pawn)
    {
        Faction faction = pawn.Faction;
        if (faction == null || !faction.def.humanlikeFaction || faction.defeated || faction.Hidden || faction.IsPlayer || pawn.IsPrisoner)
            return false;

        return !pawn.Faction.def.permanentEnemy && pawn.GetSpouses(false).NullOrEmpty();
    }
}
