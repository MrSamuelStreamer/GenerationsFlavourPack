using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MSS_Gen;

public class TechLevelConfigDef: Def
{
    public static Lazy<List<ModContentPack>> RunningMods = new(() => LoadedModManager.RunningMods.ToList());

    public string presetPath;
    public string presetLabel;
    public string version;
    public TechLevel techLevel;
    public List<string> modsMustExists;

    public bool MeetsRequirements(TechLevel techLevelToCheck)
    {
        if (techLevel != techLevelToCheck) return false;
        if (modsMustExists.NullOrEmpty()) return true;
        return modsMustExists.All(modPackageId=> RunningMods.Value.Any(runningMod => runningMod.PackageId == modPackageId));
    }
}
