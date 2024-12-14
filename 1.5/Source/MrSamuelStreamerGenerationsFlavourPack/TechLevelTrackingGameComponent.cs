using RimWorld;
using Verse;
using VFETribals;

namespace MSS_Gen;

public class TechLevelTrackingGameComponent: GameComponent
{
    public TechLevel currentTechLevel = TechLevel.Undefined;


    public TechLevelTrackingGameComponent(): base(){}
    public TechLevelTrackingGameComponent(Game game): base(){}

    public override void FinalizeInit()
    {
        if(Find.FactionManager != null)
            currentTechLevel = Find.FactionManager.OfPlayer.def.techLevel;
    }

    public override void GameComponentTick()
    {
        base.GameComponentTick();

        if(Find.FactionManager == null) return;

        if (currentTechLevel == TechLevel.Undefined)
        {
            currentTechLevel = Find.FactionManager.OfPlayer.def.techLevel;
            return;
        }

        if (Find.FactionManager.OfPlayer.def.techLevel <= currentTechLevel)
        {
            return;
        }

        TechLevel oldLevel = currentTechLevel;
        TechLevel newLevel = Find.FactionManager.OfPlayer.def.techLevel;

        ModLog.Log($"TechLevel change detected from <color=green>{oldLevel}</color> to <color=green>{newLevel}</color>. Raising signal <color=green>{Signals.MSS_Gen_TechLevelChanged}</color>");

        currentTechLevel = newLevel;

        Find.SignalManager.SendSignal(new Signal(Signals.MSS_Gen_TechLevelChanged,
            new NamedArgument(newLevel, "newTechLevel"),
            new NamedArgument(oldLevel, "oldTechLevel")
        ));

        GameComponent_Tribals comp = Current.Game.GetComponent<GameComponent_Tribals>();

        EraAdvancementDef def = null;

        switch (newLevel)
        {
            case TechLevel.Neolithic:
                def = VFET_DefOf.VFET_FormTribe;
                break;
            case TechLevel.Medieval:
                def = VFET_DefOf.VFET_FormTown;
                break;
            case TechLevel.Industrial:
                def = VFET_DefOf.VFET_FormCity;
                break;
            case TechLevel.Spacer:
                def = VFET_DefOf.VFET_FormCollective;
                break;
            case TechLevel.Ultra:
                def = VFET_DefOf.VFET_FormNexus;
                break;
        }

        if (def == null)
        {
            ModLog.Warn($"EraAdvancementDef for {newLevel} not found");
            return;
        }
        LookTargets lookTargets = new(Find.Maps.Find(map => map.IsPlayerHome).PlayerPawnsForStoryteller);

        comp.AdvanceToEra(def);
        Find.LetterStack.ReceiveLetter(def.label, def.description, LetterDefOf.RitualOutcomePositive, lookTargets, null, null, null, null);


    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref currentTechLevel, "currentTechLevel", TechLevel.Undefined);
    }
}
