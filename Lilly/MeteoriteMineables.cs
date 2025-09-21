using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Lilly
{
    //[StaticConstructorOnStartup]
    public static class MeteoriteMineables
    {
        const string HarmonyId = "Lilly.MineablesCountRange";

        //static MeteoriteMineablesCountRange()
        //{
        //    SetValue();
        //}

        public static int countRangeMin = 8;
        public static int countRangeMax = 20;

        static string tmp;

        public static void DoSettingsWindowContents(Rect inRect, Listing_Standard listing)
        {
            listing.Label("자원 운석 최소 갯수".Translate(), tipSignal: ".".Translate());
            tmp = countRangeMin.ToString();
            listing.TextFieldNumeric(ref countRangeMin, ref tmp);

            listing.Label("자원 운석 최대 갯수".Translate(), tipSignal: ".".Translate());
            tmp = countRangeMax.ToString();
            listing.TextFieldNumeric(ref countRangeMax, ref tmp);

            listing.GapLine();
        }

        public static void ExposeData()
        {
            Scribe_Values.Look(ref MeteoriteMineables.countRangeMin, "MeteoriteMineablesCountRangeMin");
            Scribe_Values.Look(ref MeteoriteMineables.countRangeMax, "MeteoriteMineablesCountRangeMax");
            if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving)
            {
                return;
            }
            MeteoriteMineables.SetValue(MeteoriteMineables.countRangeMin, MeteoriteMineables.countRangeMax);
        }

        public static void SetValue(int min=25,int max=100)
        {

            try
            {
                // 대상 필드 찾기
                FieldInfo field = typeof(ThingSetMaker_Meteorite).GetField("MineablesCountRange", BindingFlags.Public | BindingFlags.Static);

                if (field != null)
                {
                    var range = new IntRange(min, max); // 원하는 범위로 설정
                    // readonly 필드 강제 수정
                    field.SetValue(null, range); // 원하는 범위로 변경
                    Log.Warning($"+++ {HarmonyId} MineablesCountRange has been overridden to {range.min}~{range.max}");
                }
                else
                {
                    Log.Warning($"+++ {HarmonyId} Failed to find MineablesCountRange field");
                }

            }
            catch (Exception e)
            {
                Log.Error($"+++ {HarmonyId} loaded1 Fail +++");
                Log.Error(e.ToString());
                Log.Error($"+++ {HarmonyId} loaded1 Fail +++");
                //throw;
            }
        }
    }
}
