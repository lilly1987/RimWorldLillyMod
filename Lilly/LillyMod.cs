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
    [StaticConstructorOnStartup]
    public static class LillyMod
    {
        private const string HarmonyId = "com.Lilly.mod";
        static Harmony harmony;

        static LillyMod()
        {
            Log.Message("+++ LillyMod loading +++");
            try
            {
                harmony = new Harmony(HarmonyId);
                harmony.PatchAll(typeof(LillyPatch) );
            }
            catch (Exception e)
            {
                Log.Error("+++ LillyMod loading Fail +++");
                Log.Error(e.ToString());
                Log.Error("+++ LillyMod loading Fail +++");
                //throw;
            }
        }
    }

    [HarmonyPatch]
    public static class LillyPatch
    {
        [HarmonyPatch(typeof(DeepDrillUtility), nameof(DeepDrillUtility.GetNextResource))]
        public static class Patch_DeepDrillUtility_GetNextResource
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var instruction in instructions)
                {
                    // 정수 리터럴 21을 찾아서 21000으로 변경
                    if (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == 21)
                    {
                        instruction.operand = 10000;
                    }
                    yield return instruction;
                }
            }
        }
        [HarmonyPatch(typeof(CompDeepDrill), "TryProducePortion")]
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
                        instruction.operand = 10000;
                    }
                    yield return instruction;
                }
            }
        }

    }
}
