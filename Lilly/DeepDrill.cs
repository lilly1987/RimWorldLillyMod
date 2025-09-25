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
    public static class DeepDrill
    {
        const string HarmonyId = "Lilly.DeepDrill";
        static Harmony harmony;

        public static int cnt = 20000;

        static DeepDrill()
        {
            //return;
            MyLog.Warning($"{HarmonyId} loading");

            harmony = new Harmony(HarmonyId);

            try
            {
                harmony.PatchAll();
                MyLog.Warning($"{HarmonyId} loaded1 succ");
            }
            catch (Exception e)
            {
                MyLog.Error($"{HarmonyId} loaded1 Fail");
                MyLog.Error(e.ToString());
                MyLog.Error($"{HarmonyId} loaded1 Fail");
                //throw;
            }

            //try
            //{                
            //    harmony.Patch(
            //        AccessTools.Method(
            //            typeof(DeepDrillUtility),
            //            "GetNextResource",
            //            new Type[] {
            //                typeof(IntVec3),
            //                typeof(Map),
            //                typeof(ThingDef).MakeByRefType(),
            //                typeof(int).MakeByRefType(),
            //                typeof(IntVec3).MakeByRefType() 
            //            }
            //        )
            //        ,transpiler: new HarmonyMethod(typeof(Patch_DeepDrillUtility_GetNextResource), "Transpiler")
            //    );
            //    MyLog.Warning($"{HarmonyId}2 succ");
            //}
            //catch (Exception e)
            //{
            //    MyLog.Error($"{HarmonyId}2 Fail");
            //    MyLog.Error(e.ToString());
            //    MyLog.Error($"{HarmonyId}2 Fail");
            //    //throw;
            //}

            try
            {
                harmony.Patch(
                    AccessTools.Method(
                        typeof(DeepDrillUtility),
                        "GetNextResource",
                        new Type[] {
                            typeof(IntVec3),
                            typeof(Map),
                            typeof(ThingDef).MakeByRefType(),
                            typeof(int).MakeByRefType(),
                            typeof(IntVec3).MakeByRefType()
                        }
                    )
                    //,transpiler: new HarmonyMethod(typeof(Patch_DeepDrillUtility_GetNextResource2), "Transpiler")
                    , prefix: new HarmonyMethod(typeof(Patch_DeepDrillUtility_GetNextResource3), "Prefix")
                );
                MyLog.Warning($"{HarmonyId} loaded3 succ");
            }
            catch (Exception e)
            {
                MyLog.Error($"{HarmonyId}3 Fail");
                MyLog.Error(e.ToString());
                MyLog.Error($"{HarmonyId}3 Fail");
                //throw;
            }

            MyLog.Warning($"{HarmonyId} end");

        }

        //[HarmonyPatch(
        //    typeof(DeepDrillUtility), nameof(DeepDrillUtility.GetNextResource),
        //    new Type[] {
        //        typeof(IntVec3),
        //        typeof(Map),
        //        typeof(ThingDef).MakeByRefType(),
        //        typeof(int).MakeByRefType(),
        //        typeof(IntVec3).MakeByRefType() 
        //    }
        //)]
        public static class Patch_DeepDrillUtility_GetNextResource
        {


            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var instruction in instructions)
                {
                    // 정수 리터럴 21을 찾아서 7855로 변경
                    if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == 21)
                    {
                        instruction.operand = DeepDrill.cnt;
                        MyLog.Warning($"{HarmonyId} GetNextResource Succ");
                    }
                    yield return instruction;
                }
            }
        }

        // 계속 오류남
        public static class Patch_DeepDrillUtility_GetNextResource2
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                var newCodes = new List<CodeInstruction>();

                for (int i = 0; i < codes.Count; i++)
                {
                    // 패턴: intVec.InBounds(map)
                    if (i + 2 < codes.Count &&
                        codes[i].opcode == OpCodes.Ldloc_1 && // intVec
                        codes[i + 1].opcode == OpCodes.Ldarg_1 && // map
                        codes[i + 2].Calls(typeof(GenRadial).GetMethod("InBounds")))
                    {
                        // 조건 제거: intVec.InBounds(map) → 항상 true 처리
                        // 대신 InBounds 호출을 무시하고 다음 조건으로 넘어감
                        // 즉, 해당 조건을 건너뛰게 만듦

                        // InBounds 호출 제거
                        i += 2;

                        // 대신 항상 true로 처리
                        newCodes.Add(new CodeInstruction(OpCodes.Ldc_I4_1)); // push true
                        MyLog.Warning($"{HarmonyId} GetNextResource2 Succ");
                        continue;
                    }

                    newCodes.Add(codes[i]);
                }
                return newCodes;
            }
        }

        public static class Patch_DeepDrillUtility_GetNextResource3
        {
            [HarmonyPrefix]
            public static bool Prefix(IntVec3 p, Map map, ref ThingDef resDef, ref int countPresent, ref IntVec3 cell, ref bool __result)
            {
                for (int i = 0; i < DeepDrill.cnt; i++) // 반지름 확장
                {
                    IntVec3 intVec = p + GenRadial.RadialPattern[i];

                    // 원래는 InBounds 체크가 있었지만 제거 가능
                    // if (intVec.InBounds(map)) { ... }

                    ThingDef thingDef = map.deepResourceGrid.ThingDefAt(intVec);
                    if (thingDef != null)
                    {
                        resDef = thingDef;
                        countPresent = map.deepResourceGrid.CountAt(intVec);
                        cell = intVec;
                        __result = true;
                        return false; // 원본 메서드 실행 막기
                    }
                }

                resDef = DeepDrillUtility.GetBaseResource(map, p);
                countPresent = int.MaxValue;
                cell = p;
                __result = false;
                return false; // 원본 메서드 실행 막기
            }
        }

        [HarmonyPatch(typeof(CompDeepDrill), "TryProducePortion", new Type[] { typeof(float), typeof(Pawn) })]
        public static class Patch_CompDeepDrill_TryProducePortion
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var instruction in instructions)
                {
                    // 정수 리터럴 21을 찾아서 21000으로 변경
                    if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == 21)
                    {
                        instruction.operand = DeepDrill.cnt;
                        MyLog.Warning($"{HarmonyId} TryProducePortion Succ");
                    }
                    yield return instruction;
                }
            }
        }
    }
}
