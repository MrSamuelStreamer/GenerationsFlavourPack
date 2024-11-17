using System.Collections.Generic;
using ModlistConfigurator;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MSS_Gen.ModListConfiguratorCompat;

public class TechConfigWorldComponent(World world) : WorldComponent(world), ISignalReceiver
{
    private SettingsImporter Importer = new();

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        Find.SignalManager.RegisterReceiver(this);
    }

    public void Notify_SignalReceived(Signal signal)
    {
        if (signal.tag == "MSS_Gen_TechLevelChanged")
        {
            TechLevel newLevel = (TechLevel)signal.args.GetArg(0).arg;
            ModLog.Log($"{signal.ToString()} : {newLevel} | {signal.args.GetArg(1).ToString()}");

            TechLevelConfigDef tlcd = DefDatabase<TechLevelConfigDef>.AllDefsListForReading.FirstOrDefault(tlcd => tlcd.techLevel == newLevel);

            if (tlcd != null && tlcd.modlistPreset != null)
            {
                MergeSettings(tlcd.defName);
            }
        }
    }

    private void MergeSettings(string presetName)
    {
        List<string> modsToImport = Importer.ModsToImport(presetName);

        if (modsToImport.Count == 0)
        {
            Find.WindowStack.Add(new Dialog_MessageBox("All mods configs are up-to-date, nothing to import"));
        }
        else
        {
            Find.WindowStack.Add(new Dialog_MessageBox(
                $"This will merge the settings for the following mods and will restart Rimworld to apply the configs. Merging allows you to preserve any user-settings that do not conflict with the preset. Do you want to continue?\r\n\r\n{string.Join("\r\n", modsToImport)}",
                buttonAAction:
                () =>
                {
                    Importer.MergeSettings(presetName);
                    Find.WindowStack.Add(new Dialog_MessageBox(
                        $"Config from \"{presetName}\" Preset have been imported. Restarting Rimworld to apply the configs",
                        buttonAAction: ModsConfig.RestartFromChangedMods));
                }, buttonBText: "Cancel"));
        }
    }
}
