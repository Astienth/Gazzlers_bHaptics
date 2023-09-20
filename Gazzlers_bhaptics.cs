using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MyBhapticsTactsuit;
using System.Collections.Generic;

namespace Gazzlers_bhaptics
{
    [BepInPlugin("org.bepinex.plugins.Gazzlers_bhaptics", "Gazzlers_bhaptics integration", "1.0")]
    public class Plugin : BaseUnityPlugin
    {
#pragma warning disable CS0109 // Remove unnecessary warning
        internal static new ManualLogSource Log;
#pragma warning restore CS0109
        public static TactsuitVR tactsuitVr;

        private void Awake()
        {
            // Make my own logger so it can be accessed from the Tactsuit class
            Log = base.Logger;
            // Plugin startup logic
            Logger.LogMessage("Plugin Gazzlers_bhaptics is loaded!");
            tactsuitVr = new TactsuitVR();
            // one startup heartbeat so you know the vest works correctly
            tactsuitVr.PlaybackHaptics("HeartBeat");
            // patch all functions
            var harmony = new Harmony("bhaptics.patch.Gazzlers_bhaptics");
            harmony.PatchAll();
        }
    }
    /*
    [HarmonyPatch(typeof(EntityPlayerLocal), "LateUpdate")]
    public class bhaptics_OnUpdate
    {
        [HarmonyPostfix]
        public static void Postfix(EntityPlayerLocal __instance)
        {
            Plugin.currentHealth = Traverse.Create(__instance).Field("oldHealth").GetValue<float>();
        }
    }
    */
}

