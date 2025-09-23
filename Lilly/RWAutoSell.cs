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
using Verse; // 림월드 기본 네임스페이스

//[StaticConstructorOnStartup]
public class RWAutoSell_ASMainTabList : HarmonyBase
{
    public override string harmonyId => "Lilly.RWAutoSell";

    public override string label => throw new System.NotImplementedException();

    public new bool onPatch = false;

    public static List<IRule> ruleList = new List<IRule>();
    //public static ASMapComp aSMapComp = null;

    //Verse.GameComponentUtility:FinalizeInit()
    //Verse.Game:FinalizeInit()
    //Verse.Game:LoadGame()
    //Verse.SavedGameLoaderNow:LoadGameFromSaveFileNow(string)

    //[HarmonyPatch(typeof(GameComponentUtility), nameof(GameComponentUtility.LoadedGame))]// 오류
    //[HarmonyPatch(typeof(ASGameComp), nameof(ASGameComp.FinalizeInit))]//오류
    //[HarmonyPatch(typeof(ASMod), nameof(ASMod.BindWindows))]//작동 안함

    [HarmonyPatch(typeof(MapComponentUtility), nameof(MapComponentUtility.FinalizeInit))]//오류
    public static class Patch_FinalizeInit
    {
        [HarmonyPostfix]
        public static void Patch()
        {
            MyLog.Warning("Save file loaded. Game is ready.");
        }
    }

    public static Map tmpmap;

    [HarmonyPatch(typeof(SavedGameLoaderNow), nameof(SavedGameLoaderNow.LoadGameFromSaveFileNow))]
    public static class Patch_LoadGame
    {
        //[HarmonyPrefix]
        [HarmonyPostfix]
        //public static bool Prefix(ASMapComp __instance)
        public static void Patch()
        {
            MyLog.Warning($"LoadGameFromSaveFileNow ST");

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
            MyLog.Warning($"LoadGameFromSaveFileNow ED {aSMapComp.Rules.Count}");
        }
    }

    [HarmonyPatch(typeof(ASMapComp), nameof(ASMapComp.MapGenerated))]
    public static class Patch_MapGenerated
    {
        [HarmonyPostfix]
        public static void Postfix(ASMapComp __instance)
        {
            MyLog.Warning($"MapGenerated ST {__instance.map} {Find.Maps.Count} ");

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
            MyLog.Warning($"MapGenerated ED {aSMapComp.Rules.Count}");
        }
    }

    [HarmonyPatch(typeof(ASMapComp), nameof(ASMapComp.MapRemoved))]
    public static class MapRemoved
    {
        [HarmonyPostfix]
        public static void Postfix(ASMapComp __instance)
        {
            MyLog.Warning($"MapRemoved ST {__instance.map} {Find.Maps.Count} ");


            MyLog.Warning($"MapRemoved ED {__instance.Rules.Count}");
        }
    }

    //[HarmonyPatch(typeof(Game), nameof(Game.AddMap))]// 오류
    public static class Patch_AddMap
    {
        //[HarmonyPrefix]
        [HarmonyPostfix]
        public static void Patch(Map map)
        {
            MyLog.Warning($"AddMap {map}");

            var aSMapComp =ASMapComp.GetSingleton(map);
            if (aSMapComp == null)
            {
                MyLog.Error($"AddMap ASMapComp NULL");
                return;
            }
                if (Find.Maps.Count == 1)
            {
                tmpmap = map;
                foreach (var rule in ruleList)
                {
                    aSMapComp.Add(rule.DeepCopy());
                }
            }
            else
            {
                foreach (var rule in ASMapComp.GetSingleton(tmpmap).EnumerateRules())
                {
                    aSMapComp.Add(rule.DeepCopy());
                }
            }
            MyLog.Warning($"AddMap {aSMapComp.Rules.Count}");

            //ASMod.GetSingleton.mapComps.Add(aSMapComp); // 조건 없이 추가

            //return false; // 원래 메서드 실행 방지
        }

    }

    [HarmonyPatch(typeof(Game), nameof(Game.DeinitAndRemoveMap))]// 작동은 됨
    public static class Patch_RemoveMap
    {
        [HarmonyPrefix]
        //[HarmonyPostfix]
        public static void Patch(Map map)
        {
            //MyLog.Warning($"[RWAutoSell] MapRemoved {__instance} {__instance.map} {__instance.Rules.Count}");
            MyLog.Warning($"DeinitAndRemoveMap ST {map} {Find.Maps.Count}");

            if (Find.Maps.Count > 0)
            {
                ruleList.Clear();
                var aSMapComp = ASMapComp.GetSingleton(Find.CurrentMap);
                if (aSMapComp == null)
                {
                    MyLog.Error($"DeinitAndRemoveMap ASMapComp NULL");
                    //return true; // 원래 메서드 실행
                }
                MyLog.Warning($"DeinitAndRemoveMap ASMapComp {aSMapComp.Rules.Count}");
                foreach (var rule in aSMapComp.EnumerateRules())
                {
                    ruleList.Add(rule.DeepCopy());
                }
            }

            MyLog.Warning($"DeinitAndRemoveMap ED");

            //ASMod.GetSingleton.mapComps.Remove(__instance); // 조건 없이 제거

            //return false; // 원래 메서드 실행 방지
        }
    }



}