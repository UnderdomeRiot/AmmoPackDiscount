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

                if (!AmmoPackDiscountConfig.IsValidCurrencyAndAmount || !AmmoPackDiscountConfig.IsValidTemplate)
                {
                    return true;
                }

                var quantityProperty = AccessTools.Property(__instance.GetType(), "Quantity");
                if (quantityProperty == null)
                {
                    return true;
                }

                int quantity = (int)quantityProperty.GetValue(__instance);

                var countField = AccessTools.Field(__instance.GetType(), "Count");
                if (countField == null)
                {
                    return true;
                }

                double count = (double)countField.GetValue(__instance);

                __result = Helpers.Calculation.BulkDiscountStore(Math.Ceiling(count), quantity);
                Plugin.LogSource.LogDebug($"Original RequiredItemsCount result: {AmmoPackDiscountConfig.OriginalPrice}, Final result after discount: {__result}");

                return false;
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"Error intercepting RequiredItemsCount: {ex.Message}"); 
                return true;
            }
        }
    }
}