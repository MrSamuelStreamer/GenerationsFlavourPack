using ModlistConfigurator;
using RimWorld;
using Verse;

namespace MSS_Gen.ModListConfiguratorCompat;

public class TechLevelConfigDef: Def
{
    public string presetPath;
    public string presetLabel;
    public string version;
    public TechLevel techLevel;
}
