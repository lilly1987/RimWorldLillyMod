using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly
{
    [StaticConstructorOnStartup]
    public static class LillyMod
    {
        const string HarmonyId = "com.Lilly.mod";
        static Harmony harmony;

        static LillyMod()
        {
            Log.Warning($"+++ {HarmonyId} loading +++");
        }
    }
}
