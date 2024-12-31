using System.Linq;
using HarmonyLib;
using RimWorld;
using SimpleWarrants;
using UnityEngine;
using Verse;

namespace MSS_Gen.Warrants;

[HarmonyPatch(typeof(Warrant_Pawn))]
public static class Warrant_Pawn_Patch
{
    [HarmonyPatch(nameof(Warrant_Pawn.Draw))]
    [HarmonyPostfix]
    public static void Draw_Patch(Warrant_Pawn __instance, Rect rect)
    {
        if (!Prefs.DevMode || !DebugSettings.godMode) return;
        if (__instance.issuer == Faction.OfPlayer && __instance.accepteer is null)
        {
            Rect forceAcceptRect = new Rect(rect.x + 5, rect.y + 105, 95, 30);
            if (Widgets.ButtonText(forceAcceptRect, "Force Accept"))
            {
                // WarrantsManager.Instance.createdWarrants.Remove(this);
                bool IsValidFaction(Faction faction)
                    => faction.def.humanlikeFaction &&
                       !faction.defeated &&
                       !faction.Hidden &&
                       !faction.IsPlayer &&
                       __instance.issuer != faction &&
                       faction.RelationKindWith(Faction.OfPlayer) != FactionRelationKind.Hostile &&
                       Find.World.worldObjects.Settlements.Any(settlement => settlement.Faction == faction);

                if (!Find.FactionManager.AllFactions.Where(IsValidFaction).TryRandomElement(out var takerFaction))
                {
                    Log.ErrorOnce("Failed to find any valid faction to accept player warrant.", __instance.GetHashCode());
                    return;
                }

                __instance.AcceptBy(takerFaction);
                WarrantsManager manager = Current.Game.GetComponent<WarrantsManager>();
                manager.createdWarrants.Remove(__instance);
                manager.takenWarrants.Add(__instance);
                __instance.tickToBeCompleted = Find.TickManager.TicksGame + (GenDate.TicksPerDay * (int)Rand.Range(3f, 15f));
                Messages.Message("SW.FactionTookYourWarrant".Translate(takerFaction.Named("FACTION"), __instance.thing.LabelCap), MessageTypeDefOf.PositiveEvent);

            }
        }
    }
}
