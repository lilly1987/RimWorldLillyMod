using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Lilly
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HarmonyAction : Attribute
    {

    }

    public abstract class HarmonyBase 
    {
        public static List<HarmonyBase> allHarmonyBases = new List<HarmonyBase>();
        public static Action exposeData;
        public static Action<Rect, Listing_Standard> doSettingsWindowContents;

        public static Settings settings;
        public abstract string label { get;  }
        public abstract string harmonyId { get;  } 
        public virtual Harmony harmony { get; set; } = null;

        public bool onPatch= true;

        public bool onDebug= false;

        public HarmonyBase()
        {
            if (Settings.DebugMode)
                MyLog.Warning($"{this.GetType().Name}.ctor ST {allHarmonyBases.Count}");
            allHarmonyBases.Add(this);
            exposeData += ExposeData;
            doSettingsWindowContents+= DoSettingsWindowContents;
            if (Settings.DebugMode)
                MyLog.Warning($"{this.GetType().Name}.ctor ED {allHarmonyBases.Count}");

        }
        public virtual void DoSettingsWindowContents(Rect inRect, Listing_Standard listing) { }

        /// <summary>
        /// 하모니 패치 필요시 여기에 TogglePatch 호출 넣기
        /// 패치할 내용은 Patch() 에 작성
        /// </summary>
        public virtual void ExposeData() { }


        public virtual void Patch() 
        { 
        
        }

        public void TogglePatch(bool repatch = false)
        {
            Scribe_Values.Look(ref onPatch, label, true);

            if (onPatch)
            {
                if (repatch)
                    UnPatch();
                OnPatch();
            }
            else
                UnPatch();
        }

        public void OnPatch(bool force=false)
        {
            if (harmony != null) return;
            MyLog.Warning($"{harmonyId} Patch ST {Scribe.mode} {force}");
            if (!force && Scribe.mode != LoadSaveMode.Saving)
            {
                if (Settings.DebugMode)
                    MyLog.Warning($"{harmonyId} Patch ED RT");
                return;
            }
            harmony = new Harmony(harmonyId);
            try
            {
                //MyLog.Warning($"{harmonyId} Patch ST");
                Patch();
                if (Settings.DebugMode)
                    MyLog.Warning($"{harmonyId} Patch ED OK");
            }
            catch (Exception e)
            {
                MyLog.Error($"{harmonyId} Patch Fail");
                MyLog.Error(e.ToString());
                MyLog.Error($"{harmonyId} Patch Fail");
                //throw;
                UnPatch();
            }
        }

        public void UnPatch()
        {
            if (harmony == null) return;
            try
            {
                if (Settings.DebugMode)
                    MyLog.Warning($"{harmonyId} UnPatch ST");
                harmony.UnpatchAll(harmonyId);
                harmony = null;
                if (Settings.DebugMode)
                    MyLog.Warning($"{harmonyId} UnPatch ED");
            }
            catch (Exception e)
            {
                MyLog.Error($"{harmonyId} UnPatch Fail");
                MyLog.Error(e.ToString());
                MyLog.Error($"{harmonyId} UnPatch Fail");
                //throw;
            }
        }
    }
}
