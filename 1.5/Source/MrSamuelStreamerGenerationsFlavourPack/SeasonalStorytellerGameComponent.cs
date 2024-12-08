using RimWorld;
using Verse;

namespace MSS_Gen;

public class SeasonalStorytellerGameComponent: GameComponent
{
    public SeasonalStorytellerGameComponent(Game game)
    {

    }
    public SeasonalStorytellerGameComponent()
    {

    }
    public bool _Active => Santa != null;
    public bool Active => _Active && MSS_GenMod.settings.SeasonalStoryteller;

    public StorytellerDef Santa;
    public StorytellerDef OldStoryteller;

    public int NextCheck = 3600;

    public override void FinalizeInit()
    {
        base.FinalizeInit();

        Santa = DefDatabase<StorytellerDef>.AllDefsListForReading.FirstOrDefault(def => def.defName == "VCE_SantaSeasonal");
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref OldStoryteller, "OldStoryteller");
    }

    public override void GameComponentTick()
    {
        base.GameComponentTick();
        if(!Active) return;

        if(NextCheck > Find.TickManager.TicksAbs) return;
        NextCheck = Find.TickManager.TicksAbs + 3600;

        Quadrum quadrum = GenDate.Quadrum(Find.TickManager.TicksAbs, 0);

        if (quadrum == Quadrum.Decembary && Current.Game.storyteller.def != Santa)
        {
            OldStoryteller = Current.Game.storyteller.def;
            Current.Game.storyteller.def = Santa;

            Current.Game.storyteller.Notify_DefChanged();
        }else if (quadrum != Quadrum.Decembary && Current.Game.storyteller.def == Santa)
        {
            Current.Game.storyteller.def = OldStoryteller;

            Current.Game.storyteller.Notify_DefChanged();
        }
    }
}
