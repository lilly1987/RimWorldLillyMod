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
    public class PawnHealthStateDown : HarmonyBase
    {
        public static PawnHealthStateDown self;
        public static DesignationDef def;

        public PawnHealthStateDown()
        {
            self= this;

            def=new DesignationDef();
            def.texturePath = "CaptureThemDesignation";
            def.targetType= TargetType.Thing;
        }
        public override string harmonyId => "Lilly.PawnHealthStateDown";

        public override string label => "PawnHealthStateDown";

        public override void DoSettingsWindowContents(Rect inRect, Listing_Standard listing)
        {
            listing.CheckboxLabeled("PawnHealthStateDown 패치".Translate(), ref onPatch, "CaptureThem 모드 필요.".Translate());
            listing.CheckboxLabeled("PawnHealthStateDown debug".Translate(), ref onPatch, ".".Translate());
            listing.GapLine();
        }

        public override void ExposeData()
        {
            TogglePatch();
            Scribe_Values.Look(ref onDebug, "PawnHealthStateDownDebug", false);
        }

        public override void Patch()
        {
            harmony = new Harmony(harmonyId);
            var original = AccessTools.Method(typeof(Pawn_HealthTracker), "CheckForStateChange");
            var Prefix = new HarmonyMethod(typeof(PawnHealthStateDown), nameof(CheckForStateChange));
            //harmony.Patch(original, Prefix);
            original = AccessTools.Method(typeof(Pawn_HealthTracker), "ShouldBeDowned");
            Prefix = new HarmonyMethod(typeof(PawnHealthStateDown), nameof(ShouldBeDowned));
            //harmony.Patch(original, Prefix);
            original = AccessTools.Method(typeof(Pawn_HealthTracker), "MakeDowned");
            Prefix = new HarmonyMethod(typeof(PawnHealthStateDown), nameof(MakeDowned));
            harmony.Patch(original, Prefix);
        }

        static Pawn pawn;

        public static void CheckForStateChange(Pawn ___pawn) //  ,ref bool __result
        {
            if (___pawn == null || pawn != ___pawn) return;
            var pawn_ = ___pawn;
            
                if (PawnHealthStateDown.self.onDebug)
                    Log.Warning($"[+CheckForStateChange+] {pawn_.Name} , {pawn_.Downed} , {!pawn_.InBed()} , {!pawn_.IsPrisonerOfColony} , {pawn_.RaceProps.Humanlike} , {pawn_.Faction.HostileTo(Faction.OfPlayer)}");

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
                if (PawnHealthStateDown.self.onDebug && pawn_.Faction.HostileTo(Faction.OfPlayer))
                    Log.Warning($"[+ShouldBeDowned+] {pawn_.Name} , {pawn_.Downed} , {!pawn_.InBed()} , {!pawn_.IsPrisonerOfColony} , {pawn_.RaceProps.Humanlike} , {pawn_.Faction.HostileTo(Faction.OfPlayer)}");

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
                if (PawnHealthStateDown.self.onDebug) 
                    Log.Warning($"[+MakeDowned+] {pawn_.Name} , {pawn_.Downed} , {!pawn_.InBed()} , {!pawn_.IsPrisonerOfColony} , {pawn_.RaceProps.Humanlike} , {pawn_.Faction.HostileTo(Faction.OfPlayer)}");
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
            //        Log.Warning($"[-MakeDowned-] {pawn_.Name} , {pawn_.Downed} , {!pawn_.InBed()} , {!pawn_.IsPrisonerOfColony} , {pawn_.RaceProps.Humanlike} , {pawn_.Faction.HostileTo(Faction.OfPlayer)}");
            //}
        }
    }
}
