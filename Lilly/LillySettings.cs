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
    public class LillySettings : ModSettings
    {
        public static LillySettings settings;
        public static Action harmonyBase ;

        public static bool DebugMode = true;

        public LillySettings()
        {
            Scribe_Values.Look(ref DebugMode, "DebugMode", false);
            if (LillySettings.DebugMode)
                MyLog.Warning($"{this.GetType().Name}.ctor ST");
            settings = this;
            HarmonyBase.settings = settings;

            new ForbidUtilitySetForbidden();
            new WaterBodyFishMod();
            //new RWAutoSell_Patch();
            //new PawnHealthStateDown();
            //new ResourcePodGenerate();
            //new DrillTurret_LookForNewTarget();

            if (LillySettings.DebugMode)
                MyLog.Warning($"{this.GetType().Name}.ctor ED");
        }

        public override void ExposeData()
        {
            base.ExposeData();

            if (LillySettings.DebugMode)
                MyLog.Warning($"{this.GetType().Name}.ExposeData {Scribe.mode} ST");

            //if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving)
            if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving)
            {
                if (LillySettings.DebugMode)
                    MyLog.Warning($"{this.GetType().Name}.ExposeData {Scribe.mode} ED RT");
                return;
            }

            //
            Scribe_Values.Look(ref DebugMode, "DebugMode", false);

            //
            //MeteoriteMineables.ExposeData();
            //DrillCache.ExposeData();

            HarmonyBase.exposeData?.Invoke();

            if (LillySettings.DebugMode)
                MyLog.Warning($"{this.GetType().Name}.ExposeData {Scribe.mode} ED OK");
        }


    }

}
