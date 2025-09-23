using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static Verse.Grammar.Rule.ConstantConstraint;
using Type = System.Type;

namespace Lilly
{
    public class ForbidUtilitySetForbidden : HarmonyBase
    {
        public override string harmonyId => "Lilly.ForbidUtilitySetForbidden";

        public override string label => "ForbidUtilitySetForbidden";

        //public override void DoSettingsWindowContents(Rect inRect, Listing_Standard listing)
        //{
        //    base.DoSettingsWindowContents(inRect, listing);
        //}

        public override void Patch()
        {
            var original = AccessTools.Method(typeof(ForbidUtility), "SetForbidden");
            var prefix = typeof(ForbidUtilitySetForbidden).GetMethod(nameof(ForceForbid));
            harmony.Patch(original, prefix: new HarmonyMethod(prefix));
        }

        public static bool ForceForbid(
            ref bool value)
        {
            value = false;
            return true; // 계속 원래 메서드 실행
        }
    }
}
