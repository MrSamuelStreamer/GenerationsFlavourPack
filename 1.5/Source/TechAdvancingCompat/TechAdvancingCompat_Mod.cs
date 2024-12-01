using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MSS_Gen.TechAdvancingCompat;

public class TechAdvancingCompat_Mod : Mod
{
    public TechAdvancingCompat_Mod(ModContentPack content) : base(content)
    {
#if DEBUG
        ModLog.Log("MSS_Gen.TechAdvancingCompat");
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new Harmony("MSS_Gen.TechAdvancingCompat.main");
        harmony.PatchAll();

        Type rulesCls = typeof(TechAdvancing.Settings).Assembly.GetType("TechAdvancing.Rules");
        MethodInfo GetNewTechLevel = AccessTools.Method(rulesCls, "GetNewTechLevel");

        MethodInfo GetNewTechLevel_Patch_MethodInfo = typeof(TechAdvancingCompat_Mod).GetMethod("GetNewTechLevel_Patch");
        harmony.Patch(GetNewTechLevel, null, new HarmonyMethod(GetNewTechLevel_Patch_MethodInfo), null, null);
    }

    public static void GetNewTechLevel_Patch(TechLevel __result)
    {
        if(Current.ProgramState != ProgramState.Playing) return;
        if(Find.World == null || Find.FactionManager == null || Find.FactionManager.OfPlayer == null) return;

        if (Find.FactionManager.OfPlayer.def.techLevel != __result)
            Find.SignalManager.SendSignal(new Signal(Signals.MSS_Gen_TechLevelChanged, new NamedArgument(__result, "newTechLevel"), new NamedArgument(Find.FactionManager.OfPlayer.def.techLevel, "oldTechLevel")));
    }
}
