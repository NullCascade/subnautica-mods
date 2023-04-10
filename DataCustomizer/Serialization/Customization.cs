using System;
using System.Collections.Generic;
using SMLHelper.V2.Handlers;

namespace DataCustomizer
{
    internal class Customization
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public int Priority { get; set; } = 0;
        public Dictionary<string, DataEntry> Data { get; set; }

        internal class DataEntry
        {
            public int? FragmentsToScan { get; set; }
            public float? FragmentScanTime { get; set; }
            public float? CraftTime { get; set; }
            public string BackgroundType { get; set; }
            public string HarvestType { get; set; }
            public string HarvestOutput { get; set; }
            public int? HarvestFinalCutBonus { get; set; }
            public int[] ItemSize { get; set; }
            public float? BioReactorCharge { get; set; }
            public int? CraftAmount { get; set; }
            public string[] LinkedItems { get; set; }
            public Dictionary<string, int> Ingredients { get; set; }

            public void Apply(string key)
            {
                var techType = Utils.ModUtils.ParseTechType(key);

                if (FragmentsToScan.HasValue)
                {
                    Plugin.Log.LogDebug($"Setting '{key}' to require {FragmentsToScan.Value} fragments.");
                    PDAHandler.EditFragmentsToScan(techType, FragmentsToScan.Value);
                }

                if (FragmentScanTime.HasValue)
                {
                    Plugin.Log.LogDebug($"Setting '{key}' scan time to {FragmentScanTime.Value}.");
                    PDAHandler.EditFragmentScanTime(techType, FragmentScanTime.Value);
                }

                if (CraftTime.HasValue)
                {
                    Plugin.Log.LogDebug($"Setting '{key}' crafting time to {CraftTime.Value}.");
                    CraftDataHandler.SetCraftingTime(techType, CraftTime.Value);
                }

                if (!string.IsNullOrEmpty(BackgroundType))
                {
                    Plugin.Log.LogDebug($"Setting '{key}' background type to {BackgroundType}.");
                    CraftDataHandler.SetBackgroundType(techType, Utils.ModUtils.ParseBackgroundType(BackgroundType));
                }

                if (ItemSize != null)
                {
                    if (ItemSize.Length != 2)
                    {
                        throw new Exception("ItemSize is not an array of two numbers. Must contain exactly two elements.");
                    }
                    Plugin.Log.LogDebug($"Setting '{key}' item size to {ItemSize[0]}x{ItemSize[1]}.");
                    CraftDataHandler.SetItemSize(techType, ItemSize[0], ItemSize[1]);
                }

                if (!string.IsNullOrEmpty(HarvestType))
                {
                    Plugin.Log.LogDebug($"Setting '{key}' harvest type to {HarvestType}.");
                    CraftDataHandler.SetHarvestType(techType, Utils.ModUtils.ParseHarvestType(HarvestType));
                }

                if (!string.IsNullOrEmpty(HarvestOutput))
                {
                    Plugin.Log.LogDebug($"Setting '{key}' harvest output to {HarvestOutput}.");
                    CraftDataHandler.SetHarvestOutput(techType, Utils.ModUtils.ParseTechType(HarvestOutput));
                }

                if (HarvestFinalCutBonus.HasValue)
                {
                    Plugin.Log.LogDebug($"Setting '{key}' harvest final cut bonus to {HarvestFinalCutBonus.Value}.");
                    CraftDataHandler.SetHarvestFinalCutBonus(techType, HarvestFinalCutBonus.Value);
                }

                if (BioReactorCharge.HasValue)
                {
                    Plugin.Log.LogDebug($"Setting '{key}' bio reactor charge to {BioReactorCharge.Value}.");
                    BioReactorHandler.SetBioReactorCharge(techType, BioReactorCharge.Value);
                }

                var techData = CraftDataHandler.GetTechData(techType) ?? new SMLHelper.V2.Crafting.TechData();
                var techDataModified = false;

                if (Ingredients != null && Ingredients.Count > 0)
                {
                    Plugin.Log.LogDebug($"Setting '{key}' crafting recipe...");
                    techData.Ingredients.Clear();
                    foreach (var ingredient in Ingredients)
                    {
                        Plugin.Log.LogDebug($"Adding ingredient {ingredient.Key} x{ingredient.Value}");
                        var ingredientTech = Utils.ModUtils.ParseTechType(ingredient.Key);
                        techData.Ingredients.Add(new SMLHelper.V2.Crafting.Ingredient(ingredientTech, ingredient.Value));
                    }
                    techDataModified = true;
                }

                if (LinkedItems != null)
                {
                    techData.LinkedItems.Clear();
                    foreach (var linkedItem in LinkedItems)
                    {
                        techData.LinkedItems.Add(Utils.ModUtils.ParseTechType(linkedItem));
                    }
                    techDataModified = true;
                }

                if (CraftAmount.HasValue && CraftAmount.Value != techData.craftAmount)
                {
                    Plugin.Log.LogDebug($"Setting '{key}' craft amount to {CraftAmount.Value}");
                    techData.craftAmount = CraftAmount.Value;
                    techDataModified = true;
                }

                if (techDataModified)
                {
                    CraftDataHandler.SetTechData(techType, techData);
                }
            }
        }

        public void ApplyData()
        {
            Plugin.Log.LogDebug($"Applying customization '{Name}' v{Version} by {Author}...");
            foreach (var data in Data)
            {
                try
                {
                    data.Value.Apply(data.Key);
                }
                catch (Exception e)
                {
                    Plugin.Log.LogError($"Could not apply patch to '{data.Key}': {e.Message}");
                }
            }
        }
    }
}
