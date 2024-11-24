using HarmonyLib;
using RimWorld;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(LordJob_Ritual_ChildBirth))]
public static class LordJob_Ritual_ChildBirth_Patch
{
    [HarmonyPatch("RitualFinished")]
    [HarmonyPostfix]
    public static void RitualFinished_Patch()
    {
        Find.SignalManager.SendSignal(new Signal(Signals.MSS_Gen_BabyAddedToFaction));
    }
}
