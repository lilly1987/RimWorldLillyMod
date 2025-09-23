using HarmonyLib;
using Verse;

namespace Lilly
{
    // Mod 다음에 StaticConstructorOnStartup 실행
    [StaticConstructorOnStartup]
    public static class Startup
    {
        public static string harmonyId = "Lilly.Startup";
        public static Harmony harmony;

        static Startup()
        {
            MyLog.Warning($"ST {HarmonyBase.allHarmonyBases.Count}");
            //ModUI.settings = ModUI.modUI.GetSettings<MainSettings>();

            //new ForbidUtilitySetForbidden();
            //new WaterBodyFishMod();
            ////new RWAutoSell_ASMainTabList();
            //new PawnHealthStateDown();
            //new ResourcePodGenerate();
            //new DrillTurret_LookForNewTarget();

            if (harmony == null)
            {
                MyLog.Warning($"Patch ST");
                try
                {
                    harmony = new Harmony(harmonyId);
                    harmony.PatchAll();
                }
                catch (System.Exception e)
                {
                    MyLog.Error($"{harmonyId} Patch Fail");
                    MyLog.Error(e.ToString());
                    MyLog.Error($"{harmonyId} Patch Fail");
                }
                MyLog.Warning($"Patch ED");
            }

            foreach (var hb in HarmonyBase.allHarmonyBases)
            {
                hb.OnPatch(true);
            }


            MyLog.Warning($"ED");
        }
    }

    // 설정할때마다 기존에 만든 클래스 사용

}
