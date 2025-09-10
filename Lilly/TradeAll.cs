using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Lilly
{
    [StaticConstructorOnStartup]
    public static class TradeAll
    {
        const string HarmonyId = "com.Lilly.TradeAll";
        //static Harmony harmony;

        static TradeAll()
        {
            Log.Warning($"+++ {HarmonyId} loading +++");

            try
            {
                foreach (ThingDef thingDef2 in DefDatabase<ThingDef>.AllDefs.Where(delegate (ThingDef thing)
                   {
                       //ApparelProperties apparel = thing.apparel;
                       if (thing.IsApparel || thing.IsWeapon)
                           return true;
                       return false;
                   }))
                {
                    if (thingDef2.IsApparel)
                    {
                        if (thingDef2.tradeTags == null)
                        {
                            thingDef2.tradeTags = new List<string> { "Clothing" };
                            thingDef2.SetStatBaseValue(StatDefOf.SellPriceFactor, SellModApparelSettings.sellPriceFactor);
                        }
                        else if (!thingDef2.tradeTags.Contains("Clothing") && !thingDef2.tradeTags.Contains("UtilitySpecial") && !thingDef2.tradeTags.Contains("BasicClothing") && !thingDef2.tradeTags.Contains("HoraxArmor") && !thingDef2.tradeTags.Contains("Armor") && !thingDef2.tradeTags.Contains("HiTechArmor") && !thingDef2.tradeTags.Contains("Artifact") && !thingDef2.tradeTags.Contains("ExoticMisc") && !thingDef2.tradeTags.Contains("PsychicApparel"))
                        {
                            thingDef2.tradeTags.Add("Clothing");
                            thingDef2.SetStatBaseValue(StatDefOf.SellPriceFactor, SellModApparelSettings.sellPriceFactor);
                        }
                    }
                    else if (thingDef2.IsWeapon)
                    {
                        if (thingDef2.tradeTags == null)
                        {
                            thingDef2.tradeTags = new List<string> { "WeaponRanged" };
                            thingDef2.SetStatBaseValue(StatDefOf.SellPriceFactor, 0.2f);
                        }
                        else if (!thingDef2.tradeTags.Contains("WeaponRanged") && !thingDef2.tradeTags.Contains("WeaponMelee") && !thingDef2.tradeTags.Contains("ExoticMisc") && !thingDef2.tradeTags.Contains("HoraxWeapon") && !thingDef2.tradeTags.Contains("Clothing") && !thingDef2.tradeTags.Contains("PsychicWeapon"))
                        {
                            thingDef2.tradeTags.Add("WeaponRanged");
                            thingDef2.SetStatBaseValue(StatDefOf.SellPriceFactor, 0.2f);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Log.Error($"+++ {HarmonyId} loaded1 Fail +++");
                Log.Error(e.ToString());
                Log.Error($"+++ {HarmonyId} loaded1 Fail +++");
            }
        }
    }

    public class SellModApparelSettings : ModSettings
    {
        public static float sellPriceFactor = 0.75f;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref sellPriceFactor, "sellPriceFactor", 0.75f);
            base.ExposeData();
        }
    }

    public class SellModApparelMod : Mod
    {
        SellModApparelSettings settings;

        public SellModApparelMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<SellModApparelSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            Rect mainRect = new Rect(0f, 0f, inRect.width, inRect.height);

            listingStandard.Begin(mainRect);
            listingStandard.Gap(50f);
                        
            listingStandard.Label($" {"SellPriceFactor".Translate()}: {Math.Round(SellModApparelSettings.sellPriceFactor, 2)}", -1,tooltip: $"{"SellDefualt".Translate()}: 0.75");
            SellModApparelSettings.sellPriceFactor = listingStandard.Slider((float)Math.Round(SellModApparelSettings.sellPriceFactor, 2), 0f, 2f);
            listingStandard.Label($"  - {"RestartIsRequired".Translate()}");

            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "LetsSellModApparel".Translate();
        }
    }
}
