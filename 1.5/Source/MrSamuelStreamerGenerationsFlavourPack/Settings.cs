using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MSS_Gen;

public class Settings : ModSettings
{
    public Vector2 scrollPosition = Vector2.zero;

    public void DoWindowContents(Rect wrect)
    {
        float scrollViewHeigh = 24f+12f+12f;

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
                                MSS_GenMod.Importer.OverwriteSettings(techLevelConfigDef.defName);
                                Find.WindowStack.Add(new Dialog_MessageBox(
                                    "MSS_Gen_Tech_Level_Advancing_Restart".Translate(techLevelConfigDef.presetLabel)));
                            }, buttonBText: "Cancel"));
                    }
                }
            }
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
    }
}
