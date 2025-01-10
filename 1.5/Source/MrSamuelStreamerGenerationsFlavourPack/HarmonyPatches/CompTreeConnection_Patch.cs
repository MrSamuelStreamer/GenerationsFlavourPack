using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(CompTreeConnection))]
public static class CompTreeConnection_Patch
{
    [HarmonyPatch("TearConnection")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> ReplaceConstant(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == 1800000)
            {
                // Replace the operand with 300000
                instruction.operand = 300000;
            }

            yield return instruction;
        }
    }

}
