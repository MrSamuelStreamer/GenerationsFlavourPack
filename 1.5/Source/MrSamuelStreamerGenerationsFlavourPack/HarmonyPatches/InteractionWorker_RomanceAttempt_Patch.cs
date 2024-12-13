using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(InteractionWorker_RomanceAttempt))]
public static class InteractionWorker_RomanceAttempt_Patch
{
    [HarmonyPatch(nameof(InteractionWorker_RomanceAttempt.Interacted))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> InteractedTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_I4 && (int) instruction.operand == 900000)
            {
                instruction.operand = GenDate.TicksPerDay;
            }
            yield return instruction;;
        }
    }
}
