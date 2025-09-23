using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly
{
    //[StaticConstructorOnStartup]
    public static class DrillCache
    {
        //public static string harmonyId = "Lilly.DrillCache";
        //public static Harmony harmony;
        public static bool DebugMode=false;

        public static Dictionary<Map, List<Building>> cachedRocks = new Dictionary<Map, List<Building>>();
        //static DrillCache()
        //{
        //    if (harmony == null)
        //    {
        //        MyLog.Warning($"{harmonyId} Patch ST");
        //        try
        //        {
        //            harmony = new Harmony(harmonyId);
        //            harmony.PatchAll();
        //            MyLog.Warning($"{harmonyId} Patch SUCC");
        //        }
        //        catch (System.Exception e)
        //        {
        //            MyLog.Error($"{harmonyId} Patch Fail");
        //            MyLog.Error(e.ToString());
        //            MyLog.Error($"{harmonyId} Patch Fail");
        //        }
        //        MyLog.Warning($"{nameof(Startup)} Patch ED");
        //    }
        //}

        [HarmonyPatch(typeof(Map), "FinalizeInit")]
        public static class Patch_MapFinalizeInit
        {
            [HarmonyPostfix]
            public static void OnMapInit(Map __instance)
            {
                if (DebugMode)
                    MyLog.Warning($"[DrillCache] Map FinalizeInit {__instance} {__instance.Tile}");
                var mineableBuildings = __instance.listerThings.AllThings
                    .OfType<Building>()
                    .Where(b => b.def.mineable)
                    .ToList();

                DrillCache.cachedRocks[__instance] = mineableBuildings;
            }
        }

        [HarmonyPatch(typeof(Map), "FinalizeLoading")]
        public static class Patch_Map_FinalizeLoading
        {
            [HarmonyPostfix]
            public static void OnMapLoaded(Map __instance)
            {
                if (DebugMode)
                    MyLog.Warning($"[DrillCache] Map FinalizeLoading {__instance} {__instance.Tile}");
                var mineables = __instance.listerThings.AllThings
                    .OfType<Building>()
                    .Where(b => b.def.mineable)
                    .ToList();

                DrillCache.cachedRocks[__instance] = mineables;
            }
        }

        [HarmonyPatch(typeof(ThingSetMaker_Meteorite), "Generate")]
        public static class Patch_MeteoriteGenerate
        {
            [HarmonyPostfix]
            public static void AfterMeteoriteGenerated(ThingSetMakerParams parms, List<Thing> outThings)
            {
                if (DebugMode)
                    MyLog.Warning($"[DrillCache] Meteorite Generated on tile {parms.tile}, things count: {outThings.Count}");

                if (parms.tile == -1) return;

                Map map = Find.Maps.FirstOrDefault(m => m.Tile == parms.tile);
                if (map == null) return;

                var mineables = outThings
                    .OfType<Building>()
                    .Where(b => b.def.mineable)
                    .ToList();

                if (mineables.Count == 0) return;

                if (!DrillCache.cachedRocks.TryGetValue(map, out var list))
                    DrillCache.cachedRocks[map] = list = new List<Building>();

                list.AddRange(mineables);
            }
        }

        [HarmonyPatch(typeof(Building), nameof(Thing.DeSpawn))]
        public static class Patch_Building_DeSpawn
        {
            [HarmonyPrefix]
            public static void OnBuildingDespawn(Building __instance)
            {
                if (DebugMode)
                    MyLog.Warning($"[DrillCache] Thing DeSpawn {__instance} {__instance.Map} {__instance.def.defName}");

                if (__instance.Map == null)
                    return;

                if (__instance.def.mineable &&
                    DrillCache.cachedRocks.TryGetValue(__instance.Map, out var list))
                {
                    list.Remove(__instance);
                }
            }
        }

        //[HarmonyPatch(typeof(Thing), nameof(Thing.DeSpawn))]
        public static class Patch_Thing_DeSpawn
        {
            [HarmonyPrefix]
            public static void OnThingDespawn(Thing __instance)
            {
                if (DebugMode)
                    MyLog.Warning($"[DrillCache] Thing DeSpawn {__instance} {__instance.Map} {__instance.def.defName}");

                if (__instance.Map == null || !(__instance is Building))
                    return;

                Building building = __instance as Building;
                if (building.def.mineable &&
                    DrillCache.cachedRocks.TryGetValue(__instance.Map, out var list))
                {
                    list.Remove(building);
                }
            }
        }

    }
}
