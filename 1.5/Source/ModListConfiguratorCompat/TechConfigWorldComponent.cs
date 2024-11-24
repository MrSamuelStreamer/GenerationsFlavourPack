using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using HarmonyLib;
using ModlistConfigurator;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MSS_Gen.ModListConfiguratorCompat;

public class TechConfigWorldComponent(World world) : WorldComponent(world), ISignalReceiver
{
    private SettingsImporter Importer = new();

    public Lazy<FieldInfo> PresetsField = new Lazy<FieldInfo>(()=>AccessTools.Field(typeof(SettingsImporter), "Presets"));

    public DirectoryInfo presetLocation => ModListConfiguratorCompat_Mod.mod.Content.ModMetaData.RootDir.GetDirectories().FirstOrDefault(dir => dir.Name == "Settings");
    public void LoadPresets()
    {
        Dictionary<string, Preset> Presets = (Dictionary<string, Preset>)PresetsField.Value.GetValue(Importer);

        if(Presets == null) return;
        if (Presets.Count > 0) return;

        foreach (TechLevelConfigDef presetDef in DefDatabase<TechLevelConfigDef>.AllDefsListForReading)
        {
            var presetDir = presetLocation.GetDirectories().FirstOrDefault(dir => string.Equals(dir.Name, presetDef.presetPath, StringComparison.CurrentCultureIgnoreCase));
            if (presetDir == null || !presetDir.Exists)
            {
                ModLog.Warn($"Could not find preset location {presetDef.presetPath} for preset {presetDef.defName}");
                continue;
            }

            Presets.Add(presetDef.defName, new Preset(presetDef.defName, presetDef.label, presetDef.version, presetDir));
        }
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        Find.SignalManager.RegisterReceiver(this);
        LoadPresets();
    }

    public void Notify_SignalReceived(Signal signal)
    {
        if (signal.tag == "MSS_Gen_TechLevelChanged")
        {
            TechLevel newLevel = (TechLevel)signal.args.GetArg(0).arg;
            ModLog.Log($"{signal.ToString()} : {newLevel} | {signal.args.GetArg(1).ToString()}");

            TechLevelConfigDef tlcd = DefDatabase<TechLevelConfigDef>.AllDefsListForReading.FirstOrDefault(tlcd => tlcd.techLevel == newLevel);

            MergeSettings(tlcd.defName, newLevel.ToString());
        }
    }

    private void MergeSettings(string presetName, string levelName)
    {
        List<string> modsToImport = Importer.ModsToImport(presetName);

        if (modsToImport.Count == 0)
        {
            Find.WindowStack.Add(new Dialog_MessageBox("All mods configs are up-to-date, nothing to import"));
        }
        else
        {
            Find.WindowStack.Add(new Dialog_MessageBox(
                "MSS_Gen_Tech_Level_Advancing".Translate(levelName, string.Join("\r\n", modsToImport)),
                buttonAAction:
                () =>
                {
                    Importer.MergeSettings(presetName);
                    Find.WindowStack.Add(new Dialog_MessageBox(
                        "MSS_Gen_Tech_Level_Advancing_Restart".Translate(presetName)));
                }, buttonBText: "Cancel"));
        }
    }
}
