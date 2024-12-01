using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(SanguophageUtility))]
public static class SanguophageUtility_Patch
{
    public static Lazy<FieldInfo> XPLossPercentFromDeathrestRange = new Lazy<FieldInfo>(()=> AccessTools.Field(typeof(SanguophageUtility), "XPLossPercentFromDeathrestRange"));

    [HarmonyPatch(nameof(SanguophageUtility.DoXPLossFromDamage))]
    [HarmonyPrefix]
    public static bool DoXPLossFromDamagePostfix(Pawn pawn, ref TaggedString letterText)
    {
        Gene_VoidsEmbrace voidsEmbraceGene = pawn.genes?.GetFirstGeneOfType<Gene_VoidsEmbrace>();
        if(voidsEmbraceGene == null) return true;

        foreach (SkillRecord skill in pawn.skills.skills)
        {
            skill.Level = 0;
        }

        letterText += "\n\n" + "MSS_Gen_VoidsEmbraceSkillsReset".Translate(pawn.Named("PAWN"));

        letterText = letterText.Replace("deathless", "Void's Embrace");

        voidsEmbraceGene.lastSkillReductionTick = Find.TickManager.TicksGame;

        return false;
    }

}
