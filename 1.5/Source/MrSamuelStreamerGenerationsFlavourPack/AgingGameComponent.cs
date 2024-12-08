using System.Linq;
using RimWorld;
using Verse;

namespace MSS_Gen;

public class AgingGameController: GameComponent
{
    public bool DefaultAgingHasBeenApplied = false;

    public AgingGameController(){}
    public AgingGameController(Game game){}

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref DefaultAgingHasBeenApplied, "DefaultAgingHasBeenApplied", false);
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        if (!DefaultAgingHasBeenApplied)
        {
            DefaultAgingHasBeenApplied = true;
            DifficultyDef customDef = DefDatabase<DifficultyDef>.AllDefs.First(def => def.isCustom);
            Current.Game.storyteller.difficultyDef = customDef;
            Current.Game.storyteller.difficulty.adultAgingRate = 16f;
            Current.Game.storyteller.difficulty.childAgingRate = 32f;
            Current.Game.storyteller.Notify_DefChanged();
        }
    }
}
