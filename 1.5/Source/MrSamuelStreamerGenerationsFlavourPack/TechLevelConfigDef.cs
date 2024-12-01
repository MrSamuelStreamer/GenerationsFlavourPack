using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MSS_Gen;

public class TechLevelConfigDef: Def
{
    public string presetPath;
    public string presetLabel;
    public string version;
    public TechLevel techLevel;
    public List<string> modsMustExists;
}
