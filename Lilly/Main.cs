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
    public class Main : Mod
    {
        //const string HarmonyId = "com.Lilly.mod";
        //static Harmony harmony;
        public static MainSettings settings; 

        public Main(ModContentPack content) : base(content)
        {
            Log.Warning($"+++ LillyMod start +++");
            
            settings = GetSettings<MainSettings>();// 주의. MainSettings의 patch가 먼저 실행됨
            

            Log.Warning($"+++ LillyMod end +++");
        }
        public override string SettingsCategory()
        {
            return "- Lill_Mod".Translate();
        }

        string tmp;
        Vector2 scrollPosition;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            var rect= new Rect(0, 0, inRect.width - 16, 1000);
            Widgets.BeginScrollView(inRect, ref scrollPosition, rect);
            Listing_Standard listing = new Listing_Standard();

            listing.Begin(rect);

            listing.CheckboxLabeled("DebugMode", ref settings.DebugMode, "DebugMode.");
            listing.GapLine();

            listing.CheckboxLabeled("상호작용 활성화 패치".Translate(), ref settings.forbidUtilitySetForbiddenIs, "상호작용 기본값이 활성상태로 기본 적용".Translate());
            listing.GapLine();

            listing.CheckboxLabeled("물고기 패치".Translate(), ref settings.waterBodyFishModIs, "해당 맵에서 사용 가능한 물고기 타입 무조건 적용. 새로운 맵 만들때 적용.".Translate());
            listing.Label("물고기 최대 개체수 설정".Translate(), tipSignal: "위 패치에 적용. 또는 아래 적용을 눌러야 적용.".Translate());
            tmp = settings.maxFishPopulation.ToString();
            listing.TextFieldNumeric(ref settings.maxFishPopulation, ref tmp);
            if (
                listing.ButtonTextLabeled(
                    "모든 맵에 물고기 종류 추가".Translate()
                    , "적용".Translate()
                    , tooltip: "새로운 맵이 아닌 기존 모든 맵에도 적용. maxFishPopulation = 1000000".Translate()
                )
            )
            {
                WaterBodyFishMod.All();
            }
            ;
            if (
                listing.ButtonTextLabeled(
                    "물고기 종류 출력 테스트".Translate()
                    , "디버그출력".Translate()
                    , tooltip: "테스트".Translate()
                )
            )
            {
                WaterBodyFishMod.GetAllFishDefs();
            }
            ;
            listing.GapLine();

            listing.Label("자원 운석 최소 갯수".Translate(), tipSignal: ".".Translate());
            tmp = settings.MeteoriteMineablesCountRangeMin.ToString();
            listing.TextFieldNumeric(ref settings.MeteoriteMineablesCountRangeMin, ref tmp);

            listing.Label("자원 운석 최대 갯수".Translate(), tipSignal: ".".Translate());
            tmp = settings.MeteoriteMineablesCountRangeMax.ToString();
            listing.TextFieldNumeric(ref settings.MeteoriteMineablesCountRangeMax, ref tmp);

            listing.GapLine();

            listing.CheckboxLabeled("PawnHealthStateDown 패치".Translate(), ref settings.PawnHealthStateDownIs, "CaptureThem 모드 필요.".Translate());
            listing.CheckboxLabeled("PawnHealthStateDown debug".Translate(), ref settings.PawnHealthStateDownDebug, ".".Translate());
            listing.GapLine();

            //listing.CheckboxLabeled("RWAutoSell 패치".Translate(), ref settings.patch_ASMainTabListIs, "RWAutoSell.".Translate());
            //listing.GapLine();

            listing.CheckboxLabeled("자원포드 변경 적용 패치".Translate(), ref settings.resourcePodGenerateIs, ".".Translate());
            TextFieldNumeric(listing,ref settings.resourcePodGenerateMin,"총 시장가치 최소");
            TextFieldNumeric(listing,ref settings.resourcePodGenerateMax,"총 시장가치 최대");
            TextFieldNumeric(listing,ref settings.resourcePodGenerateStackMin,"포드별 스택 최소");
            TextFieldNumeric(listing,ref settings.resourcePodGenerateStackMax,"포드별 스택 최대");
            TextFieldNumeric(listing,ref settings.resourcePodGeneratePodMax,"포드 갯수 최대");
            listing.GapLine();

            listing.End();
            Widgets.EndScrollView();

        }

        private void TextFieldNumeric<T>(Listing_Standard listing, ref T num, string label = "", string tipSignal = "") where T : struct
        {
            listing.Label(label.Translate(), tipSignal: tipSignal.Translate());
            tmp = num.ToString();
            listing.TextFieldNumeric<T>(ref num, ref tmp);
        }
    }

    // 설정할때마다 기존에 만든 클래스 사용

}
