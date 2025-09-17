using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lilly
{
    public abstract class HarmonyBase
    {
        public static MainSettings settings;
        public virtual string harmonyId { get; set; } = "com.Lilly.Mod";
        public virtual Harmony harmony { get; set; } = null;
        public abstract void Patch();
    }
}
