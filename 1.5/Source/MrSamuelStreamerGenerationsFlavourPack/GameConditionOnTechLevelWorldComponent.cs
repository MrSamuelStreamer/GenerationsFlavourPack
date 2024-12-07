﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace MSS_Gen;

public class GameConditionOnTechLevelWorldComponent(World world) : WorldComponent(world), ISignalReceiver
{
    public bool ArchoGiftHasFired = false;
    public int CountdownToArchoGift = -1;

    public bool MeteorHasFired = false;
    public int CountdownToMeteor = -1;
    public FloatRange MeteorDuration = new FloatRange(0.06f, 0.07f);

    public bool IceAgeHasFired = false;
    public int CountdownToIceAge = -1;

    public bool GlobalWarmingHasFired = false;
    public int CountdownToGlobalWarming = -1;

    public bool ArchonRaidHasFired = false;
    public int CountdownToArchonRaid = -1;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ArchoGiftHasFired, "ArchoGiftHasFired", false);
        Scribe_Values.Look(ref CountdownToArchoGift, "CountdownToArchoGift", -1);
        Scribe_Values.Look(ref MeteorHasFired, "MeteorHasFired", false);
        Scribe_Values.Look(ref CountdownToMeteor, "CountdownToMeteor", -1);
        Scribe_Values.Look(ref IceAgeHasFired, "IceAgeHasFired", false);
        Scribe_Values.Look(ref CountdownToIceAge, "CountdownToIceAge", -1);
        Scribe_Values.Look(ref GlobalWarmingHasFired, "GlobalWarmingHasFired", false);
        Scribe_Values.Look(ref CountdownToGlobalWarming, "CountdownToGlobalWarming", -1);
        Scribe_Values.Look(ref ArchonRaidHasFired, "ArchonRaidHasFired", false);
        Scribe_Values.Look(ref CountdownToArchonRaid, "CountdownToArchonRaid", -1);
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        Find.SignalManager.RegisterReceiver(this);
    }

    public override void WorldComponentTick()
    {
        base.WorldComponentTick();
        DoArchoGift();
        DoMeteor();
        DoIceAge();
        DoGlobalWarming();
        DoArchonRaid();
    }

    public void DoArchoGift()
    {
        if(CountdownToArchoGift < 0) return;
        if(ArchoGiftHasFired) return;
        if(CountdownToArchoGift >= Find.TickManager.TicksGame) return;

        ArchoGiftHasFired = true;

        Map target = Current.Game.Maps.First(map => map.IsPlayerHome);

        ThingDef gift = DefDatabase<ThingDef>.GetNamedSilentFail("MAG_ArchoReproductor") ?? ThingDefOf.Gold;
        IntVec3 intVec3 = DropCellFinder.RandomDropSpot(target);
        SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming, gift, intVec3, target);

        LookTargets lookTargets = new TargetInfo(intVec3, target);

        Find.LetterStack.ReceiveLetter("MSS_Gen_ArchoGiftLanded_letterTitle".Translate(),
            "MSS_Gen_ArchoGiftLanded_letterDesc".Translate(),
            LetterDefOf.NeutralEvent,
            lookTargets
        );
    }

    public void DoMeteor()
    {
        if(CountdownToMeteor < 0) return;
        if(MeteorHasFired) return;
        if(CountdownToMeteor >= Find.TickManager.TicksGame) return;

        MeteorHasFired = true;

        int duration = Mathf.RoundToInt(MeteorDuration.RandomInRange * GenDate.TicksPerDay);

        GameCondition conditionMeteor = GameConditionMaker.MakeCondition(GameConditionDef.Named("MeteorStorm"), duration);

        Map target = Current.Game.Maps.First(map => map.IsPlayerHome);

        target.gameConditionManager.RegisterCondition(conditionMeteor);

        List<TargetInfo> lookTargets = target.mapPawns.FreeColonists.Select(pawn=>new TargetInfo(pawn)).ToList();

        Find.LetterStack.ReceiveLetter("Meteo_letterTitle".Translate(),
            "Meteo_letterDesc".Translate(),
            LetterDefOf.ThreatBig,
            lookTargets
        );
    }

    public void DoIceAge()
    {
        if(CountdownToIceAge < 0) return;
        if(IceAgeHasFired) return;
        if(CountdownToIceAge >= Find.TickManager.TicksGame) return;

        IceAgeHasFired = true;

        GameCondition conditionIceAge = GameConditionMaker.MakeCondition(GameConditionDef.Named("IceAge"));

        Find.World.gameConditionManager.RegisterCondition(conditionIceAge);

        Find.LetterStack.ReceiveLetter("MSS_Gen_IceAgeStart_letterTitle".Translate(),
            "MSS_Gen_IceAgeStart_letterDesc".Translate(),
            LetterDefOf.ThreatBig
        );
    }

    public void DoGlobalWarming()
    {
        if(CountdownToGlobalWarming < 0) return;
        if(GlobalWarmingHasFired) return;
        if(CountdownToGlobalWarming >= Find.TickManager.TicksGame) return;

        GlobalWarmingHasFired = true;

        GameCondition iceAge = Find.World.gameConditionManager.GetActiveCondition(GameConditionDef.Named("IceAge"));
        iceAge?.End();

        GameCondition conditionGlobalWarming = GameConditionMaker.MakeCondition(GameConditionDef.Named("GlobalWarming"));

        Find.World.gameConditionManager.RegisterCondition(conditionGlobalWarming);

        Find.LetterStack.ReceiveLetter("MSS_Gen_IceAgeStart_letterTitle".Translate(),
            "MSS_Gen_IceAgeStart_letterDesc".Translate(),
            LetterDefOf.ThreatBig
        );
    }

    public void DoArchonRaid()
    {
        if(CountdownToArchonRaid < 0) return;
        if(ArchonRaidHasFired) return;
        if(CountdownToArchonRaid >= Find.TickManager.TicksGame) return;

        ArchonRaidHasFired = true;

        GameCondition globalWarming = Find.World.gameConditionManager.GetActiveCondition(GameConditionDef.Named("GlobalWarming"));
        globalWarming?.End();
        //
        // GameCondition conditionGlobalWarming = GameConditionMaker.MakeCondition(GameConditionDef.Named("GlobalWarming"));
        //
        // Find.World.gameConditionManager.RegisterCondition(conditionGlobalWarming);
        //
        // Find.LetterStack.ReceiveLetter("MSS_Gen_IceAgeStart_letterTitle".Translate(),
        //     "MSS_Gen_IceAgeStart_letterDesc".Translate(),
        //     LetterDefOf.ThreatBig
        // );
    }

    public void Notify_SignalReceived(Signal signal)
    {
        if (signal.tag == Signals.MSS_Gen_TechLevelChanged)
        {
            TechLevel newLevel = (TechLevel)signal.args.GetArg("newTechLevel").arg;

            switch (newLevel)
            {
                case TechLevel.Neolithic:
                    if(ArchoGiftHasFired) return;
                    if(CountdownToArchoGift > -1) return;
                    CountdownToArchoGift = GenDate.TicksPerDay + Find.TickManager.TicksGame;
                    Find.LetterStack.ReceiveLetter("MSS_Gen_ArchoGift_letterTitle".Translate(),
                        "MSS_Gen_ArchoGift_letterDesc".Translate(),
                        LetterDefOf.ThreatBig, delayTicks:GenDate.TicksPerHour);
                    break;
                case TechLevel.Medieval:
                    if(MeteorHasFired) return;
                    if(CountdownToMeteor > -1) return;
                    CountdownToMeteor = GenDate.TicksPerDay + Find.TickManager.TicksGame;
                    Find.LetterStack.ReceiveLetter("MSS_Gen_Meteo_letterTitle".Translate(),
                        "MSS_Gen_Meteo_letterDesc".Translate(),
                        LetterDefOf.ThreatBig, delayTicks:GenDate.TicksPerHour);
                    break;
                case TechLevel.Industrial:
                    if(IceAgeHasFired) return;
                    if(CountdownToIceAge > -1) return;
                    CountdownToIceAge = GenDate.TicksPerDay + Find.TickManager.TicksGame;
                    Find.LetterStack.ReceiveLetter("MSS_Gen_IceAge_letterTitle".Translate(),
                        "MSS_Gen_IceAge_letterDesc".Translate(),
                        LetterDefOf.ThreatBig, delayTicks:GenDate.TicksPerHour);
                    break;
                case TechLevel.Spacer:
                    if(GlobalWarmingHasFired) return;
                    if(CountdownToGlobalWarming > -1) return;
                    CountdownToGlobalWarming = GenDate.TicksPerDay + Find.TickManager.TicksGame;
                    Find.LetterStack.ReceiveLetter("MSS_Gen_GlobalWarming_letterTitle".Translate(),
                        "MSS_Gen_GlobalWarming_letterDesc".Translate(),
                        LetterDefOf.ThreatBig, delayTicks:GenDate.TicksPerHour);
                    break;
                case TechLevel.Ultra:
                    if(ArchonRaidHasFired) return;
                    if(CountdownToArchonRaid > -1) return;
                    CountdownToArchonRaid = GenDate.TicksPerDay + Find.TickManager.TicksGame;
                    Find.LetterStack.ReceiveLetter("MSS_Gen_ArchonRaid_letterTitle".Translate(),
                        "MSS_Gen_ArchonRaid_letterDesc".Translate(),
                        LetterDefOf.ThreatBig, delayTicks:GenDate.TicksPerHour);
                    break;
            }
        }
    }
}