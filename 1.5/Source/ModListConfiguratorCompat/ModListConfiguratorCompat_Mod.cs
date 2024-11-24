using HarmonyLib;
using Verse;

namespace MSS_Gen.ModListConfiguratorCompat;

public class ModListConfiguratorCompat_Mod : Mod
{
    public static ModListConfiguratorCompat_Mod mod;
    public ModListConfiguratorCompat_Mod(ModContentPack content) : base(content)
    {
        mod = this;
#if DEBUG
        ModLog.Log("MSS_Gen.ModListConfiguratorCompat");
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new Harmony("MSS_Gen.ModListConfiguratorCompat.main");
        harmony.PatchAll();
    }
}
