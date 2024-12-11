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

public class TechConfigWorldComponent(World world) : WorldComponent(world), ISignalReceiver
{
    public static Lazy<List<ModContentPack>> RunningMods = new(() => LoadedModManager.RunningMods.ToList());

    public bool Loaded = false;

    public TechLevel LatestTechLevel = TechLevel.Undefined;

    public Lazy<FieldInfo> PresetsField = new Lazy<FieldInfo>(()=>AccessTools.Field(typeof(SettingsImporter), "Presets"));

    public DirectoryInfo presetLocation => MSS_GenMod.mod.Content.ModMetaData.RootDir.GetDirectories().FirstOrDefault(dir => dir.Name == "Settings");
    public void LoadPresets()
    {
        Dictionary<string, Preset> Presets = (Dictionary<string, Preset>)PresetsField.Value.GetValue(new SettingsImporter());

        if(Presets == null) return;
        if (Presets.Count > 0) return;

        foreach (TechLevelConfigDef presetDef in DefDatabase<TechLevelConfigDef>.AllDefsListForReading)
        {
            var presetDir = presetLocation.GetDirectories().FirstOrDefault(dir => string.Equals(dir.Name, presetDef.presetPath, StringComparison.CurrentCultureIgnoreCase));
            if (presetDir is not { Exists: true })
            {
                ModLog.Warn($"Could not find preset location {presetDef.presetPath} for preset {presetDef.defName}");
                continue;
            }

            Presets.Add(presetDef.defName, new Preset(presetDef.defName, presetDef.label, presetDef.version, presetDir));
        }

        Loaded = true;;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref LatestTechLevel, "LatestTechLevel", TechLevel.Undefined);
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        Find.SignalManager.RegisterReceiver(this);
    }

    public bool MeetsRequirements(TechLevel techLevelToCheck, TechLevelConfigDef def)
    {
        if (def.techLevel != techLevelToCheck) return false;
        if (def.modsMustExists.NullOrEmpty()) return true;
        return def.modsMustExists.All(modPackageId=> RunningMods.Value.Any(runningMod => runningMod.PackageId == modPackageId));
    }

    public void Notify_SignalReceived(Signal signal)
    {
        if (signal.tag == Signals.MSS_Gen_TechLevelChanged)
        {
            if (!Loaded)
            {
                LoadPresets();
            }

            TechLevel newLevel = (TechLevel)signal.args.GetArg("newTechLevel").arg;

            if(newLevel <= LatestTechLevel) return;
            LatestTechLevel = newLevel;

            ModLog.Debug($"TechConfigWorldComponent got change to {newLevel} from {signal.args.GetArg("oldTechLevel").ToString()}");

            if (Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.Fluid)
            {
                if (!Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.development.TryAddDevelopmentPoints(MSS_GenMod.settings.ReformationPointsPerTechLevel))
                {
                    ModLog.Debug("Couldn't add reformation points");
                }
                else
                {
                    Messages.Message("MSS_Gen_TechLeve".Translate(MSS_GenMod.settings.ReformationPointsPerTechLevel), MessageTypeDefOf.PositiveEvent, false);
                }
            }

            List<TechLevelConfigDef> defs = DefDatabase<TechLevelConfigDef>.AllDefsListForReading;

            TechLevelConfigDef tlcd = defs.FirstOrDefault(tlcd => MeetsRequirements(newLevel, tlcd));

            if (tlcd is not null)
            {
                ModLog.Debug($"TechConfigWorldComponent found TechLevelConfigDef for {newLevel} - {tlcd.ToString()}");
                OverwriteSettings(tlcd.defName, newLevel.ToString());
            }
            else
            {
                ModLog.Debug($"TechConfigWorldComponent found no relevant TechLevelConfigDef for {newLevel}");
            }
        }
    }

    private void OverwriteSettings(string presetDefName, string levelName)
    {
        Find.WindowStack.Add(new Dialog_MessageBox(
            "MSS_Gen_Tech_Level_Advancing".Translate(levelName),
            buttonADestructive: true,
            buttonAAction:
            () =>
            {
                SettingsImporter importer = new SettingsImporter();
                importer.OverwriteSettings(presetDefName);
                Find.WindowStack.Add(new Dialog_MessageBox(
                    "MSS_Gen_Tech_Level_Advancing_Restart".Translate(presetDefName)));
            }, buttonBText: "Cancel", layer: WindowLayer.Super));
    }
}
