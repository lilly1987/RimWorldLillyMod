using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Lilly
{
    public class PawnHealthStateDown : HarmonyBase
    {
        public override string harmonyId => "com.Lilly.PawnHealthStateDown"; 
        public override void Patch()
        {
            harmony = new Harmony(harmonyId);
            var original = AccessTools.Method(typeof(Pawn_HealthTracker), "MakeDowned");
            var Prefix = new HarmonyMethod(typeof(PawnHealthStateDown), nameof(Postfix));
            harmony.Patch(original, Prefix);
        }

        public static void Postfix(Pawn ___pawn)
        {
            var pawn = ___pawn;
            if(HarmonyBase.settings.DebugMode)
                Log.Warning($"[+++] {pawn.Name} , {pawn.Downed} , {!pawn.InBed()} , {!pawn.IsPrisonerOfColony} , {pawn.RaceProps.Humanlike} , {pawn.Faction.HostileTo(Faction.OfPlayer)}");
            if (pawn != null 
                && pawn.Downed 
                //&& pawn.Faction != Faction.OfPlayer 
                && !pawn.InBed() 
                && !pawn.IsPrisonerOfColony 
                && pawn.RaceProps.Humanlike
                && pawn.Faction.HostileTo(Faction.OfPlayer)
                )
            {
                Job job = JobMaker.MakeJob(JobDefOf.Capture, pawn);
                pawn.jobs.TryTakeOrderedJob(job);
            }
        }
    }
}
