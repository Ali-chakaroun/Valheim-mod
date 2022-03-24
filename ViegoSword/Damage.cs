using BepInEx;
using HarmonyLib;
using JotunnLib.Entities;
using JotunnLib.Managers;
using System;
using UnityEngine;
using System.Linq;
using System.Reflection;
using ViegoSword;
namespace CheckDmg
{
    [BepInPlugin("checkdmg.ValheimMod", "Damge mod", "1.0.0")]
    [BepInProcess("valheim.exe")]
    class Damage : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("checkdmg.ValheimMod");

        void Awake()
        {
            harmony.PatchAll();
        }
        public static Character player;
        public static float hp = 0;
        public static bool Primaryattack = false;
        public static bool Secondaryattack = false; 
        public static bool Weaponhit = false;
        public static bool Borkinuse = false;
        public static bool attackqueue = false;
        [HarmonyPatch(typeof(HitData), nameof(HitData.GetTotalDamage))]
       public static class DMG 
        {
          
            static void Postfix( float __result,HitData __instance)
            {
                player = __instance.GetAttacker();

                if (Secondaryattack) {
                    if ((player == Player.m_localPlayer && __result > 0) && Borkinuse)
                    {
                        
                        //Debug.Log($"dealer is: {__instance.m_attacker.m_id}");
                        //Debug.Log($"dmg is: {__result}");
                        float dmg = __result;
                        float percentage = 15;
                        float total = (int)((percentage * dmg) / 100);
                        total = Mathf.Min(total, 120);
                        player.Heal((float)total);
                        //Debug.Log($"healing for: {(float)total}");
                        Secondaryattack = false;
                        Primaryattack = false;
                        Borkinuse = false;

                    }
                }

              
                
            }
        }

        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.StartAttack))]
        class healing5
        {
            static void Postfix(ref Humanoid __instance)
            {
                ItemDrop.ItemData currentWeapon = __instance.GetCurrentWeapon();
                if (currentWeapon.m_shared.m_name == "Blade of the Ruined King" )
                {
                    Borkinuse = true;
                  //  Debug.Log($"is bork in use {Borkinuse}");
                }
 
            }
        }
       
        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.StartAttack))]
        class secondarycheck
        {
            static void Postfix(ref bool ___m_secondaryAttack, ref bool ___m_attack,Humanoid __instance)
            {
                
                    if ((__instance.InAttack() && !__instance.HaveQueuedChain()) || __instance.InDodge() || !__instance.CanMove() || __instance.IsKnockedBack() || __instance.IsStaggering() || __instance.InMinorAction())
                    {
                        attackqueue = true;
                        //Debug.Log($"attacker {__instance.m_name} attackqueue: {attackqueue}");
                    }
                    else
                    {
                        attackqueue = false;
                        //Debug.Log($"attacker {__instance.m_name} attackqueue: {attackqueue}");
                    }
                    if (___m_secondaryAttack && !attackqueue)
                    {

                        Secondaryattack = ___m_secondaryAttack;
                        Primaryattack = false;
                        //  Debug.Log($"using secondary: {___m_secondaryAttack}");
                    }
                    if (___m_attack && !attackqueue)
                    {

                        Primaryattack = ___m_attack;
                        Secondaryattack = false;
                        //  Debug.Log($"using Primary: {___m_attack}");
                    }
            }
              
        }
    }
}
