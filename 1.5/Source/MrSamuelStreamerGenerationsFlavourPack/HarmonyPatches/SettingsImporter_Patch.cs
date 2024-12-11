using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using HarmonyLib;
using ModlistConfigurator;
using Verse;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(SettingsImporter))]
public static class SettingsImporter_Patch
{
    public static Lazy<FieldInfo> Presets = new Lazy<FieldInfo>(()=>AccessTools.Field(typeof(SettingsImporter), "Presets"));
    private static string GetSettingsFilename(string modIdentifier, string modHandleName) => Path.Combine(
        GenFilePaths.ConfigFolderPath,
        GenText.SanitizeFilename($"Mod_{(object) modIdentifier}_{(object) modHandleName}.xml"));

    [HarmonyPatch(nameof(SettingsImporter.ShouldImport))]
    [HarmonyPrefix]
    public static bool ShouldImport(ref bool __result, string importFilePath, string modId, string modName)
    {
        if (modId == "2817607528" && modName.ToLower() == "ResourceDictionaryMod".ToLower())
        {
            // skip check for ResourceDictionaryMod
            __result = true;
            return false;
        }

        // fix a bug in ModlistConfigurator
        XmlElement importSettings = SettingsImporter.GetSettingsFromFile(importFilePath)?.DocumentElement;
        XmlElement currentSettings = SettingsImporter.GetSettingsFromFile(GetSettingsFilename(modId, modName))?.DocumentElement;

        if (importSettings is null) __result = false;
        else if (currentSettings is null) __result = true;
        else __result = !XmlUtils.NodesAreEqual(importSettings, currentSettings);

        return false;
    }

    [HarmonyPatch(nameof(SettingsImporter.MergeSettings))]
    [HarmonyPrefix]
    public static bool MergeSettings_Patch(SettingsImporter __instance, string presetName)
    {
        // fix a bug in ModlistConfigurator
        Dictionary<string, Preset> presets = (Dictionary<string, Preset>)Presets.Value.GetValue(__instance);

        if (!presets.ContainsKey(presetName))
            return false;

        foreach (FileInfo file in presets[presetName].SettingsDirectory.GetFiles())
        {
            Match match = Regex.Match(file.Name, "^Mod_(.*)_(.*).xml$");
            if (match.Success)
            {
                string fullName = file.FullName;
                string modIdentifier = match.Groups[1].Value;
                string modHandleName = match.Groups[2].Value;

                if (modIdentifier == "2817607528" && modHandleName.ToLower() == "ResourceDictionaryMod".ToLower())
                {
                    // Skip for ResourceDictionaryMod
                    File.Copy(fullName, GetSettingsFilename(modIdentifier, modHandleName));
                    continue;
                }

                XmlElement documentElement1 = SettingsImporter.GetSettingsFromFile(fullName)?.DocumentElement;
                XmlDocument settingsFromFile = SettingsImporter.GetSettingsFromFile(GetSettingsFilename(modIdentifier, modHandleName));
                XmlElement documentElement2 = settingsFromFile?.DocumentElement;

                if(documentElement1 is null || documentElement2 is null) continue;

                XmlNode node = XmlUtils.MergeNodes((XmlNode) documentElement1, (XmlNode) documentElement2);
                settingsFromFile.ReplaceChild(settingsFromFile.ImportNode(node, true), (XmlNode) documentElement2);
                FileStream output = new FileStream(GetSettingsFilename(modIdentifier, modHandleName), FileMode.Create, FileAccess.Write, FileShare.None);
                XmlWriter w = XmlWriter.Create((Stream) output, new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "\t"
                });
                settingsFromFile.WriteTo(w);
                w.Close();
                output.Close();
            }
        }
        return false;
    }
}
