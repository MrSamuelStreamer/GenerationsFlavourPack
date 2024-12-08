using System;
using HarmonyLib;
using MSS_Gen.Comps;
using RimWorld;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(CompQuality))]
public static class CompQuality_Patch
{
    [HarmonyPatch(nameof(CompQuality.SetQuality))]
    [HarmonyPostfix]
    public static void SetQualityPostfix(CompQuality __instance, QualityCategory q)
    {
        if(q != QualityCategory.Legendary) return;
        if (!__instance.parent.TryGetComp(out CompLegendaryTracker compLegendaryTracker))
        {
            compLegendaryTracker = Activator.CreateInstance(typeof(CompLegendaryTracker)) as CompLegendaryTracker;
            compLegendaryTracker!.parent = __instance.parent;
            __instance.parent.AllComps.Add(compLegendaryTracker);
            compLegendaryTracker.Initialize(new CompProperties());
        }

        compLegendaryTracker.BecameLegendaryAtTechLevel = Find.World.GetComponent<TechConfigWorldComponent>().LatestTechLevel;
    }
}
