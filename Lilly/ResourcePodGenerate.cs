using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace Lilly
{
    // 정상 작동
    [StaticConstructorOnStartup]
    public static class ResourcePodGenerate
    {
        const string HarmonyId = "com.Lilly.ResourcePodGenerate";
        static Harmony harmony;

        static ResourcePodGenerate()
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

        [HarmonyPatch(typeof(ThingSetMaker_ResourcePod), "Generate")]
        public static class Patch_ResourcePodGenerate
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
            {
                Log.Warning($"+++ {HarmonyId} Transpile +++");
                var codes = new List<CodeInstruction>(instructions);
                var newCodes = new List<CodeInstruction>();

                for (int i = 0; i < codes.Count; i++)
                {
                    var instruction = codes[i];
                    try
                    {
                        // 총 시장가치 합계
                        if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 150f)
                        {
                            Log.Warning($"+++ {HarmonyId} succ1 +++");
                            instruction.operand = 10000f;
                        }
                        else if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 600f)
                        {
                            Log.Warning($"+++ {HarmonyId} succ2 +++");
                            instruction.operand = 100000f;
                        }

                        //19  0033    ldc.i4.s    20
                        //20  0035    ldc.i4.s    0x28
                        //21  0037    call int32 Verse.Rand::Range(int32, int32)
                        //22  003C stloc.s V_4(4)

                        // 포드별 갯수 범위
                        else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte sb && sb == 20) // 
                        {
                            Log.Warning($"+++ {HarmonyId} succ3 +++");
                            instruction.opcode = OpCodes.Ldc_I4;
                            instruction.operand = 1000;
                        }
                        else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte sb2 && sb2 == 40)
                        {
                            Log.Warning($"+++ {HarmonyId} succ4 +++");
                            instruction.opcode = OpCodes.Ldc_I4;
                            instruction.operand = 10000;
                        }

                        // 포드 최대 갯수
                        else if (instruction.opcode == OpCodes.Ldc_I4_7)
                        {
                            Log.Warning($"+++ {HarmonyId} succ5 +++");
                            instruction.opcode = OpCodes.Ldc_I4;
                            instruction.operand = 100;
                        }                        
                    }
                    catch (Exception e)
                    {
                        Log.Error($"+++ {HarmonyId} Transpile Fail +++");
                        Log.Error(e.ToString());
                        Log.Error($"+++ {HarmonyId} Transpile Fail +++");
                    }

                    newCodes.Add(instruction);
                    //Log.Warning($"+++ {instruction.opcode} : {instruction.operand} +++");

                    // 종류 다양화
                    if (instruction.opcode == OpCodes.Stloc_3)
                    {
                        Log.Warning($"+++ {HarmonyId} succ6 +++");
                        // thingDef = ThingSetMaker_ResourcePod.RandomPodContentsDef(false);
                        var methodInfo = AccessTools.Method(typeof(ThingSetMaker_ResourcePod), "RandomPodContentsDef");
                        newCodes.Add(new CodeInstruction(OpCodes.Ldc_I4_0)); // false
                        newCodes.Add(new CodeInstruction(OpCodes.Call, methodInfo)); // 메서드 호출
                        newCodes.Add(new CodeInstruction(OpCodes.Stloc_0)); // thingDef 저장 (로컬 변수 0번이라고 가정)
                    }
                }

                return newCodes;

            }
        }

    }
}
