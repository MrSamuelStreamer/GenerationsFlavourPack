﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using MSS_Gen.Comps;
using RimWorld;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(CompQuality))]
public static class CompQuality_Patch
{
    public static ReformationPointsWorldComponent Comp => Find.World.GetComponent<ReformationPointsWorldComponent>();

    public static HashSet<QualityCategory> validCategories = new()
    {
        QualityCategory.Good,
        QualityCategory.Excellent,
        QualityCategory.Masterwork,
        QualityCategory.Legendary
    };

    [HarmonyPatch(nameof(CompQuality.SetQuality))]
    [HarmonyPostfix]
    public static void SetQualityPostfix(CompQuality __instance, QualityCategory q, ArtGenerationContext? source)
    {
        if(Find.FactionManager == null || Find.FactionManager.OfPlayer == null) return;
        if(__instance.parent is not { Faction.IsPlayer: true }) return;
        if(!validCategories.Contains(q)) return;
        if (!__instance.parent.TryGetComp(out CompLegendaryTracker compLegendaryTracker))
        {
            compLegendaryTracker = Activator.CreateInstance(typeof(CompLegendaryTracker)) as CompLegendaryTracker;
            compLegendaryTracker!.parent = __instance.parent;
            __instance.parent.AllComps.Add(compLegendaryTracker);
            compLegendaryTracker.Initialize(new CompProperties());
        }

        compLegendaryTracker.BecameLegendaryAtTechLevel = Find.FactionManager.OfPlayer == null ? TechLevel.Undefined : Find.FactionManager.OfPlayer.def.techLevel;

        if(q == QualityCategory.Legendary && Comp.AddPoints(MSS_GenMod.settings.ReformationPointsForLegendary))
            Messages.Message("MSS_Gen_ReformationPointsForLegendary".Translate(__instance.parent.LabelCap, MSS_GenMod.settings.ReformationPointsForLegendary), MessageTypeDefOf.PositiveEvent, true);
    }
}
