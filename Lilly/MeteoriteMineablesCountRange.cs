using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly
{
    [StaticConstructorOnStartup]
    public static class MeteoriteMineablesCountRange
    {
        const string HarmonyId = "com.Lilly.MineablesCountRange";

        static MeteoriteMineablesCountRange()
        {
            try
            {
                // 대상 필드 찾기
                FieldInfo field = typeof(ThingSetMaker_Meteorite).GetField("MineablesCountRange", BindingFlags.Public | BindingFlags.Static);

                if (field != null)
                {
                    var range = new IntRange(25, 100); // 원하는 범위로 설정
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
