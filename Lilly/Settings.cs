using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly
{
    public class Settings : ModSettings
    {
        public static Settings settings;
        public static Action harmonyBase ;

        public bool DebugMode = false;

        public Settings()
        {
            Log.Warning($"+++ Lilly {this.GetType().Name}.ctor ST +++");
            settings = this;
            HarmonyBase.settings = settings;

            new ForbidUtilitySetForbidden();
            new WaterBodyFishMod();
            //new RWAutoSell_ASMainTabList();
            new PawnHealthStateDown();
            new ResourcePodGenerate();
            new DrillTurret_LookForNewTarget();

            Log.Warning($"+++ Lilly {this.GetType().Name}.ctor ED +++");
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Log.Warning($"+++ {this.GetType().Name}.ExposeData {Scribe.mode} ST +++");
            //if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving)
            if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving)
            {
                Log.Warning($"+++ {this.GetType().Name}.ExposeData {Scribe.mode} ED 1 +++");
                return;
            }

            //
            Scribe_Values.Look(ref DebugMode, "DebugMode", false);

            //
            MeteoriteMineables.ExposeData();

            HarmonyBase.exposeData?.Invoke();

            Log.Warning($"+++ {this.GetType().Name}.ExposeData {Scribe.mode} ED 2 +++");
        }


    }

}
