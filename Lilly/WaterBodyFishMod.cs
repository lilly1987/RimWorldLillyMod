using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Lilly
{
    [HarmonyPatch]
    public class WaterBodyFishMod : HarmonyBase
    {

        
        public override string label => "WaterBodyFishMod"; 

        public override string harmonyId => "Lilly.WaterBody";

        public static WaterBodyFishMod self;

        public static int maxFishPopulation = 1000000;

        string tmp;

        public WaterBodyFishMod() : base() {
            self = this;
        }
        
        public override void DoSettingsWindowContents(Rect inRect, Listing_Standard listing)
        {
            base.DoSettingsWindowContents(inRect, listing);
                        
            listing.Label("물고기 최대 개체수 설정".Translate(), tipSignal: "위 패치에 적용. 또는 아래 적용을 눌러야 적용.".Translate());
            tmp = maxFishPopulation.ToString();
            listing.TextFieldNumeric(ref maxFishPopulation, ref tmp);
            if (
                listing.ButtonTextLabeled(
                    "모든 맵에 물고기 종류 추가".Translate()
                    , "적용".Translate()
                    , tooltip: "새로운 맵이 아닌 기존 모든 맵에도 적용. maxFishPopulation = 1000000".Translate()
                )
            )
            {
                WaterBodyFishMod.All();
            }
            ;
            if (
                listing.ButtonTextLabeled(
                    "물고기 종류 출력 테스트".Translate()
                    , "디버그출력".Translate()
                    , tooltip: "테스트".Translate()
                )
            )
            {
                WaterBodyFishMod.GetAllFishDefs();
            }
            ;
        }

        public override void Patch()
        {
            var original = AccessTools.Method(typeof(WaterBody), "SetFishTypes");
            var Prefix = new HarmonyMethod(typeof(WaterBodyFishMod), nameof(WaterBodyFishMod.PreSetFishTypes));
            harmony.Patch(original, Prefix);

            //original = AccessTools.Method(typeof(ScribeLoader), "InitLoading");// RWAutoSell에서 자꾸 호출해대서 수정 필요
            original = AccessTools.Method(typeof(SavedGameLoaderNow), "LoadGameFromSaveFileNow");// RWAutoSell에서 자꾸 호출해대서 수정 필요
            var postfix = new HarmonyMethod(typeof(WaterBodyFishMod), nameof(WaterBodyFishMod.All));
            harmony.Patch(original, postfix: postfix);

            var original2 = AccessTools.Constructor(typeof(Zone_Fishing), new[] { typeof(ZoneManager) });
            postfix = new HarmonyMethod(typeof(WaterBodyFishMod), nameof(WaterBodyFishMod.AfterConstruct));
            harmony.Patch(original2, postfix: postfix);
            
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref maxFishPopulation, "maxFishPopulation");
            WaterBodyFishMod.All();
        }

        public static bool PreSetFishTypes(WaterBody __instance)
        {
            if (__instance.map.Biome.fishTypes == null)
            {
                return false; ;
            }

            var type = __instance.GetType();
            // 🔍 private 필드 접근
            var commonFishField = type.GetField("commonFish", BindingFlags.NonPublic | BindingFlags.Instance);
            var uncommonFishField = type.GetField("uncommonFish", BindingFlags.NonPublic | BindingFlags.Instance);

            var commonFish = (List<ThingDef>)commonFishField.GetValue(__instance);
            var uncommonFish = (List<ThingDef>)uncommonFishField.GetValue(__instance);

            commonFish.Clear();
            uncommonFish.Clear();

            var shouldHaveFishField = type.GetField("shouldHaveFish", BindingFlags.NonPublic | BindingFlags.Instance);
            shouldHaveFishField.SetValue(__instance, true);

            //BiomeFishTypes fishTypes = __instance.map.Biome.fishTypes;
            commonFish.AddRange(ThingCategoryDefOf.Fish.childThingDefs);
            uncommonFish.AddRange(ThingCategoryDefOf.Fish.childThingDefs);
            //commonFish.AddRange(from fishChance in fishTypes.freshwater_Common select fishChance.fishDef);
            //commonFish.AddRange(from fishChance in fishTypes.saltwater_Common select fishChance.fishDef);
            //uncommonFish.AddRange(from fishChance in fishTypes.freshwater_Uncommon select fishChance.fishDef);
            //uncommonFish.AddRange(from fishChance in fishTypes.saltwater_Uncommon select fishChance.fishDef);
            MaxPopulation(__instance);
            return false;
        }

        public static void MaxPopulation(WaterBody __instance)
        {
            __instance.map.TileInfo.PrimaryBiome.maxFishPopulation = maxFishPopulation;
            __instance.Population = __instance.MaxPopulation;
        }

        public static void All()
        {
            if (Find.Maps == null) return;
            foreach (var map in Find.Maps)
            {
                MyLog.Warning($"WaterBody All start",print: self.onDebug);                
                MyLog.Warning($"maxFishPopulation {map.TileInfo.PrimaryBiome.maxFishPopulation}", print: self.onDebug);                
                MyLog.Warning($"Bodies {map.waterBodyTracker.Bodies.Count}", print: self.onDebug);                
                foreach (var waterBody in map.waterBodyTracker.Bodies)
                {
                    waterBody.SetFishTypes();
                }
                MyLog.Warning($"WaterBody All End", print: self.onDebug);                
            }
        }

        public static  void GetAllFishDefs()//List<ThingDef>
        {
            MyLog.Warning($"{ThingCategoryDefOf.Fish.childThingDefs.Count}", print: self.onDebug);
            foreach(var t in ThingCategoryDefOf.Fish.childThingDefs)
            {
                MyLog.Warning($"{t.description}");
            }            
            //return DefDatabase<ThingDef>.AllDefs
            //    .Where(def => def.thingCategories != null &&
            //                  def.thingCategories.Contains(ThingCategoryDefOf.Fish))
            //    .ToList();
        }

        //public FishRepeatMode repeatMode = FishRepeatMode.TargetCount;
        //public FishRepeatMode repeatMode = FishRepeatMode.DoForever;

        public static void AfterConstruct(Zone_Fishing __instance)
        {
            __instance.repeatMode = FishRepeatMode.DoForever;
        }
    }
}
