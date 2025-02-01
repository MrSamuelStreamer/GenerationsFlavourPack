using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(SettlementDefeatUtility))]
public static class SettlementDefeatUtility_Patch
{
    public static void CheckDefeatedPostfix(Settlement factionBase)
    {
        Find.SignalManager.SendSignal(new Signal(Signals.MSS_Gen_SettlementDefeated, factionBase.Name, factionBase.Faction));
    }

    [HarmonyPatch(nameof(SettlementDefeatUtility.CheckDefeated))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> CheckDefeated_Patch(IEnumerable<CodeInstruction> instructions)
    {
        FieldInfo factionDefeatedField = AccessTools.Field(typeof(Faction), "defeated");
        FieldInfo factionField = AccessTools.Field(typeof(Settlement), "factionInt");
        MethodInfo factionDefeatedSignalMethod = AccessTools.Method(
            typeof(SettlementDefeatUtility_Patch), nameof(FactionDefeatedSignal));
        MethodInfo CheckDefeatedPostfixMethod = AccessTools.Method(
            typeof(SettlementDefeatUtility_Patch), nameof(CheckDefeatedPostfix));

        bool found = false;

        CodeInstruction prev = null;
        CodeInstruction prevPrev = null;


        foreach (CodeInstruction code in instructions)
        {
            if (prev != null && prevPrev != null && prev.opcode == OpCodes.Pop && prevPrev.opcode == OpCodes.Call && code.opcode == OpCodes.Ret)
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, CheckDefeatedPostfixMethod);
            }
            yield return code;

            // Look for the line factionBase.Faction.defeated = true;
            if (!found && code.opcode == OpCodes.Stfld && (FieldInfo)code.operand == factionDefeatedField)
            {
                found = true;
                // Insert after the found instruction
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, factionField);
                yield return new CodeInstruction(OpCodes.Call, factionDefeatedSignalMethod);
            }

            prevPrev = prev;
            prev = code;
        }


    }

    public static void FactionDefeatedSignal(Faction faction)
    {
        Find.SignalManager.SendSignal(new Signal(Signals.MSS_Gen_FactionDefeated, faction));
    }

}
