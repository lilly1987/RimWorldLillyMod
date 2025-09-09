using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace Lilly
{
    // 정상 작동
    [StaticConstructorOnStartup]
    public static class LillyMod_ResourcePodGenerate2
    {
        const string HarmonyId = "com.Lilly.ResourcePodGenerate2";
        static Harmony harmony;

        static LillyMod_ResourcePodGenerate2()
        {
            //return;
            Log.Warning($"+++ {HarmonyId} loading +++");

            harmony = new Harmony(HarmonyId);

            try
            {
                //harmony.PatchAll(typeof(LillyMod_ResourcePodGenerate).Assembly);
                harmony.PatchAll();
                //var original = AccessTools.Method(typeof(ThingSetMaker_ResourcePod), "Generate");
                //var prefix = typeof(Patch_ResourcePodGenerate).GetMethod(nameof(Patch_ResourcePodGenerate.Transpile));
                //harmony.Patch(original, transpiler: new HarmonyMethod(prefix));
                Log.Warning($"+++ {HarmonyId} loaded1 succ +++");
            }
            catch (Exception e)
            {
                Log.Error($"+++ {HarmonyId} loaded1 Fail +++");
                Log.Error(e.ToString());
                Log.Error($"+++ {HarmonyId} loaded1 Fail +++");
                //throw;

            }

            Log.Warning($"+++ {HarmonyId} end +++");

        }
/*
        [HarmonyPatch(typeof(ThingSetMaker_ResourcePod), "Generate")]
        public static class Patch_ResourcePod_Generate
        {
            [HarmonyPrefix]
            public static bool ReplaceGenerate(ThingSetMakerParams parms, List<Thing> outThings)
            {
                // ✅ 원래 메서드 실행을 막고, 여기에 새로운 로직을 작성
                Log.Warning("🔁 ThingSetMaker_ResourcePod.Generate has been replaced!");

                ThingDef thingDef = ThingSetMaker_ResourcePod.RandomPodContentsDef(false);
                float num = Rand.Range(1500f, 60000f);
                while (num > thingDef.BaseMarketValue)
                {
                    ThingDef stuff = GenStuff.RandomStuffByCommonalityFor(thingDef, TechLevel.Undefined);
                    Thing thing = ThingMaker.MakeThing(thingDef, stuff);
                    int num2 = Rand.Range(20, 4000);
                    if (num2 > thing.def.stackLimit)
                    {
                        num2 = thing.def.stackLimit;
                    }
                    float statValue = thing.GetStatValue(StatDefOf.MarketValue, true, -1);
                    if ((float)num2 * statValue > num)
                    {
                        num2 = Mathf.FloorToInt(num / statValue);
                    }
                    if (num2 == 0)
                    {
                        num2 = 1;
                    }
                    thing.stackCount = num2;
                    outThings.Add(thing);
                    num -= (float)num2 * statValue;
                    if (outThings.Count >= 7 || num <= statValue)
                    {
                        break;
                    }
                    thingDef = ThingSetMaker_ResourcePod.RandomPodContentsDef(false);
                }

                // false 반환 → 원래 메서드 실행 안 함
                return false;
            }
        }
*/
    }
}
