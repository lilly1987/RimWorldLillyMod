using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Lilly
{
    //[StaticConstructorOnStartup]
    public class LillyMod : Mod
    {
        //const string HarmonyId = "com.Lilly.mod";
        //static Harmony harmony;
        internal LillyModSettings settings;

        public LillyMod(ModContentPack content) : base(content)
        {
            Log.Warning($"+++ LillyMod start +++");
            
            settings = GetSettings<LillyModSettings>();
            LillyHarmonyBase.settings= settings;

            Log.Warning($"+++ LillyMod end +++");
        }
        public override string SettingsCategory()
        {
            return "- Lill_Mod".Translate();
        }

        string tmp;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled("DebugMode", ref settings.DebugMode, "DebugMode.");
            
            listing.CheckboxLabeled("상호작용 활성화 패치".Translate(), ref settings.forbidUtilitySetForbiddenIs, "상호작용 기본값이 활성상태로 기본 적용".Translate());
            listing.GapLine();
            listing.CheckboxLabeled("물고기 패치".Translate(), ref settings.waterBodyFishModIs, "해당 맵에서 사용 가능한 물고기 타입 무조건 적용. 새로운 맵 만들때 적용.".Translate());
            tmp = settings.maxFishPopulation.ToString();
            listing.Label("물고기 최대 개체수 설정".Translate(), tipSignal: "위 패치에 적용. 또는 아래 적용을 눌러야 적용.".Translate());
            listing.TextFieldNumeric(ref settings.maxFishPopulation, ref tmp);
            if (
                listing.ButtonTextLabeled(
                    "모든 맵에 물고기 종류 추가".Translate()
                    , "적용".Translate()
                    ,tooltip: "새로운 맵이 아닌 기존 모든 맵에도 적용. maxFishPopulation = 1000000".Translate()
                )
            ) {
                WaterBodyFishMod.All();
            }
            ;
            if (
                listing.ButtonTextLabeled(
                    "물고기 종류 출력 테스트".Translate()
                    , "디버그출력".Translate()
                    ,tooltip: "테스트".Translate()
                )
            ) {
                WaterBodyFishMod.GetAllFishDefs();
            }
            ;
            listing.GapLine();
            
            listing.End();
        }
    }

    // 설정할때마다 기존에 만든 클래스 사용

}
