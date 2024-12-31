using Verse;
using HarmonyLib;

namespace MSS_Gen.Warrants;

public class MSS_GenWarrantsMod : Mod
{

    public MSS_GenWarrantsMod(ModContentPack content) : base(content)
    {
        ModLog.Log("MSS_GenWarrantsMod");
#if DEBUG
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new Harmony("MrSamuelStreamer.rimworld.MSS_GenWarrantsMod.main");
        harmony.PatchAll();
    }
}
