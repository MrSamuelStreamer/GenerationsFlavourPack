using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using VFETribals;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(RitualOutcomeEffectWorker_TribalGathering))]
public static class RitualOutcomeEffectWorker_TribalGathering_Patch
{
    [HarmonyPatch(nameof(RitualOutcomeEffectWorker_TribalGathering.Apply))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        bool first = false;
        bool second = false;
        bool third = false;

        foreach (CodeInstruction instruction in instructions)
        {
            if (!first && instruction.opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float) (instruction.operand), 0.5f))
            {
                first = true;
                instruction.operand = 0f;
            }else if (!second && instruction.opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float) (instruction.operand), 0.25f))
            {
                second = true;
                instruction.operand = 0f;
            }else if (!third && instruction.opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float) (instruction.operand), 0.5f))
            {
                third = true;
                instruction.operand = 0f;
            }

            yield return instruction;
        }
    }
}
