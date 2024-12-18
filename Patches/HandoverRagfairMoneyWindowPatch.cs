using HarmonyLib;
using System.Reflection.Emit;
using EFT.UI.Ragfair;
using System.Collections.Generic;
using UnityEngine;
using EFT.InventoryLogic;
using System.Reflection;
using underdomeriot_ammopackdiscount.Config;
using System;
using SPT.Reflection.Patching;

namespace underdomeriot_ammopackdiscount.Patches
{

    internal class PurchasePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(RagFairClass), "Purchase");
        }

        [PatchPrefix]
        static bool Prefix(CustomizationClass purchases)
        {
            try
            {
                if (AmmoPackDiscountConfig.IsValidTemplate && AmmoPackDiscountConfig.IsValidTrader)
                {
                    foreach (GClass2083 item in purchases)
                    {
                        foreach (GClass2079 subItem in item.Items)
                        {
                            Plugin.LogSource.LogDebug($"Modifying ExistingCount of {subItem.Id} from {subItem.ExistingCount} to {AmmoPackDiscountConfig.FinalPrice}");
                            subItem.ExistingCount = AmmoPackDiscountConfig.FinalPrice;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"Error intercepting Purchase: {ex.Message}");
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(HandoverRagfairMoneyWindow), "Show")]
    public static class HandoverRagfairMoneyWindowShowPatch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode != null && codes[i].operand != null)
                {
                    if (codes[i].opcode == OpCodes.Stfld
                        && codes[i].operand.ToString().Contains("EFT.InventoryLogic.Item[] item_0"))
                    {

                        Debug.Log($"[Transpiler Show] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");
                        Debug.Log($"[Transpiler Show] Initiating Modifying instruction at index {i}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldarg_0));
                        Debug.Log($"[Transpiler Show] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldfld, typeof(HandoverRagfairMoneyWindow).GetField("ememberCategory_0", BindingFlags.Instance | BindingFlags.NonPublic)));
                        Debug.Log($"[Transpiler Show] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, typeof(ValidTrader).GetMethod("CheckTrader", new[] { typeof(EMemberCategory) })));
                        Debug.Log($"[Transpiler Show] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        Debug.Log($"[Transpiler Show] Ending Modifying instruction at index {i}");

                        i++;

                    }
                }
                Debug.Log($"[Transpiler Show] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");
            }

            return codes;

        }
    }

    [HarmonyPatch(typeof(HandoverRagfairMoneyWindow), "method_4")] 
    public static class HandoverRagfairMoneyWindowMethod4Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {

            var codes = new List<CodeInstruction>(instructions);
            var discount = generator.DeclareLocal(typeof(string)).LocalIndex;

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode != null && codes[i].operand != null)
                {

                    if (codes[i].opcode == OpCodes.Stloc_S && codes[i].operand.ToString().Contains("System.String (31)"))
                    {
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");
                        Debug.Log($"[Transpiler Method4] Initiating modification at index {i}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloca_S, 33));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, typeof(KeyValuePair<GInterface195, int>).GetProperty("Key").GetGetMethod()));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Callvirt, typeof(GInterface195).GetProperty("Item").GetGetMethod()));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Callvirt, typeof(Item).GetProperty("Template").GetGetMethod()));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Callvirt, typeof(object).GetMethod("GetType")));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Callvirt, typeof(System.Type).GetProperty("FullName").GetGetMethod()));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, typeof(ValidTemplate).GetMethod("CheckTemplate", new[] { typeof(string) })));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        Label skipValidationLabel1 = generator.DefineLabel();
                        Label skipValidationLabel2 = generator.DefineLabel();
                        
                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldsfld, typeof(AmmoPackDiscountConfig).GetField("IsValidTrader", BindingFlags.Static | BindingFlags.Public)));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Brfalse_S, skipValidationLabel1));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldsfld, typeof(AmmoPackDiscountConfig).GetField("IsValidTemplate", BindingFlags.Static | BindingFlags.Public)));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Brfalse_S, skipValidationLabel2));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 29));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 31));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, typeof(Helpers.Calculation).GetMethod("BulkDiscountRagfair", new[] { typeof(int), typeof(string) })));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        var tupleLocalIndex = generator.DeclareLocal(typeof(ValueTuple<int, string>)).LocalIndex;

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stloc, tupleLocalIndex));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloca, tupleLocalIndex));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldfld, typeof(ValueTuple<int, string>).GetField("Item1")));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stloc, 29));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloca, tupleLocalIndex));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldfld, typeof(ValueTuple<int, string>).GetField("Item2")));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stloc, discount));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes[i].labels.Add(skipValidationLabel2);
                        codes[i].labels.Add(skipValidationLabel1);

                        Debug.Log($"[Transpiler Method4] Ending Modifying instruction at index {i}");

                    }
                    
                    if (codes[i].opcode == OpCodes.Ldstr && codes[i].operand.ToString().Contains("ragfair/Are you sure you want to buy {0} ({1}) for {2}?"))
                    {

                        Debug.Log($"[Transpiler Method4] Initiating modification at index {i}");

                        Label skipValidationLabel1 = generator.DefineLabel();
                        Label skipValidationLabel2 = generator.DefineLabel();
                        Label skipValidationLabel3 = generator.DefineLabel();

                        codes.Insert(i, new CodeInstruction(OpCodes.Ldsfld, typeof(AmmoPackDiscountConfig).GetField("IsValidTrader", BindingFlags.Static | BindingFlags.Public)));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Brfalse_S, skipValidationLabel1));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldsfld, typeof(AmmoPackDiscountConfig).GetField("IsValidTemplate", BindingFlags.Static | BindingFlags.Public)));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Brfalse_S, skipValidationLabel2));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldstr, "Are you sure you want to buy {0} ({1})({3}) for {2}?"));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, 4));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Newarr, typeof(string)));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Dup));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, 0));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 30));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stelem_Ref));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Dup));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, 1));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 31));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stelem_Ref));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Dup));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, 2));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 32));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stelem_Ref));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}"); 
                        
                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Dup));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, 3));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc, discount));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stelem_Ref));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, typeof(string).GetMethod("Format", new[] { typeof(string), typeof(string[]) })));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stloc, 2));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Br, skipValidationLabel3));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes[i].labels.Add(skipValidationLabel2);
                        codes[i].labels.Add(skipValidationLabel1);

                        Debug.Log($"[Transpiler Method4] Ending Modifying instruction at index {i}");

                        i = i + 8;
                        codes[i].labels.Add(skipValidationLabel3);
                        Debug.Log($"[Transpiler Method4] Inserting Br with label: skipValidationLabel3 at index {i}");
                        i = i - 8;

                        i++;
                    }
                }

                Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

            }

            return codes;

        }
    }

    public class ValidTrader
    {
        public static void CheckTrader(EMemberCategory Trader)
        {

            if (Trader.ToString() == "Trader")
            {
                AmmoPackDiscountConfig.IsValidTrader = true;
            }
            else
            {
                AmmoPackDiscountConfig.IsValidTrader = false;
            }

            Plugin.LogSource.LogDebug("IsValidTrader: " + AmmoPackDiscountConfig.IsValidTrader.ToString());

        }
    }

    public class ValidTemplate
    {
        public static void CheckTemplate(string Template)
        {

            if (Template.ToString() == "EFT.InventoryLogic.AmmoTemplate")
            {
                AmmoPackDiscountConfig.IsValidTemplate = true;
            }
            else
            {
                AmmoPackDiscountConfig.IsValidTemplate = false;
            }

            Plugin.LogSource.LogDebug("IsValidTemplate: " + AmmoPackDiscountConfig.IsValidTemplate.ToString());

        }
    }
}