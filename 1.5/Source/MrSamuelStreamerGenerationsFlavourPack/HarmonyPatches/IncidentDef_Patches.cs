using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MSS_Gen.TechAdvancingCompat;
using RimWorld;

namespace MSS_Gen.HarmonyPatches;

[HarmonyPatch(typeof(IncidentDef))]
public static class IncidentDef_Patches
{
    public class NeverRunIncidentWorker : IncidentWorker
    {
        public override float ChanceFactorNow(IIncidentTarget target) => 0f;
        protected override bool CanFireNowSub(IncidentParms parms) => false;
    }

    public static Dictionary<IncidentDef, NeverRunIncidentWorker> neverRunIncidentWorkers = new();

    public static NeverRunIncidentWorker WorkerForDef(IncidentDef def)
    {
        if (!neverRunIncidentWorkers.TryGetValue(def, out NeverRunIncidentWorker result))
        {
            result = new NeverRunIncidentWorker();
            neverRunIncidentWorkers[def] = result;
        }

        return result;
    }

    [HarmonyPatch(nameof(IncidentDef.Worker), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool Worker(IncidentDef __instance, ref IncidentWorker __result)
    {
        if (DisableForTechLevelDef.DisabledForThisTechLevel().SelectMany(def => def.incidents).Any(inc => inc == __instance))
        {
            __result = WorkerForDef(__instance);
            return false;
        }

        return true;
    }

}
