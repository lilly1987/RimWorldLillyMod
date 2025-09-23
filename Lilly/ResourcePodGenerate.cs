using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace Lilly
{
    // 정상 작동
    public class ResourcePodGenerate : HarmonyBase
    {
        public override string harmonyId => "Lilly.ResourcePodGenerate";

        public override string label => "ResourcePodGenerate";

        public static float resourcePodGenerateMin = 10000f;
        public static float resourcePodGenerateMax = 1000000f;
        public static int resourcePodGenerateStackMin = 100;
        public static int resourcePodGenerateStackMax = 10000;
        public static int resourcePodGeneratePodMax = 100;

        public static ResourcePodGenerate self = null;

        public ResourcePodGenerate() : base()
        {
            ResourcePodGenerate.self = this;
        }

        public override void DoSettingsWindowContents(Rect inRect, Listing_Standard listing)
        {
            base.DoSettingsWindowContents(inRect, listing);
            
            ModUI.TextFieldNumeric(listing, ref resourcePodGenerateMin, "총 시장가치 최소");
            ModUI.TextFieldNumeric(listing, ref resourcePodGenerateMax, "총 시장가치 최대");
            ModUI.TextFieldNumeric(listing, ref resourcePodGenerateStackMin, "포드별 스택 최소");
            ModUI.TextFieldNumeric(listing, ref resourcePodGenerateStackMax, "포드별 스택 최대");
            ModUI.TextFieldNumeric(listing, ref resourcePodGeneratePodMax, "포드 갯수 최대");
            
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref resourcePodGenerateMin, "resourcePodGenerateMin");
            Scribe_Values.Look(ref resourcePodGenerateMax, "resourcePodGenerateMax");
            Scribe_Values.Look(ref resourcePodGenerateStackMin, "resourcePodGenerateStackMin");
            Scribe_Values.Look(ref resourcePodGenerateStackMax, "resourcePodGenerateStackMax");
            Scribe_Values.Look(ref resourcePodGeneratePodMax, "resourcePodGeneratePodMax");
            TogglePatch();
        }

        public override void Patch()
        {            
            harmony = new Harmony(harmonyId);
            var original = AccessTools.Method(typeof(ThingSetMaker_ResourcePod), "Generate"
                , new Type[] { typeof(ThingSetMakerParams) ,typeof(List<Thing>) });
            var transpiler = new HarmonyMethod(typeof(ResourcePodGenerate), nameof(Transpile));
            harmony.Patch(original, transpiler: transpiler);
        }

        public static string Id = "ResourcePodGenerate";

        public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
        {
            MyLog.Warning($"{Id} Transpile");
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
                        if (ResourcePodGenerate.self.onDebug)
                            MyLog.Warning($"{Id} succ1");
                        //instruction.operand = 10000f;
                        instruction.operand = resourcePodGenerateMin;
                    }
                    else if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 600f)
                    {
                        if (ResourcePodGenerate.self.onDebug)
                            MyLog.Warning($"{Id} succ2");
                        instruction.operand = resourcePodGenerateMax;
                    }

                    // 포드별 갯수 범위
                    else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte sb && sb == 20) // 
                    {
                        if (ResourcePodGenerate.self.onDebug)
                            MyLog.Warning($"{Id} succ3");
                        instruction.opcode = OpCodes.Ldc_I4;
                        instruction.operand = resourcePodGenerateStackMin;
                    }
                    else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte sb2 && sb2 == 40)
                    {
                        if (ResourcePodGenerate.self.onDebug)
                            MyLog.Warning($"{Id} succ4");
                        instruction.opcode = OpCodes.Ldc_I4;
                        instruction.operand = resourcePodGenerateStackMax;
                    }

                    // 포드 최대 갯수
                    else if (instruction.opcode == OpCodes.Ldc_I4_7)
                    {
                        if (ResourcePodGenerate.self.onDebug)
                            MyLog.Warning($"{Id} succ5");
                        instruction.opcode = OpCodes.Ldc_I4;
                        instruction.operand = resourcePodGeneratePodMax;
                    }                        
                }
                catch (Exception e)
                {
                    MyLog.Error($"{Id} Transpile Fail");
                    MyLog.Error(e.ToString());
                    MyLog.Error($"{Id} Transpile Fail");
                }

                newCodes.Add(instruction);
                //MyLog.Warning($"{instruction.opcode} : {instruction.operand}");

                // 종류 다양화
                if (instruction.opcode == OpCodes.Stloc_3)
                {
                    if (ResourcePodGenerate.self.onDebug)
                        MyLog.Warning($"{Id} succ6");
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
