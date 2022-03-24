using BepInEx;
using HarmonyLib;
using JotunnLib.Entities;
using JotunnLib.Managers;
using System;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Shared;
using CheckDmg;
using System.Collections.Generic;
using System.IO;

namespace ViegoSword
{
  

    [BepInPlugin("BladeoftheRuinedKing.Bork", "Blade of the Ruined King", "1.0.0")]
    [BepInDependency(JotunnLib.JotunnLib.ModGuid)]
    public  class Bork : BaseUnityPlugin
    {
        
        private static List<string> resources = new List<string>();
        private static List<string> amount = new List<string>();
        static string[] items = new string[4];
        static  int[] amounts = new int[4];
        static int n;
        static int a = 0;
        static int m = 0;
        private static void Fileread()
        {
           

            List<string> lines = File.ReadAllLines(@"BepInEx\plugins\BorkRecipe.txt").ToList();
            foreach (string line in lines)
            {
                if (!line.IsNullOrWhiteSpace())
                {
                    if (line.IndexOf("Resource:") != -1)
                    {

                        if (!line.Remove(0, 9).Trim().ToString().IsNullOrWhiteSpace())
                        {
                            resources.Add(line.Remove(0, 9).Trim());
                        }
                    }
                    if (line.IndexOf("Amount:") != -1)
                    {
                        if (int.TryParse(line.Remove(0, 7).Trim(), out n) && !line.Remove(0, 7).Trim().ToString().IsNullOrWhiteSpace())
                        {
                            amount.Add(line.Remove(0, 7).Trim());
                        }
                    }
                }
            }
            foreach (string res in resources)
            {
                items[a] = res;
                //Debug.Log($"{items[a]}");
                a += 1;

            }
            foreach (string am in amount)
            {
                amounts[m] = int.Parse(am);
                //Debug.Log($"{amounts[m]}");
                m +=  1;
            }
        }

        private void Awake()
        {
            Fileread();
            PrefabManager.Instance.PrefabRegister += RegisterPrefabs;
            ObjectManager.Instance.ObjectRegister += InitObjects;
        }

        private void RegisterPrefabs(object sender, EventArgs e)
        {
            var Borkbundle = AssetBundleHelper.GetAssetBundleFromResources("bladeoftheruinedking");
            var Bork = Borkbundle.LoadAsset<GameObject>("Assets/CustomItems/ViegoSword/Bladeoftheruinedking.prefab");
            //var swordBlockItemDrop = swordBlock.GetComponent<ItemDrop>();

            //PrefabManager.Instance.RegisterPrefab(swordBlock, "pipislongsword");
            AccessTools.Method(typeof(PrefabManager), "RegisterPrefab", new Type[] { typeof(GameObject), typeof(string) }).Invoke(PrefabManager.Instance, new object[] { Bork, "viegosword" });

        }
        private void InitObjects(object sender, EventArgs e)
        {
            // Add block sword as an item
            ObjectManager.Instance.RegisterItem("viegosword");

            // Add magic armor as an item
            //ObjectManager.Instance.RegisterItem("MagicArmor");

            // Add a sample recipe for the example sword
            ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_viegosword",

                // Name of the prefab for the crafted item
                Item = "viegosword",

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "forge",

                RepairStation = "forge",
                MinStationLevel = 3,

                // List of requirements to craft your item
                Requirements = new PieceRequirementConfig[]
                
                {

                   new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item= items[0],

                        // Amount required
                        Amount = amounts[0]

                    },
                    new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item= items[1],

                        // Amount required
                        Amount = amounts[1]

                    },
                      new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item= items[2],

                        // Amount required
                        Amount = amounts[2]

                    },
                        new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item= items[3],

                        // Amount required
                        Amount = amounts[3]

                    }

                }
            });

        }



    }
}
