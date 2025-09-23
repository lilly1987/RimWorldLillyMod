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
    // Mod 다음에 StaticConstructorOnStartup 실행
    public class ModUI : Mod
    {
        public static Settings settings; 
        public static ModUI modUI;

        // LoadingVars ResolvingCrossRefs PostLoadInit
        public ModUI(ModContentPack content) : base(content)
        {
            MyLog.Warning($"{this.GetType().Name}.ctor ST");

            modUI = this;
            settings = GetSettings<Settings>();// 주의. MainSettings의 patch가 먼저 실행됨            

            MyLog.Warning($"{this.GetType().Name}.ctor ED");
        }

        public override string SettingsCategory()
        {
            return "- Lill_Mod".Translate();
        }

        
        Vector2 scrollPosition;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            var rect= new Rect(0, 0, inRect.width - 16, 1000);
            Widgets.BeginScrollView(inRect, ref scrollPosition, rect);
            Listing_Standard listing = new Listing_Standard();

            listing.Begin(rect);

            listing.CheckboxLabeled("DebugMode", ref Settings.DebugMode, ".");
            listing.GapLine();

            listing.CheckboxLabeled("DrillCache Debug", ref DrillCache.DebugMode, ".");
            listing.GapLine();

            MeteoriteMineables.DoSettingsWindowContents(inRect, listing);

            HarmonyBase.doSettingsWindowContents?.Invoke(inRect, listing);

            listing.End();
            Widgets.EndScrollView();

        }

        static string tmp;

        public static void TextFieldNumeric<T>(Listing_Standard listing, ref T num, string label = "", string tipSignal = "") where T : struct
        {
            listing.Label(label.Translate(), tipSignal: tipSignal.Translate());
            tmp = num.ToString();
            listing.TextFieldNumeric<T>(ref num, ref tmp);
        }
    }

    // 설정할때마다 기존에 만든 클래스 사용

}
