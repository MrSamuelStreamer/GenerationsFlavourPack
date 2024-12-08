using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(StorytellerUI))]
public static class StoryTellerUI_Patch
{
    [HarmonyPatch("DrawCustomLeft")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> DrawCustomLeftTranspiler(IEnumerable<CodeInstruction> instructionsEnumerable)
    {
        List<CodeInstruction> instructions = instructionsEnumerable.ToList();

        for (int i = 1; i < instructions.Count-1; i++)
        {
            if (instructions[i - 1].opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float)instructions[i - 1].operand, 1f) &&
                instructions[i].opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float)instructions[i].operand, 6f) &&
                instructions[i + 1].opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float)instructions[i + 1].operand, 1f))
            {
                instructions[i].operand = 32f;
            }
        }

        return instructions;
    }
}
