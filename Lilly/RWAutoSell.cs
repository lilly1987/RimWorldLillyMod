using HarmonyLib;
using Lilly;
using RWASFilterLib;
using RWAutoSell;
using RWAutoSell.AI;

//using RWASFilterLib;
//using RWAutoSell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using Verse; // 림월드 기본 네임스페이스

//[StaticConstructorOnStartup]
public class RWAutoSell_Patch : HarmonyBase
{
    public override string harmonyId => "Lilly.RWAutoSell";

    public override string label => throw new System.NotImplementedException();

    public new bool onPatch = false;

    public static List<IRule> ruleList = new List<IRule>();

    public static Map tmpmap;
    public static RWAutoSell_Patch self;

    public RWAutoSell_Patch()
    {
        self = this;
    }


    public override void ExposeData()
    {

        Scribe_Values.Look(ref onDebug, "onDebug", false);
    }

    public override void DoSettingsWindowContents(Rect inRect, Listing_Standard listing)
    {
        listing.CheckboxLabeled("RWAutoSell_Patch Debug", ref onDebug, ".");
        listing.GapLine();
    }

    [HarmonyPatch(typeof(SavedGameLoaderNow), nameof(SavedGameLoaderNow.LoadGameFromSaveFileNow))]
    public static class Patch_LoadGame
    {
        [HarmonyPostfix]
        public static void Patch()
        {
            MyLog.Warning($"LoadGameFromSaveFileNow ST", print: self.onDebug);

            var aSMapComp = ASMapComp.GetSingleton(Find.CurrentMap);
            if (aSMapComp== null)
            {
                MyLog.Error($"LoadGameFromSaveFileNow ASMapComp NULL");
                return;
            }
            ruleList.Clear();
            foreach (var rule in aSMapComp.EnumerateRules())
            {
                ruleList.Add(rule.DeepCopy());
            }
            MyLog.Warning($"LoadGameFromSaveFileNow ED {aSMapComp.Rules.Count}", print: self.onDebug);
        }
    }

    [HarmonyPatch(typeof(ASMapComp), nameof(ASMapComp.MapGenerated))]
    public static class Patch_MapGenerated
    {
        [HarmonyPostfix]
        public static void Postfix(ASMapComp __instance)
        {
            MyLog.Warning($"MapGenerated ST", print: self.onDebug);

            try
            {
                MyLog.Warning($"map {__instance?.map}", print: self.onDebug);
                MyLog.Warning($"Find.Maps.Count {Find.Maps?.Count} ", print: self.onDebug);

                var aSMapComp = __instance;
                if (aSMapComp == null)
                {
                    MyLog.Error($"AddMap ASMapComp NULL");
                    return;
                }
                if (Find.Maps.Count == 1)
                {
                    tmpmap = __instance.map;
                    foreach (var rule in ruleList)
                    {
                        aSMapComp.Add(rule.DeepCopy());
                    }
                }
                else if (Find.Maps.Count > 1)
                {
                    foreach (var rule in ASMapComp.GetSingleton(Find.Maps[Find.Maps.Count - 2]).EnumerateRules())
                    {
                        aSMapComp.Add(rule.DeepCopy());
                    }
                }
                MyLog.Warning($"aSMapComp.Rules.Count {aSMapComp.Rules.Count}", print: self.onDebug);
            }
            catch (Exception e)
            {
                MyLog.Error(e.ToString());
            }
            MyLog.Warning($"MapGenerated ED", print: self.onDebug);
        }
    }

    [HarmonyPatch(typeof(ASMapComp), nameof(ASMapComp.MapRemoved))]
    public static class MapRemoved
    {
        [HarmonyPrefix]
        public static void Postfix(ASMapComp __instance)
        {
            try
            {
                MyLog.Warning($"MapRemoved ST", print: self.onDebug);
                MyLog.Warning($"__instance.Rules.Count {__instance?.Rules?.Count}", print: self.onDebug);
                MyLog.Warning($"__instance.map {__instance?.map}", print: self.onDebug);
                MyLog.Warning($"Find.Maps.Count {Find.Maps?.Count}", print: self.onDebug);
                if (Find.Maps?.Count == 0)
                {
                    ruleList.Clear();
                    foreach (var rule in __instance.EnumerateRules())
                    {
                        ruleList.Add(rule.DeepCopy());
                    }
                }

                MyLog.Warning($"__instance.Rules.Count {__instance?.Rules?.Count}", print: self.onDebug);
            }
            catch (Exception e)
            {
                MyLog.Error(e.ToString());
            }
            MyLog.Warning($"MapRemoved ED" , print: self.onDebug);
        }
    }

    //[HarmonyPatch(typeof(Game), nameof(Game.DeinitAndRemoveMap))]// 정상이지만 중복이라 제거
    public static class Patch_RemoveMap
    {
        [HarmonyPrefix]
        //[HarmonyPostfix]
        public static void Patch(Map map)
        {
            MyLog.Warning($"DeinitAndRemoveMap ST",print: self.onDebug);
            try
            {
                MyLog.Warning($"map {map}", print: self.onDebug);

                MyLog.Warning($"Find.Maps.Count {Find.Maps?.Count}", print: self.onDebug);

                if (Find.Maps?.Count > 0)
                {
                    var aSMapComp = ASMapComp.GetSingleton(Find.CurrentMap);
                    if (aSMapComp == null)
                    {
                        MyLog.Error($"DeinitAndRemoveMap ASMapComp NULL");
                        return;
                    }
                    MyLog.Warning($"aSMapComp.Rules.Count  {aSMapComp.Rules.Count}", print: self.onDebug);
                    ruleList.Clear();
                    foreach (var rule in aSMapComp.EnumerateRules())
                    {
                        ruleList.Add(rule.DeepCopy());
                    }
                }
            }
            catch (Exception e)
            {
                MyLog.Error(e.ToString());
            }
            MyLog.Warning($"DeinitAndRemoveMap ED", print: self.onDebug);
        }
    }



}