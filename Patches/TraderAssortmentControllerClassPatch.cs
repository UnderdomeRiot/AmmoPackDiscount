using System;
using HarmonyLib;
using System.Reflection;
using SPT.Reflection.Patching;
using EFT.InventoryLogic;
using underdomeriot_ammopackdiscount.Config;

namespace underdomeriot_ammopackdiscount.Patches
{
    internal class TraderAssortmentControllerClassPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TraderAssortmentControllerClass), nameof(TraderAssortmentControllerClass.SelectItem), new[] { typeof(Item) });
        }

        [PatchPrefix]
        static bool Prefix(Item item)
        {
            try
            {
                AmmoPackDiscountConfig.IsValidTemplate = false;

                var templateProperty = AccessTools.Property(item.GetType(), "Template");
                if (templateProperty == null)
                {
                    return true;
                }

                var template = templateProperty.GetValue(item);

                if (template?.GetType().FullName == "EFT.InventoryLogic.AmmoTemplate")
                {
                    AmmoPackDiscountConfig.IsValidTemplate = true;
                    Plugin.LogSource.LogDebug($"Valid template ({template?.GetType().FullName}) detected.");
                }

                return true;
            }
            catch
            {
                return true;
            }
        }
    }
}