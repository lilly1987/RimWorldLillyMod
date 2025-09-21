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

        public override void DoSettingsWindowContents(Rect inRect, Listing_Standard listing)
        {
            listing.CheckboxLabeled("DrillTurret 범위 무제한 적용 패치".Translate(), ref onPatch, ".".Translate());
            listing.GapLine();
        }

        public override void ExposeData()
        {
            TogglePatch();
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

            MethodInfo targetMethod = AccessTools.Method(drillTurretType, "lookForNewTarget",
                new Type[] { typeof(IntVec3).MakeByRefType() } );

            MyLog.Warning("isValidTargetAt");
            isValidTargetAt = AccessTools.Method(drillTurretType, "isValidTargetAt",
                new Type[] { typeof(IntVec3) });
            MyLog.Warning("TargetPosition");
            targetPosProp = AccessTools.Field(drillTurretType, "TargetPosition");
            //MyLog.Warning("TrueCenter");
            //trueCenterMethod = AccessTools.Method(drillTurretType, "TrueCenter");
            //MyLog.Warning("turretTopRotation");
            //turretTopRotationField = AccessTools.Field(drillTurretType, "turretTopRotation");
            MyLog.Warning("end");

            //if (targetMethod == null || isValidTargetAt == null || targetPosProp == null || trueCenterMethod == null || turretTopRotationField == null)
            if (targetMethod == null || isValidTargetAt == null || targetPosProp == null )
            {
                //MyLog.Error($"[DrillTurretPatch] 필요한 메서드 또는 필드를 찾을 수 없습니다.{targetMethod == null}, {isValidTargetAt == null} , {targetPosProp == null} , { trueCenterMethod == null} , {turretTopRotationField == null}");
                MyLog.Error($"[DrillTurretPatch] 필요한 메서드 또는 필드를 찾을 수 없습니다.{targetMethod == null}, {isValidTargetAt == null} , {targetPosProp == null} ");
                return;
            }

            // Prefix 메서드 정의
            MethodInfo prefixMethod = typeof(DrillTurret_LookForNewTarget).GetMethod("Prefix", BindingFlags.Static | BindingFlags.Public);

            // 패치 적용
            harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefixMethod));

        }

        public static Type drillTurretType;// 
        public static MethodInfo isValidTargetAt;// = AccessTools.Method(drillTurretType, "isValidTargetAt");
        public static FieldInfo targetPosProp;// = AccessTools.Property(drillTurretType, "TargetPosition");
        //public static MethodInfo trueCenterMethod;// = AccessTools.Method(drillTurretType, "TrueCenter");
        //public static FieldInfo turretTopRotationField;// = AccessTools.Field(drillTurretType, "turretTopRotation");

        public static bool Prefix(object __instance, out IntVec3 newTargetPosition,ref float ___turretTopRotation)
        {
            newTargetPosition = IntVec3.Invalid;

            // 캐스팅
            var thing = __instance as Thing;
            var map = thing.Map;

            // 맵 전체 셀 무작위 순회
            foreach (var cell in map.AllCells.InRandomOrder())
            {
                bool isValid = (bool)isValidTargetAt.Invoke(__instance, new object[] { cell });
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

    }
}
