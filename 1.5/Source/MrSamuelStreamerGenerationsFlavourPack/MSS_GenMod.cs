using System;
using System.Linq;
using System.Reflection;
using Verse;
using UnityEngine;
using HarmonyLib;
using JetBrains.Annotations;
using ModlistConfigurator;

namespace MSS_Gen;

public class MSS_GenMod : Mod
{

    public static Settings settings;
    public static MSS_GenMod mod;

    public MSS_GenMod(ModContentPack content) : base(content)
    {
        mod = this;
        ModLog.Debug("Hello world from Mr Samuel Streamer's Generations Flavour Pack");

        // initialize settings
        settings = GetSettings<Settings>();
#if DEBUG
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new Harmony("MrSamuelStreamer.rimworld.MSS_Gen.main");
        harmony.PatchAll();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        settings.DoWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "MSS_Gen_SettingsCategory".Translate();
    }
}
