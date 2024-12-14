using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MSS_Gen.TechAdvancingCompat;

public class StoryTellerTechLevelWorldComponent(World world) : WorldComponent(world), ISignalReceiver
{
    public override void FinalizeInit()
    {
        base.FinalizeInit();
        Find.SignalManager.RegisterReceiver(this);
    }

    public void Notify_SignalReceived(Signal signal)
    {
        if(signal.tag != Signals.MSS_Gen_TechLevelChanged) return;

        TechLevel? techLevel = (TechLevel)signal.args.GetArg(0).arg;

        ModLog.Debug($"StoryTellerTechLevelWorldComponent got tech level change to {techLevel.ToString()}");

        StoryTellerForTechLevelDef newTeller = DefDatabase<StoryTellerForTechLevelDef>.AllDefsListForReading.FirstOrDefault(def => def.StoryTeller!=null && def.TechLevel == techLevel);

        if(newTeller == null) return;

        ModLog.Debug($"StoryTellerTechLevelWorldComponent changing storyteller to {newTeller.StoryTeller.LabelCap}");

        Current.Game.storyteller.def = newTeller.StoryTeller;
        Current.Game.storyteller.Notify_DefChanged();

        ModLog.Log($"Storyteller changed to {newTeller.StoryTeller.LabelCap}");
    }
}
