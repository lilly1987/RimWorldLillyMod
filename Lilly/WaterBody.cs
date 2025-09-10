using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace Lilly
{
    public class WaterBodyFishMod : LillyHarmonyBase
    {
        public override string harmonyId => "Lilly.WaterBody";

        public override void Patch()
        {
            var original = AccessTools.Method(typeof(WaterBody), "SetFishTypes");
            var Prefix = new HarmonyMethod(typeof(WaterBodyFishMod), nameof(WaterBodyFishMod.Prefix));
            harmony.Patch(original, Prefix);
        }

        [HarmonyPrefix]
        public static bool Prefix(WaterBody __instance)
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
            __instance.map.TileInfo.PrimaryBiome.maxFishPopulation = settings.maxFishPopulation;
            __instance.Population = __instance.MaxPopulation;
        }

        public static void All()
        {
            foreach (var map in Find.Maps)
            {
                Log.Warning($"+++ maxFishPopulation {map.TileInfo.PrimaryBiome.maxFishPopulation} +++");                
                Log.Warning($"+++ Bodies {map.waterBodyTracker.Bodies.Count} +++");                
                foreach (var waterBody in map.waterBodyTracker.Bodies)
                {
                    waterBody.SetFishTypes();
                }
            }
        }

        public static  void GetAllFishDefs()//List<ThingDef>
        {
            Log.Warning($"+++ {ThingCategoryDefOf.Fish.childThingDefs.Count} +++");
            foreach(var t in ThingCategoryDefOf.Fish.childThingDefs)
            {
                Log.Warning($"+++ {t.description} +++");
            }            
            //return DefDatabase<ThingDef>.AllDefs
            //    .Where(def => def.thingCategories != null &&
            //                  def.thingCategories.Contains(ThingCategoryDefOf.Fish))
            //    .ToList();
        }

    }
}
