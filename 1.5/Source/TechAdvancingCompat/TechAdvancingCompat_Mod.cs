using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MSS_Gen.TechAdvancingCompat;

public class TechAdvancingCompat_Mod : Mod
{
    public static Type TA_ResearchManagerCls = typeof(TechAdvancing.Settings).Assembly.GetType("TechAdvancing.TA_ResearchManager");
    public static Lazy<MethodInfo> Postfix = new(()=>AccessTools.Method(TA_ResearchManagerCls, "Postfix"));
    public static Lazy<MethodInfo> RaiseSignalInfo = new(() => AccessTools.Method(typeof(TechAdvancingCompat_Mod), nameof(RaiseSignal)));
    public static Lazy<MethodInfo> ReceiveLetterInfo = new(() => AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), [typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(string), typeof(int), typeof(bool)]));

    public TechAdvancingCompat_Mod(ModContentPack content) : base(content)
    {
#if DEBUG
        ModLog.Log("MSS_Gen.TechAdvancingCompat");
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new Harmony("MSS_Gen.TechAdvancingCompat.main");
        harmony.PatchAll();

        MethodInfo TA_ResearchManager_Postfix_Transpiler_MethodInfo = typeof(TechAdvancingCompat_Mod).GetMethod("TA_ResearchManager_Postfix_Transpiler");
        harmony.Patch(Postfix.Value, null, null, new HarmonyMethod(TA_ResearchManager_Postfix_Transpiler_MethodInfo), null);
    }

    public static void RaiseSignal(TechLevel newLevel)
    {
        ModLog.Log("MSS_Gen.TechAdvancingCompat");
        if (Find.FactionManager.OfPlayer.def.techLevel != newLevel)
            Find.SignalManager.SendSignal(new Signal(Signals.MSS_Gen_TechLevelChanged, new NamedArgument(newLevel, "newTechLevel"), new NamedArgument(Find.FactionManager.OfPlayer.def.techLevel, "oldTechLevel")));
    }

    public static IEnumerable<CodeInstruction> TA_ResearchManager_Postfix_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            yield return instruction;
            if (instruction.Calls(ReceiveLetterInfo.Value))
            {
                yield return new CodeInstruction(OpCodes.Ldloc_0); // Assuming your newTechLevel is in loc.0
                yield return new CodeInstruction(OpCodes.Call, RaiseSignalInfo.Value);
            }
        }
    }
}
