using System;
using HarmonyLib;
using System.Reflection;
using SPT.Reflection.Patching;

using underdomeriot_ammopackdiscount.Config;

namespace underdomeriot_ammopackdiscount.Patches
{
    internal class GClass2288Patch : ModulePatch
    {

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.PropertyGetter(typeof(GClass2288), nameof(GClass2288.RequiredItemsCount));
        }

        [PatchPrefix]
        public static bool Prefix(ref int __result, object __instance)
        {
            try
            {
                Plugin.LogSource.LogWarning("Intercepting RequiredItemsCount...");
                Plugin.LogSource.LogWarning($"AmmoPackDiscountConfig.IsValidCurrencyAndAmount {AmmoPackDiscountConfig.IsValidCurrencyAndAmount}");
                Plugin.LogSource.LogWarning($"AmmoPackDiscountConfig.IsValidTemplate {AmmoPackDiscountConfig.IsValidTemplate}");

                // Check if the currency is valid and template is valid
                if (!AmmoPackDiscountConfig.IsValidCurrencyAndAmount || !AmmoPackDiscountConfig.IsValidTemplate)
                {
                    Plugin.LogSource.LogWarning("Currency or Template is invalid. Skipping logic.");
                    return true; // Let the original method execute
                }

                // Access the Quantity property
                var quantityProperty = AccessTools.Property(__instance.GetType(), "Quantity");
                if (quantityProperty == null)
                {
                    Plugin.LogSource.LogWarning("Quantity property not found.");
                    return true; // Let the original method execute
                }

                int quantity = (int)quantityProperty.GetValue(__instance);

                // Access the Count field
                var countField = AccessTools.Field(__instance.GetType(), "Count");
                if (countField == null)
                {
                    Plugin.LogSource.LogWarning("Count field not found.");
                    return true; // Let the original method execute
                }

                double count = (double)countField.GetValue(__instance);

                __result = Helpers.Calculation.BulkDiscountStore(Math.Ceiling(count), quantity);
                Plugin.LogSource.LogWarning($"Original RequiredItemsCount result: {AmmoPackDiscountConfig.OriginalPrice}, Final result after discount: {__result}");

                return false; // Prevent original method from executing
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"Error intercepting RequiredItemsCount: {ex.Message}"); 
                return true; // Let the original method execute in case of error
            }
        }
    }
}