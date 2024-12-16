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
            return AccessTools.Method(typeof(TraderAssortmentControllerClass), nameof(TraderAssortmentControllerClass.SelectItem));
        }

        [PatchPrefix]
        static bool Prefix(Item item)
        {
            try
            {
                AmmoPackDiscountConfig.IsValidTemplate = false;
                Plugin.LogSource.LogWarning("Intercepting SelectItem...");

                var templateProperty = AccessTools.Property(item.GetType(), "Template");
                if (templateProperty == null)
                {
                    Plugin.LogSource.LogWarning("Template property not found.");
                    return true;
                }

                var template = templateProperty.GetValue(item);
                Plugin.LogSource.LogWarning($"Template type: {template?.GetType().FullName ?? "null"}");

                if (template?.GetType().FullName == "EFT.InventoryLogic.AmmoTemplate" ||
                    template?.GetType().FullName == "EFT.InventoryLogic.AmmoBoxTemplate")
                {
                    AmmoPackDiscountConfig.IsValidTemplate = true;
                    Plugin.LogSource.LogWarning($"Valid template ({template?.GetType().FullName}) detected.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"Error intercepting SelectItem: {ex.Message}");
                return true;
            }
        }
    }
}