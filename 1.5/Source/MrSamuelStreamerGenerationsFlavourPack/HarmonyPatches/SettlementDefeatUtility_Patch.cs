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

    [HarmonyPatch(nameof(SettlementDefeatUtility.CheckDefeated))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> CheckDefeated_Patch(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        var factionDefeatedField = AccessTools.Field(typeof(Faction), "defeated");
        var factionField = AccessTools.Field(typeof(Settlement), "factionInt");
        var factionDefeatedSignalMethod = AccessTools.Method(
            typeof(SettlementDefeatUtility_Patch), nameof(FactionDefeatedSignal));

        for (int i = 0; i < codes.Count; i++)
        {
            // Look for the line factionBase.Faction.defeated = true;
            if (codes[i].opcode == OpCodes.Stfld && codes[i].operand == factionDefeatedField)
            {
                // Insert after the found instruction
                codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0)); // Load `factionBase`
                codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldfld, factionField)); // Load `factionBase.Faction`
                codes.Insert(i + 3, new CodeInstruction(OpCodes.Call, factionDefeatedSignalMethod)); // Call method
                break;
            }
        }
        return codes.AsEnumerable();
    }

    public static void FactionDefeatedSignal(Faction faction)
    {
        Find.SignalManager.SendSignal(new Signal(Signals.MSS_Gen_FactionDefeated, faction));
    }

}
