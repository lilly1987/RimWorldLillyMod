using Verse;

namespace Lilly
{
    // Mod 다음에 StaticConstructorOnStartup 실행
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            MyLog.Warning($"{nameof(Startup)}.ctor ST {HarmonyBase.allHarmonyBases.Count}");
            //ModUI.settings = ModUI.modUI.GetSettings<MainSettings>();

            //new ForbidUtilitySetForbidden();
            //new WaterBodyFishMod();
            ////new RWAutoSell_ASMainTabList();
            //new PawnHealthStateDown();
            //new ResourcePodGenerate();
            //new DrillTurret_LookForNewTarget();
            foreach (var hb in HarmonyBase.allHarmonyBases)
            {
                hb.OnPatch(true);
            }

            MyLog.Warning($"{nameof(Startup)}.ctor ED");
        }
    }

    // 설정할때마다 기존에 만든 클래스 사용

}
