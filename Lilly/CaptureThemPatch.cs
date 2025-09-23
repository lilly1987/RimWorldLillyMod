using CaptureThem;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Verse;
using Verse.AI;

namespace Lilly
{
    public class CaptureThemPatch : HarmonyBase
    {
        public static CaptureThemPatch self;
        public static DesignationDef def;

        public CaptureThemPatch()
        {
            self= this;

            def=new DesignationDef();
            def.texturePath = "CaptureThemDesignation";
            def.targetType= TargetType.Thing;
        }
        public override string harmonyId => "Lilly.CaptureThem";

        public override string label => "CaptureThem";

        public override void DoSettingsWindowContents(Rect inRect, Listing_Standard listing)
        {
            base.DoSettingsWindowContents(inRect, listing);
            //listing.CheckboxLabeled("PawnHealthStateDown 패치".Translate(), ref onPatch, "CaptureThem 모드 필요.".Translate());
        }

        public override void ExposeData()
        {
            base.ExposeData();
            TogglePatch();
        }

        public override void Patch()
        {
            harmony = new Harmony(harmonyId);
            var original = AccessTools.Method(typeof(Pawn_HealthTracker), "CheckForStateChange");
            var Prefix = new HarmonyMethod(typeof(CaptureThemPatch), nameof(CheckForStateChange));
            //harmony.Patch(original, Prefix);
            original = AccessTools.Method(typeof(Pawn_HealthTracker), "ShouldBeDowned");
            Prefix = new HarmonyMethod(typeof(CaptureThemPatch), nameof(ShouldBeDowned));
            //harmony.Patch(original, Prefix);
            original = AccessTools.Method(typeof(Pawn_HealthTracker), "MakeDowned");
            Prefix = new HarmonyMethod(typeof(CaptureThemPatch), nameof(MakeDowned));
            harmony.Patch(original, Prefix);
        }

        static Pawn pawn;

        public static void CheckForStateChange(Pawn ___pawn) //  ,ref bool __result
        {
            if (___pawn == null || pawn != ___pawn) return;
            var pawn_ = ___pawn;
            
                if (CaptureThemPatch.self.onDebug)
                    MyLog.Warning($"[+CheckForStateChange+] {pawn_.Name} , {pawn_.Downed} , {!pawn_.InBed()} , {!pawn_.IsPrisonerOfColony} , {pawn_.RaceProps.Humanlike} , {pawn_.Faction.HostileTo(Faction.OfPlayer)}");

                pawn_.Map.designationManager.AddDesignation(new Designation(pawn_, def, null));

        }
        public static void ShouldBeDowned(Pawn ___pawn) //  ,ref bool __result
        {
            if (___pawn == null) return;
            var pawn_ = ___pawn;
            if (___pawn != null
                //&& pawn_.Downed 
                //&& pawn.Faction != Faction.OfPlayer 
                && !pawn_.InBed()
                && !pawn_.IsPrisonerOfColony
                && pawn_.RaceProps.Humanlike
                && pawn_.Faction.HostileTo(Faction.OfPlayer)
                )
            {
                if (CaptureThemPatch.self.onDebug && pawn_.Faction.HostileTo(Faction.OfPlayer))
                    MyLog.Warning($"[+ShouldBeDowned+] {pawn_.Name} , {pawn_.Downed} , {!pawn_.InBed()} , {!pawn_.IsPrisonerOfColony} , {pawn_.RaceProps.Humanlike} , {pawn_.Faction.HostileTo(Faction.OfPlayer)}");

                pawn = pawn_;
            }
        }
        public static void MakeDowned(Pawn ___pawn) //  ,ref bool __result
        {
            if (___pawn == null) return;
            //if (___pawn == null || pawn != ___pawn) return;
            var pawn_ = ___pawn;
            if (pawn_ != null
                //&& pawn_.Downed 
                //&& pawn.Faction != Faction.OfPlayer 
                && !pawn_.InBed() 
                && !pawn_.IsPrisonerOfColony 
                && pawn_.RaceProps.Humanlike
                && pawn_.Faction.HostileTo(Faction.OfPlayer)
                )
            {
                if (CaptureThemPatch.self.onDebug) 
                    MyLog.Warning($"[+MakeDowned+] {pawn_.Name} , {pawn_.Downed} , {!pawn_.InBed()} , {!pawn_.IsPrisonerOfColony} , {pawn_.RaceProps.Humanlike} , {pawn_.Faction.HostileTo(Faction.OfPlayer)}");
                //pawn_.Map.designationManager.AddDesignation(new Designation(pawn_, def, null));
                //Job job = JobMaker.MakeJob(JobDefOf.Capture, pawn);
                //pawn.jobs.TryTakeOrderedJob(job);
                //pawn = pawn_;
                var a = pawn_.Map?.designationManager;
                if (a != null)
                {
                    a.RemoveAllDesignationsOn(pawn, false);
                    a.AddDesignation(new Designation(pawn_, def, null));
                }
            } 
            //else
            //    pawn = null;
            //{
            //    if (HarmonyBase.settings.PawnHealthStateDownDebug && pawn_.Faction.HostileTo(Faction.OfPlayer))
            //        MyLog.Warning($"[-MakeDowned-] {pawn_.Name} , {pawn_.Downed} , {!pawn_.InBed()} , {!pawn_.IsPrisonerOfColony} , {pawn_.RaceProps.Humanlike} , {pawn_.Faction.HostileTo(Faction.OfPlayer)}");
            //}
        }
    }
}
