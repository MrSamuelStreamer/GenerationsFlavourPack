using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using ModlistConfigurator;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MSS_Gen;

public class TechConfigWorldComponent(World world) : WorldComponent(world)
{
    public static Lazy<List<ModContentPack>> RunningMods = new(() => LoadedModManager.RunningMods.ToList());

    public bool Loaded = false;

    public Lazy<FieldInfo> PresetsField = new Lazy<FieldInfo>(()=>AccessTools.Field(typeof(SettingsImporter), "Presets"));

    public DirectoryInfo presetLocation => MSS_GenMod.mod.Content.ModMetaData.RootDir.GetDirectories().FirstOrDefault(dir => dir.Name == "Settings");

    public bool HaveChangedSettingsThisTick = false;

    public override void WorldComponentTick()
    {
        base.WorldComponentTick();
        HaveChangedSettingsThisTick = false;
    }

    public void LoadPresets()
    {
        Dictionary<string, Preset> Presets = (Dictionary<string, Preset>)PresetsField.Value.GetValue(new SettingsImporter());

        if(Presets == null) return;
        if (Presets.Count > 0) return;

        foreach (TechLevelConfigDef presetDef in DefDatabase<TechLevelConfigDef>.AllDefsListForReading)
        {
            DirectoryInfo presetDir = presetLocation.GetDirectories().FirstOrDefault(dir => string.Equals(dir.Name, presetDef.presetPath, StringComparison.CurrentCultureIgnoreCase));
            if (presetDir is not { Exists: true })
            {
                ModLog.Warn($"Could not find preset location {presetDef.presetPath} for preset {presetDef.defName}");
                continue;
            }

            Presets.Add(presetDef.defName, new Preset(presetDef.defName, presetDef.label, presetDef.version, presetDir));
        }

        Loaded = true;;
    }

    public bool MeetsRequirements(TechLevel techLevelToCheck, TechLevelConfigDef def)
    {
        if (def.techLevel != techLevelToCheck) return false;
        if (def.modsMustExists.NullOrEmpty()) return true;
        return def.modsMustExists.All(modPackageId=> RunningMods.Value.Any(runningMod => runningMod.PackageId == modPackageId));
    }

    public void SetNewConfigs(TechLevel techLevel)
    {
        if(HaveChangedSettingsThisTick) return;

        HaveChangedSettingsThisTick = true;
        
        Find.SignalManager.SendSignal(new Signal(Signals.MSS_Gen_TechLevelChanged,
            new NamedArgument(techLevel, "newTechLevel")
        ));

        if (!Loaded)
        {
            LoadPresets();
        }

        if (Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.Fluid)
        {
            if (!Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.development.TryAddDevelopmentPoints(MSS_GenMod.settings.ReformationPointsPerTechLevel))
            {
                ModLog.Log("Couldn't add reformation points");
            }
            else
            {
                Messages.Message("MSS_Gen_TechLeve".Translate(MSS_GenMod.settings.ReformationPointsPerTechLevel), MessageTypeDefOf.PositiveEvent, false);
            }
        }

        List<TechLevelConfigDef> defs = DefDatabase<TechLevelConfigDef>.AllDefsListForReading;

        TechLevelConfigDef tlcd = defs.FirstOrDefault(tlcd => MeetsRequirements(techLevel, tlcd));

        if (tlcd is not null)
        {
            ModLog.Log($"TechConfigWorldComponent found TechLevelConfigDef for {techLevel} - {tlcd}");
            MergeSettings(tlcd.defName, techLevel.ToString());
        }
        else
        {
            ModLog.Log($"TechConfigWorldComponent found no relevant TechLevelConfigDef for {techLevel}");
        }
    }

    private void MergeSettings(string presetDefName, string levelName)
    {
        SettingsImporter importer = new();
        List<string> modsToImport = importer.ModsToImport(presetDefName);

        Find.WindowStack.Add(new Dialog_MessageBox(
            "MSS_Gen_Tech_Level_Advancing".Translate(levelName, string.Join("\r\n", modsToImport)),
            buttonADestructive: true,
            buttonAAction:
            () =>
            {
                importer.MergeSettings(presetDefName);
                Find.WindowStack.Add(new Dialog_MessageBox(
                    "MSS_Gen_Tech_Level_Advancing_Restart".Translate(presetDefName)));
            }, buttonBText: "Cancel", layer: WindowLayer.Super));
    }
}
