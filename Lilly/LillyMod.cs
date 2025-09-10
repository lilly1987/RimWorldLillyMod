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
            
            Log.Warning($"+++ LillyMod end +++");
        }
        public override string SettingsCategory()
        {
            return "- Lill_Mod".Translate();
        }

        string tmp_s;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled("DebugMode", ref settings.DebugMode, "DebugMode.");
            listing.CheckboxLabeled("상호작용 적용".Translate(), ref settings.isPatch, "상호작용 기본값이 활성상태로 기본 적용".Translate());

            listing.End();
        }
    }

    // 설정할때마다 기존에 만든 클래스 사용

}
