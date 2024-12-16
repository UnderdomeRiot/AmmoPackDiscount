using System;
using HarmonyLib;
using System.Reflection;
using SPT.Reflection.Patching;
using EFT.UI;
using EFT.InventoryLogic;
using underdomeriot_ammopackdiscount.Config;

namespace underdomeriot_ammopackdiscount.Patches
{
    // Check currency to unable bartering.
    internal class Method18Patch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TraderDealScreen), nameof(TraderDealScreen.method_18));
        }

        [PatchPostfix]
        static void Postfix(bool __result, ECurrencyType currency, int amount)
        {
            try
            {
                AmmoPackDiscountConfig.IsValidCurrencyAndAmount = false;
                Plugin.LogSource.LogWarning("Intercepting TraderDealScreen.method_18...");

                if (!__result)
                {
                    Plugin.LogSource.LogWarning("TraderDealScreen.method_18 returned false. Skipping validation.");
                    return;
                }

                if ((currency == ECurrencyType.RUB ||
                        currency == ECurrencyType.EUR ||
                        currency == ECurrencyType.USD ||
                        currency == ECurrencyType.GP) &&
                    amount > 0)
                {
                    AmmoPackDiscountConfig.IsValidCurrencyAndAmount = true;
                    Plugin.LogSource.LogWarning($"Valid currency ({currency}) and valid amount ({amount}) detected.");
                }
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"Error intercepting method_18: {ex.Message}");
            }
        }        
    }

    // Modifying the UI string to make it a bit more readable
    internal class FormatPricePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TraderDealScreen), nameof(TraderDealScreen.FormatPrice));
        }

        [PatchPrefix]
        public static bool Prefix(ref string __result, int amount)
        {
            try
            {
                Plugin.LogSource.LogWarning("Intercepting TraderDealScreen.FormatPrice...");

                if (!AmmoPackDiscountConfig.IsValidCurrencyAndAmount || !AmmoPackDiscountConfig.IsValidTemplate)
                {
                    Plugin.LogSource.LogWarning("Currency or Template is invalid. Skipping logic.");
                    return true;
                }

                if (amount >= 0)
                {
                    __result = $"{amount.FormatSeparate(" ")} ({AmmoPackDiscountConfig.TotalDiscountPercentApplied}%)";
                }
                else
                {
                    __result = "A LOT".Localized(null);
                }

                Plugin.LogSource.LogWarning($"Intercepted FormatPrice: Input Amount = {amount}, Result = {__result}");

                return false;
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"Error intercepting FormatPrice: {ex.Message}");
                return true;
            }
        }
    }
}