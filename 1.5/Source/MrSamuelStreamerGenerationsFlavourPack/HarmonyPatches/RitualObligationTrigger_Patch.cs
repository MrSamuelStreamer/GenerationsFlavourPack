using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using VFETribals;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(RitualObligationTrigger))]
public static class RitualObligationTrigger_Patch
{
    [HarmonyPatch(nameof(RitualObligationTrigger.Notify_RitualExecuted))]
    [HarmonyPostfix]
    public static void Notify_RitualExecuted(RitualObligationTrigger __instance, LordJob_Ritual ritual)
    {
        if(__instance is not RitualObligationTrigger_TargetTechlevel ritualObligationTrigger) return;

        TechConfigWorldComponent comp = Find.World.GetComponent<TechConfigWorldComponent>();

        if(comp == null) return;

        EraAdvancementDef def = DefDatabase<EraAdvancementDef>.AllDefs.FirstOrDefault(def => def.newTechLevel == ritualObligationTrigger.targetTechLevel);

        if(def == null) return;

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
