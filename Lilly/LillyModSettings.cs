using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly
{
    public class LillyModSettings : ModSettings
    {
        public bool DebugMode = false;
        public bool isPatch = true;
        public ForbidUtilitySetForbidden forbidUtilitySetForbidden = new ForbidUtilitySetForbidden();

        public LillyModSettings()
        {
            Log.Warning($"+++ LillyModSettings ctor +++");
        }

        public override void ExposeData()
        {
            Log.Warning($"+++ LillyModSettings ExposeData +++");
            base.ExposeData();
            Scribe_Values.Look(ref DebugMode, "DebugMode", false);
            Scribe_Values.Look(ref isPatch, "isPatch", true);
            if (isPatch)
                Patch(forbidUtilitySetForbidden);
            else
                UnPatch(forbidUtilitySetForbidden);
        }

        public static void Patch(LillyHarmonyBase lillyHarmonyBase)
        {
            if (lillyHarmonyBase.harmony != null) return;

            lillyHarmonyBase.harmony = new Harmony(lillyHarmonyBase.harmonyId);
            try
            {
                lillyHarmonyBase.Patch();
                Log.Warning($"+++ {lillyHarmonyBase.harmonyId} Patch succ +++");
            }
            catch (Exception e)
            {
                Log.Error($"+++ {lillyHarmonyBase.harmonyId} Patch Fail +++");
                Log.Error(e.ToString());
                Log.Error($"+++ {lillyHarmonyBase.harmonyId} Patch Fail +++");
                //throw;
            }
        }

        public static void UnPatch(LillyHarmonyBase lillyHarmonyBase)
        {
            if (lillyHarmonyBase.harmony == null) return;
            try
            {
                lillyHarmonyBase.harmony.UnpatchAll(lillyHarmonyBase.harmonyId);
                lillyHarmonyBase.harmony = null;
                Log.Warning($"+++ {lillyHarmonyBase.harmonyId} UnPatch succ +++");
            }
            catch (Exception e)
            {
                Log.Error($"+++ {lillyHarmonyBase.harmonyId} UnPatch Fail +++");
                Log.Error(e.ToString());
                Log.Error($"+++ {lillyHarmonyBase.harmonyId} UnPatch Fail +++");
                //throw;
            }
        }
    }

}
