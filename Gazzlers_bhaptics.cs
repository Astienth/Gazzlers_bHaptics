using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MyBhapticsTactsuit;
using System.Collections.Generic;
using Banzai.GAZZLERS;

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
    
    [HarmonyPatch(typeof(PlayerHealth), "ApplyDamage")]
    public class bhaptics_ApplyDamage
    {
        [HarmonyPostfix]
        public static void Postfix(PlayerHealth __instance, PlayerDamageType damageType)
        {
            switch (damageType)
            {
                case PlayerDamageType.Bullet:
                    Plugin.tactsuitVr.PlaybackHaptics("Impact", true, 1f, 1f);
                    Plugin.tactsuitVr.PlaybackHaptics("ShotVisor");
                    break;
                case PlayerDamageType.EnergyWave:
                    Plugin.tactsuitVr.PlaybackHaptics("Impact", true, 2f, 1.5f);
                    Plugin.tactsuitVr.PlaybackHaptics("ShotVisor");
                    break;
                case PlayerDamageType.Rocket:
                    Plugin.tactsuitVr.PlaybackHaptics("Impact", true, 4f, 2f); 
                    Plugin.tactsuitVr.PlaybackHaptics("ExplosionFace");
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerHealth), "ApplyHealWithSound")]
    public class bhaptics_Heal
    {
        [HarmonyPostfix]
        public static void Postfix(PlayerHealth __instance)
        {
            Plugin.tactsuitVr.PlaybackHaptics("Heal");
        }
    }

    [HarmonyPatch(typeof(PlayerHealth), "EnableLowHealthFeedback")]
    public class bhaptics_EnableLowHealthFeedback
    {
        [HarmonyPostfix]
        public static void Postfix(PlayerHealth __instance)
        {            
            Plugin.tactsuitVr.StartHeartBeat();
        }
    }

    [HarmonyPatch(typeof(PlayerHealth), "DisableLowHealthFeedback")]
    public class bhaptics_DisableLowHealthFeedback
    {
        [HarmonyPostfix]
        public static void Postfix(PlayerHealth __instance)
        {
            Plugin.tactsuitVr.StopHeartBeat();
        }
    }

    [HarmonyPatch(typeof(PlayerHealth), "IsDead")]
    public class bhaptics_IsDead
    {
        public static bool died = false;

        [HarmonyPostfix]
        public static void Postfix(PlayerHealth __instance, bool __result)
        {
            if(__result && !died)
            {
                died = true;
                Plugin.tactsuitVr.PlaybackHaptics("Death");
            }

            if (!__result)
            {
                died = false;
            }
            else
            {
                Plugin.tactsuitVr.StopAllHapticFeedback();
                Plugin.tactsuitVr.StopThreads();
            }
        }
    }
    
    [HarmonyPatch(typeof(Reloader), "Reload")]
    public class bhaptics_ReloadWeapon
    {
        [HarmonyPostfix]
        public static void Postfix(Reloader __instance)
        {
            Plugin.tactsuitVr.PlaybackHaptics("RecoilArm_R", true, 0.5f);
            Plugin.tactsuitVr.PlaybackHaptics("RecoilVest_R", true, 0.5f);
        }
    }

    [HarmonyPatch(typeof(PlayerWeapon), "OnShootEvent")]
    public class bhaptics_OnShootEvent
    {
        [HarmonyPostfix]
        public static void Postfix(PlayerWeapon __instance)
        {
            if (__instance.curHand == HAND.RIGHT)
            {
                Plugin.tactsuitVr.PlaybackHaptics("RecoilArm_R");
                Plugin.tactsuitVr.PlaybackHaptics("RecoilVest_R");
            }
            else
            {
                Plugin.tactsuitVr.PlaybackHaptics("RecoilArm_L");
                Plugin.tactsuitVr.PlaybackHaptics("RecoilVest_L");
            }
        }
    }

    [HarmonyPatch(typeof(ShieldCollider), "Hit")]
    public class bhaptics_PlayerProjectile
    {
        [HarmonyPostfix]
        public static void Postfix(ShieldCollider __instance)
        {
            Plugin.tactsuitVr.PlaybackHaptics("RecoilArm_L");
            Plugin.tactsuitVr.PlaybackHaptics("RecoilVest_L");
        }
    }
    
    [HarmonyPatch(typeof(ShieldCanvas), "OnEnergyChanged")]
    public class bhaptics_ShieldCanvas
    {
        [HarmonyPostfix]
        public static void Postfix(ShieldCanvas __instance, float currentEnergy, float maxEnergy)
        {
            if(currentEnergy <= 0)
            {
                Plugin.tactsuitVr.PlaybackHaptics("ShieldKOArmsL");
                Plugin.tactsuitVr.PlaybackHaptics("ShieldKOVest");
            }
        }
    }
    
    [HarmonyPatch(typeof(UtilityHand), "OpenShield")]
    public class bhaptics_OpenShield
    {
        [HarmonyPostfix]
        public static void Postfix(UtilityHand __instance)
        {
            Plugin.tactsuitVr.StartShield();
        }
    }

    [HarmonyPatch(typeof(UtilityHand), "DeactivateShield")]
    public class bhaptics_DeactivateShield
    {
        [HarmonyPostfix]
        public static void Postfix(UtilityHand __instance)
        {
            Plugin.tactsuitVr.StopShield();
        }
    }
    
    [HarmonyPatch(typeof(UtilityHand), "ExitShieldOverheat")]
    public class bhaptics_ExitShieldOverheat
    {
        [HarmonyPostfix]
        public static void Postfix(UtilityHand __instance)
        {
            Plugin.tactsuitVr.PlaybackHaptics("ShieldOKArmsL");
            Plugin.tactsuitVr.PlaybackHaptics("ShieldOKVest");
        }
    }
}

