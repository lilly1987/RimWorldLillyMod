using CaptureThem;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions.Must;
using Verse;
using Verse.AI;

namespace Lilly
{
    public class PawnHealthStateDown : HarmonyBase
    {
        public static DesignationDef def;
        public PawnHealthStateDown()
        {
            def=new DesignationDef();
            def.texturePath = "CaptureThemDesignation";
            def.targetType= TargetType.Thing;
        }
        public override string harmonyId => "com.Lilly.PawnHealthStateDown"; 
        public override void Patch()
        {
            harmony = new Harmony(harmonyId);
            var original = AccessTools.Method(typeof(Pawn_HealthTracker), "CheckForStateChange");
            var Prefix = new HarmonyMethod(typeof(PawnHealthStateDown), nameof(Postfix));
            harmony.Patch(original, Prefix);
        }

        public static void Postfix(Pawn ___pawn) //  ,ref bool __result
        {
            //if (!__result) return;
            var pawn = ___pawn;
            if (pawn != null
                && pawn.Downed 
                //&& pawn.Faction != Faction.OfPlayer 
                && !pawn.InBed() 
                && !pawn.IsPrisonerOfColony 
                && pawn.RaceProps.Humanlike
                && pawn.Faction.HostileTo(Faction.OfPlayer)
                )
            {
                if (HarmonyBase.settings.PawnHealthStateDownDebug)
                    Log.Warning($"[+++] {pawn.Name} , {pawn.Downed} , {!pawn.InBed()} , {!pawn.IsPrisonerOfColony} , {pawn.RaceProps.Humanlike} , {pawn.Faction.HostileTo(Faction.OfPlayer)}");
                pawn.Map.designationManager.RemoveAllDesignationsOn(pawn, false);
                pawn.Map.designationManager.AddDesignation(new Designation(pawn, def, null));
                //Job job = JobMaker.MakeJob(JobDefOf.Capture, pawn);
                //pawn.jobs.TryTakeOrderedJob(job);
            } else
            {
                if (HarmonyBase.settings.PawnHealthStateDownDebug)
                    Log.Warning($"[---] {pawn.Name} , {pawn.Downed} , {!pawn.InBed()} , {!pawn.IsPrisonerOfColony} , {pawn.RaceProps.Humanlike} , {pawn.Faction.HostileTo(Faction.OfPlayer)}");
            }
        }
    }
}
