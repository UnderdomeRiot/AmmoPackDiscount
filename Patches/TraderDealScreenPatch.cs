using System;
using HarmonyLib;
using System.Reflection;
using SPT.Reflection.Patching;
using EFT.UI;
using EFT.InventoryLogic;
using underdomeriot_ammopackdiscount.Config;

namespace underdomeriot_ammopackdiscount.Patches
{
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

                if (!__result)
                {
                    return;
                }

                if ((currency == ECurrencyType.RUB ||
                        currency == ECurrencyType.EUR ||
                        currency == ECurrencyType.USD ||
                        currency == ECurrencyType.GP) &&
                    amount > 0)
                {
                    AmmoPackDiscountConfig.IsValidCurrencyAndAmount = true;
                    Plugin.LogSource.LogDebug($"Valid currency ({currency}) and valid amount ({amount}) detected.");
                }
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"Error intercepting method_18: {ex.Message}");
            }
        }        
    }

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

                if (!AmmoPackDiscountConfig.IsValidCurrencyAndAmount || !AmmoPackDiscountConfig.IsValidTemplate)
                {
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

                Plugin.LogSource.LogDebug($"Intercepted FormatPrice: Input Amount = {amount}, Result = {__result}");

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