using HarmonyLib;
using Lilly;
//using RWASFilterLib;
//using RWAutoSell;
using System;
using System.Reflection;
using Verse; // 림월드 기본 네임스페이스

//[StaticConstructorOnStartup]
public class RWAutoSell_ASMainTabList : HarmonyBase
{
    public override string harmonyId => "Lilly.RWAutoSell";

    public override string label => throw new System.NotImplementedException();

    public new bool onPatch = false;

    static Type ASMainTabList;

    public override void Patch()
    {
        ASMainTabList = AccessTools.TypeByName("RWAutoSell.ASMainTabList");
        if (ASMainTabList == null)
        {
            Log.Warning("[+++] ASMainTabList 타입을 찾을 수 없습니다.");
            onPatch = false;
            return;
        }
        var IRuleComp = AccessTools.TypeByName("RWASFilterLib.IRuleComp");
        if (IRuleComp == null)
        {
            Log.Warning("[+++] IRuleComp 타입을 찾을 수 없습니다.");
            onPatch = false;
            return;
        }
        var ctor = AccessTools.Constructor(ASMainTabList, new[] { IRuleComp, typeof(Map) });

        if (ctor != null)
        {
            harmony = new Harmony(harmonyId);
            harmony.Patch(
                original: ctor,
                postfix: new HarmonyMethod(typeof(RWAutoSell_ASMainTabList), nameof(Postfix_Ctor))
            );
        }

    }

    public static void Postfix_Ctor(object __instance)
    {
        Log.Warning("[+++] ASMainTabList 생성자 후처리 실행됨");

        // 생성자 이후 Map 값을 강제로 변경
        var mapField = AccessTools.Field(ASMainTabList, "<Map>k__BackingField");
        if (mapField != null)
        {
            mapField.SetValue(__instance, Find.CurrentMap);
            Log.Warning("[+++] 생성자 후 Map 값 변경 완료");
        }
    }


}