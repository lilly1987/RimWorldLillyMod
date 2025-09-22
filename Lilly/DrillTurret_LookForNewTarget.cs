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

namespace Lilly
{
    public class DrillTurret_LookForNewTarget : HarmonyBase
    {
        public override string harmonyId => "Lilly.DrillTurret";

        public override string label => "DrillTurret_LookForNewTarget";

        public static bool onSight = true;

        public override void DoSettingsWindowContents(Rect inRect, Listing_Standard listing)
        {
            listing.CheckboxLabeled("DrillTurret 범위 무제한 적용 패치".Translate(), ref onPatch, ".".Translate());
            listing.CheckboxLabeled("시야 무시".Translate(), ref onSight, ".".Translate());
            listing.GapLine();
        }

        public override void ExposeData()
        {
            TogglePatch();
            Scribe_Values.Look(ref onSight, "onSight", true);
        }

        public override void Patch()
        {
            var harmony = new Harmony(harmonyId);

            // internal 클래스 접근
            drillTurretType = AccessTools.TypeByName("Building_DrillTurret");
            if (drillTurretType == null)
            {
                MyLog.Warning("Building_DrillTurret 타입을 찾을 수 없습니다.");
                return;
            }

            MethodInfo lookForNewTarget = AccessTools.Method(drillTurretType, "lookForNewTarget",
                new Type[] { typeof(IntVec3).MakeByRefType() } );

            MyLog.Warning("isValidTargetAt");
            isValidTargetAt = AccessTools.Method(drillTurretType, "isValidTargetAt",
                new Type[] { typeof(IntVec3) });
            MyLog.Warning("TargetPosition");
            targetPosProp = AccessTools.Field(drillTurretType, "TargetPosition");

            // miningMode 필드 추출
            miningModeField = AccessTools.Field(drillTurretType, "miningMode");
            MyLog.Warning("end");

            if (lookForNewTarget == null || isValidTargetAt == null || targetPosProp == null )
            {
                MyLog.Error($"[DrillTurretPatch] 필요한 메서드 또는 필드를 찾을 수 없습니다.{lookForNewTarget == null}, {isValidTargetAt == null} , {targetPosProp == null} ");
                return;
            }

            // Prefix 메서드 정의
            MethodInfo prefixMethod = typeof(DrillTurret_LookForNewTarget).GetMethod("Prefix", BindingFlags.Static | BindingFlags.Public);
            MethodInfo prefixMethod2 = typeof(DrillTurret_LookForNewTarget).GetMethod("MyisValidTargetAt", BindingFlags.Static | BindingFlags.Public);

            // 패치 적용
            harmony.Patch(lookForNewTarget, prefix: new HarmonyMethod(prefixMethod));

            harmony.Patch(isValidTargetAt, prefix: new HarmonyMethod(prefixMethod2));

        }

        public static Type drillTurretType;
        public static MethodInfo isValidTargetAt;
        public static FieldInfo targetPosProp;
        public static FieldInfo miningModeField;

        public static bool Prefix(object __instance, out IntVec3 newTargetPosition,ref float ___turretTopRotation,bool ___designatedOnly)
        {
            newTargetPosition = IntVec3.Invalid;

            // 캐스팅
            var thing = __instance as Thing;
            var map = thing.Map;

            var miningMode = (MiningMode)miningModeField.GetValue(__instance);

            // 맵 전체 셀 무작위 순회
            bool isValid=false;

            var designated = new List<IntVec3>();
            var fallback = new List<IntVec3>();

            foreach (var cell in map.AllCells)
            {
                Building edifice = cell.GetEdifice(map);
                if (edifice == null || !edifice.def.mineable)
                    continue;

                if (map.designationManager.DesignationAt(cell, DesignationDefOf.Mine) != null)
                    designated.Add(cell);
                else
                    fallback.Add(cell);
            }

            //foreach (var cell in map.AllCells.InRandomOrder())
            foreach (var cell in designated.InRandomOrder())
            {
                //bool isValid = (bool)isValidTargetAt.Invoke(__instance, new object[] { cell });
                MyisValidTargetAt(__instance,ref isValid, cell , ___designatedOnly,  miningMode);
                if (isValid)
                {
                    newTargetPosition = cell;
                    break;
                }
            }
            if (!newTargetPosition.IsValid)
                foreach (var cell in fallback.InRandomOrder())
                {
                    //bool isValid = (bool)isValidTargetAt.Invoke(__instance, new object[] { cell });
                    MyisValidTargetAt(__instance, ref isValid, cell, ___designatedOnly, miningMode);
                    if (isValid)
                    {
                        newTargetPosition = cell;
                        break;
                    }
                }

            // 타겟이 유효하면 회전 처리
            if (newTargetPosition.IsValid)
            {
                var targetPos = (IntVec3)targetPosProp.GetValue(__instance);
                //var trueCenter = (Vector3)trueCenterMethod.Invoke(__instance, null);
                float angle = (targetPos.ToVector3Shifted() - thing.TrueCenter()).AngleFlat();
                ___turretTopRotation = Mathf.Repeat(angle, 360f);
            }

            return false; // 원래 메서드 실행 방지
        }

        public enum MiningMode
        {
            // Token: 0x0400001B RID: 27
            Ores,
            // Token: 0x0400001C RID: 28
            Rocks,
            // Token: 0x0400001D RID: 29
            OresAndRocks
        }

        public static bool MyisValidTargetAt(object __instance, ref bool __result, IntVec3 position, bool ___designatedOnly,MiningMode ___miningMode)
        {
            // 캐스팅
            var thing = __instance as Thing;
            var map = thing.Map;


            if ( !onSight && !GenSight.LineOfSight(position, position, map, false, null, 0, 0))
            {
                __result = false;
                return false;
            }
            if (___designatedOnly && map.designationManager.DesignationAt(position, DesignationDefOf.Mine) == null)
            {
                __result = false;
                return false;
            }
            Building edifice = position.GetEdifice(map);
            if (edifice == null || !edifice.def.mineable)
            {
                __result = false;
                return false;
            }
            // 여기부터 마무리가 필요해
            if (edifice.def.building.isResourceRock)
            {
                __result= ___miningMode == MiningMode.Ores || ___miningMode == MiningMode.OresAndRocks;
                return false;
            }
            __result = ___miningMode - MiningMode.Rocks <= 1;
            return false; // 원래 메서드 실행 방지
        }
    }
}
