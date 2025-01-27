using RimWorld;
using Verse;

namespace MSS_Gen;

[DefOf]
public static class MSS_GenDefOf
{
    // Remember to annotate any Defs that require a DLC as needed e.g.
    // [MayRequireBiotech]
    // public static GeneDef YourPrefix_YourGeneDefName;

    public static GeneDef MSS_Gen_VoidsEmbrace;
    public static IncidentDef VREA_PsychicStorm;

    static MSS_GenDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(MSS_GenDefOf));
}
