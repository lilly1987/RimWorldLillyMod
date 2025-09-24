using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
        public static float drillDamage = 10f;

        public static DrillTurret_LookForNewTarget self;


        public DrillTurret_LookForNewTarget():base()
        {
            self = this;
        }

        public override void DoSettingsWindowContents(Rect inRect, Listing_Standard listing)
        {
            base.DoSettingsWindowContents(inRect, listing);
            
            listing.CheckboxLabeled("시야 무시".Translate(), ref onSight, ".".Translate());            
            ModUI.TextFieldNumeric<float>(listing, ref drillDamage, "드릴 공격력 배율", "");
            
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref onSight, "onSight", true);
            Scribe_Values.Look(ref drillDamage, "drillDamage", 1f);

            TogglePatch(true);
        }

        public override void Patch()
        {
            var harmony = new Harmony(harmonyId);

            // internal 클래스 접근
            drillTurretType = AccessTools.TypeByName("Building_DrillTurret");
            if (drillTurretType == null)
            {
                MyLog.Error("Building_DrillTurret 타입을 찾을 수 없습니다.");
                return;
            }

            MethodInfo lookForNewTarget = AccessTools.Method(drillTurretType, "lookForNewTarget",
                new Type[] { typeof(IntVec3).MakeByRefType() } );

            MyLog.Warning("isValidTargetAt", self.onDebug);
            isValidTargetAt = AccessTools.Method(drillTurretType, "isValidTargetAt", new Type[] { typeof(IntVec3) });

            
            MyLog.Warning("TargetPosition", self.onDebug);
            targetPosProp = AccessTools.Field(drillTurretType, "TargetPosition");

            // miningMode 필드 추출
            MyLog.Warning("miningMode", self.onDebug);
            miningModeField = AccessTools.Field(drillTurretType, "miningMode");

            MyLog.Warning("laserBeamTexture", self.onDebug);
            laserTextureField = AccessTools.Field(drillTurretType, "laserBeamTexture");

            MyLog.Warning("computeDrawingParameters", self.onDebug);
            MethodInfo computeDrawingParameters = AccessTools.Method(drillTurretType, "computeDrawingParameters", new Type[] { });

            MyLog.Warning("drillRock", self.onDebug);
            MethodInfo drillRock = AccessTools.Method(drillTurretType, "drillRock", new Type[] { });

            MyLog.Warning("end", self.onDebug);

            if (lookForNewTarget == null || isValidTargetAt == null || targetPosProp == null || computeDrawingParameters==null|| drillRock == null)
            {
                MyLog.Error($"[DrillTurretPatch] 필요한 메서드 또는 필드를 찾을 수 없습니다.{lookForNewTarget == null}, {isValidTargetAt == null} , {targetPosProp == null}, {computeDrawingParameters == null} ");
                return;
            }

            // Prefix 메서드 정의

            // 패치 적용
            MethodInfo prefixMethod = typeof(DrillTurret_LookForNewTarget).GetMethod(nameof(PreLookForNewTarget), BindingFlags.Static | BindingFlags.Public);
            harmony.Patch(lookForNewTarget, prefix: new HarmonyMethod(prefixMethod));

            MethodInfo prefixMethod2 = typeof(DrillTurret_LookForNewTarget).GetMethod(nameof(MyisValidTargetAt), BindingFlags.Static | BindingFlags.Public);
            harmony.Patch(isValidTargetAt, prefix: new HarmonyMethod(prefixMethod2));

            MethodInfo prefixMethod3 = typeof(DrillTurret_LookForNewTarget).GetMethod(nameof(PreComputeDrawingParameters), BindingFlags.Static | BindingFlags.Public);
            harmony.Patch(computeDrawingParameters, prefix: new HarmonyMethod(prefixMethod3));

            MethodInfo prefixMethod4 = typeof(DrillTurret_LookForNewTarget).GetMethod(nameof(DrillRock), BindingFlags.Static | BindingFlags.Public);
            harmony.Patch(drillRock, prefix: new HarmonyMethod(prefixMethod4));

        }

        public static Type drillTurretType;
        public static MethodInfo isValidTargetAt;
        public static FieldInfo targetPosProp;
        public static FieldInfo miningModeField;
        public static FieldInfo laserTextureField;
        
        public static void DrillRock(ref int ___drillDamageAmount)
        {
            ___drillDamageAmount = (int)(___drillDamageAmount * drillDamage);
            if (self.onDebug)
            MyLog.Warning($"___drillDamageAmount : {___drillDamageAmount}");
        }

        public static bool PreComputeDrawingParameters()
        {
            return false;
        }

        public static bool PreLookForNewTarget(object __instance, out IntVec3 newTargetPosition,ref float ___turretTopRotation,bool ___designatedOnly)
        {
            newTargetPosition = IntVec3.Invalid;

            // 캐스팅
            var thing = __instance as Thing;
            var map = thing.Map;
            var position = thing.Position;

            var miningMode = (MiningMode)miningModeField.GetValue(__instance);

            // 맵 전체 셀 무작위 순회
            bool isValid=false;

            var designated = new List<IntVec3>();
            var fallback = new List<IntVec3>();

            //foreach (var cell in map.AllCells)
            foreach (var cell in DrillCache.cachedRocks[map]
                .OrderBy(b => b.Position.DistanceToSquared(position))
                )
            {
                if (map.designationManager.DesignationAt(cell.Position, DesignationDefOf.Mine) != null)
                    designated.Add(cell.Position);
                else
                    fallback.Add(cell.Position);
            }

            foreach (var cell in designated)
            {
                MyisValidTargetAt(__instance, ref isValid, cell, ___designatedOnly, miningMode);
                if (isValid)
                {
                    newTargetPosition = cell;
                    break;
                }                
            }

            // 지시된 셀에서 못 찾았으면 일반 셀 탐색
            if (!newTargetPosition.IsValid)
            {
                foreach (var cell in fallback)
                {
                    MyisValidTargetAt(__instance, ref isValid, cell, ___designatedOnly, miningMode);
                    if (isValid)
                    {
                        newTargetPosition = cell;
                        break;
                    }
                }
            }

            // 타겟이 유효하면 회전 처리
            if (newTargetPosition.IsValid)
            {
                var targetPos = (IntVec3)targetPosProp.GetValue(__instance);
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
            if (___designatedOnly && 
                map.designationManager.DesignationAt(position, DesignationDefOf.Mine) == null 
                //&& map.designationManager.DesignationAt(position, DesignationDefOf.Deconstruct) == null                 
                )
            {
                __result = false;
                return false;
            }

            Building edifice = position.GetEdifice(map);
            if (edifice == null)
            {
                __result = false;
                return false;
            }

            // 적대 건물 허용
            //bool isHostileStructure = edifice.Faction != null && edifice.Faction.HostileTo(Faction.OfPlayer);

            //if (!edifice.def.mineable && !isHostileStructure)
            if (!edifice.def.mineable )
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
