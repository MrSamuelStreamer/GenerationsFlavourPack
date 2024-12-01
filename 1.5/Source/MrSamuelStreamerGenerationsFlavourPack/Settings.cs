using System.Collections.Generic;
using ModlistConfigurator;
using UnityEngine;
using Verse;

namespace MSS_Gen;

public class Settings : ModSettings
{
    public Vector2 scrollPosition = Vector2.zero;

    public int ReformationPointsPerYear = 1;
    public int ReformationPointsPerTechLevel = 5;
    public int ReformationPointsPerBaby = 1;
    public int ReformationPointsPerDefeatedFaction = 5;

    public void DoWindowContents(Rect wrect)
    {
        float scrollViewHeigh = 24f+12f+12f+12f+12f+24f+24f+24f+24f+12f+24f+24f+12f+24f+24f;

        List<TechLevelConfigDef> defs = DefDatabase<TechLevelConfigDef>.AllDefsListForReading;

        scrollViewHeigh += 30f*defs.Count;

        Rect viewRect = new Rect(0f, 0f, wrect.width- 20, scrollViewHeigh);
        scrollPosition = GUI.BeginScrollView(new Rect(0, 50, wrect.width, wrect.height - 50), scrollPosition, viewRect);

        Listing_Standard options = new Listing_Standard();

        options.Begin(viewRect);

        try
        {
            if (ModLister.GetActiveModWithIdentifier("garethp.modlistconfigurator") != null)
            {
                options.Label("MSS_Gen_Settings_ForceLoadConfig".Translate());

                foreach (TechLevelConfigDef techLevelConfigDef in defs)
                {
                    options.GapLine();
                    if (options.ButtonText(techLevelConfigDef.presetLabel))
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox(
                            "This will override your settings with this preset. You'll need to restart Rimworld after applying for all configs to be applied.",
                            buttonAAction:
                            () =>
                            {
                                new SettingsImporter().OverwriteSettings(techLevelConfigDef.defName);
                                Find.WindowStack.Add(new Dialog_MessageBox(
                                    "MSS_Gen_Tech_Level_Advancing_Restart".Translate(techLevelConfigDef.presetLabel)));
                            }, buttonBText: "Cancel"));
                    }
                }
            }
            options.Gap();

            options.Label("MSS_Gen_Settings_ReformationPointsPerYear".Translate(ReformationPointsPerYear));
            options.IntAdjuster(ref ReformationPointsPerYear, 1, 0);

            options.Gap();

            options.Label("MSS_Gen_Settings_ReformationPointsPerTechLevel".Translate(ReformationPointsPerTechLevel));
            options.IntAdjuster(ref ReformationPointsPerTechLevel, 1, 0);

            options.Gap();

            options.Label("MSS_Gen_Settings_ReformationPointsPerBaby".Translate(ReformationPointsPerBaby));
            options.IntAdjuster(ref ReformationPointsPerBaby, 1, 0);

            options.Gap();

            options.Label("MSS_Gen_Settings_ReformationPointsPerDefeatedFaction".Translate(ReformationPointsPerDefeatedFaction));
            options.IntAdjuster(ref ReformationPointsPerDefeatedFaction, 1, 0);

            options.Gap();
        }
        finally
        {
            options.End();
            GUI.EndScrollView();
        }
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref ReformationPointsPerYear, "ReformationPointsPerYear", 1);
        Scribe_Values.Look(ref ReformationPointsPerTechLevel, "ReformationPointsPerYear", 5);
        Scribe_Values.Look(ref ReformationPointsPerBaby, "ReformationPointsPerBaby", 1);
        Scribe_Values.Look(ref ReformationPointsPerBaby, "ReformationPointsPerDefeatedFaction", 5);
    }
}
