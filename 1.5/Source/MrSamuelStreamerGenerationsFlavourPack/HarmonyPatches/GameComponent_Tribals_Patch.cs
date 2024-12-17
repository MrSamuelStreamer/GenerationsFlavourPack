using HarmonyLib;
using RimWorld;
using Verse;
using VFETribals;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(GameComponent_Tribals))]
public static class GameComponent_Tribals_Patch
{
    [HarmonyPatch(nameof(GameComponent_Tribals.AdvanceToEra))]
    [HarmonyPostfix]
    public static void AdvanceToEraPostfix(EraAdvancementDef def)
    {
        TechConfigWorldComponent comp = Find.World.GetComponent<TechConfigWorldComponent>();

        if(comp == null) return;

        if (def == VFET_DefOf.VFET_FormTribe)
        {
            comp.SetNewConfigs(TechLevel.Neolithic);
        }
        else if (def == VFET_DefOf.VFET_FormTown)
        {
            comp.SetNewConfigs(TechLevel.Medieval);
        }
        else if (def == VFET_DefOf.VFET_FormCity)
        {
            comp.SetNewConfigs(TechLevel.Industrial);
        }
        else if (def == VFET_DefOf.VFET_FormCollective)
        {
            comp.SetNewConfigs(TechLevel.Spacer);
        }
        else if (def == VFET_DefOf.VFET_FormNexus)
        {
            comp.SetNewConfigs(TechLevel.Ultra);
        }
    }
}
