using HarmonyLib;
using Lilly;
using RWASFilterLib;
using RWAutoSell;
using System.Reflection;
using Verse; // 림월드 기본 네임스페이스

//[StaticConstructorOnStartup]
public class Patch_ASMainTabList : HarmonyBase
{
    public override string harmonyId => "com.Lilly.RWAutoSell";

    public override void Patch()
    {
        return;
        var targetType = typeof(ASMainTabList);
        var ctor = AccessTools.Constructor(targetType, new[] { typeof(IRuleComp), typeof(Map) });

        if (ctor != null)
        {
            harmony = new Harmony(harmonyId);
            harmony.Patch(
                original: ctor,
                postfix: new HarmonyMethod(typeof(Patch_ASMainTabList), nameof(Postfix_Ctor))
            );
        }

    }

    public static void Postfix_Ctor(ASMainTabList __instance)
    {
        Log.Warning("[+++] ASMainTabList 생성자 후처리 실행됨");

        // 생성자 이후 Map 값을 강제로 변경
        var mapField = AccessTools.Field(typeof(ASMainTabList), "<Map>k__BackingField");
        if (mapField != null)
        {
            mapField.SetValue(__instance, Find.CurrentMap);
            Log.Warning("[+++] 생성자 후 Map 값 변경 완료");
        }
    }


}