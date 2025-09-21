using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly
{
    //[StaticConstructorOnStartup]
    public static class DropPodUtilityForceForbid
    {
        const string HarmonyId = "Lilly.DropPodUtility";
        static Harmony harmony;

        static DropPodUtilityForceForbid()
        {
            //return;
            Log.Warning($"+++ {HarmonyId} loading +++");

            harmony = new Harmony(HarmonyId);

            try
            {
                var original = AccessTools.Method(typeof(DropPodUtility), "DropThingGroupsNear");
                var prefix = typeof(DropPodUtilityForceForbid).GetMethod(nameof(ForceForbid));
                harmony.Patch(original, prefix: new HarmonyMethod(prefix));
                Log.Warning($"+++ {HarmonyId} loaded1 succ +++");
            }
            catch (Exception e)
            {
                Log.Error($"+++ {HarmonyId} loaded1 Fail +++");
                Log.Error(e.ToString());
                Log.Error($"+++ {HarmonyId} loaded1 Fail +++");
                //throw;
            }
        }

        public static bool ForceForbid(
            ref bool forbid)
        {
            forbid = false;
            return true; // 계속 원래 메서드 실행
        }

    }
}
