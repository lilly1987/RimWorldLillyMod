using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly
{
    public class MainSettings : ModSettings
    {
        public bool DebugMode = false;

        public bool forbidUtilitySetForbiddenIs = true;
        public ForbidUtilitySetForbidden forbidUtilitySetForbidden = new ForbidUtilitySetForbidden();
        
        public bool waterBodyFishModIs = true;
        public WaterBodyFishMod waterBodyFishMod = new WaterBodyFishMod();
        public int maxFishPopulation = 1000000;
        public int MeteoriteMineablesCountRangeMin = 8;
        public int MeteoriteMineablesCountRangeMax = 20;

        public bool patch_ASMainTabListIs = true;
        public Patch_ASMainTabList  patch_ASMainTabList = new Patch_ASMainTabList();

        public bool PawnHealthStateDownIs = true;
        public PawnHealthStateDown pawnHealthStateDown = new PawnHealthStateDown();

        public MainSettings()
        {
            Log.Warning($"+++ LillyModSettings ctor +++");
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Log.Warning($"+++ LillyModSettings ExposeData {Scribe.mode} +++");
            if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving)
            {            
                return;
            }

            //
            Scribe_Values.Look(ref DebugMode, "DebugMode", false);

            //
            TogglePatch(forbidUtilitySetForbiddenIs, "waterBodyFishModIs", forbidUtilitySetForbidden);

            //
            TogglePatch(waterBodyFishModIs, "waterBodyFishModIs", waterBodyFishMod);

            Scribe_Values.Look(ref maxFishPopulation, "maxFishPopulation");
            WaterBodyFishMod.All();

            //
            Scribe_Values.Look(ref MeteoriteMineablesCountRangeMin, "MeteoriteMineablesCountRangeMin");
            Scribe_Values.Look(ref MeteoriteMineablesCountRangeMax, "MeteoriteMineablesCountRangeMax");
            MeteoriteMineablesCountRange.SetValue(MeteoriteMineablesCountRangeMin, MeteoriteMineablesCountRangeMax);
            
            //
            TogglePatch(patch_ASMainTabListIs, "patch_ASMainTabList", patch_ASMainTabList);
            
            //
            TogglePatch(PawnHealthStateDownIs, "pawnHealthStateDown", pawnHealthStateDown);
        }

        public void TogglePatch(bool isEnabled,string msg, HarmonyBase patch)
        {
            Scribe_Values.Look(ref isEnabled, msg, true);
            if (isEnabled)
                Patch(patch);
            else
                UnPatch(patch);
        }


        public static void Patch(HarmonyBase lillyHarmonyBase)
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

        public static void UnPatch(HarmonyBase lillyHarmonyBase)
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
