using HarmonyLib;
using System.Reflection.Emit;
using EFT.UI.Ragfair;
using System.Collections.Generic;
using UnityEngine;
using underdomeriot_ammopackdiscount;
using EFT.InventoryLogic;
using System.Reflection;
using underdomeriot_ammopackdiscount.Config;
using BepInEx.Logging;
using EFT.Hideout;
using System;
using Comfort.Common;
using System.Runtime.CompilerServices;
using static CW2.Animations.PhysicsSimulator.Val;
using System.Linq;

namespace underdomeriot_ammopackdiscount.Patches
{
    [HarmonyPatch(typeof(HandoverRagfairMoneyWindow), "Show")]
    public static class HandoverRagfairMoneyWindowShowPatch
    {
        static bool getOnlyDefault = false;

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new List<CodeInstruction>(instructions);

            // BUSCAR SI SOLO ES UN TRADER Y NO JUGADORES.
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode != null && codes[i].operand != null && !getOnlyDefault)
                {
                    if (codes[i].opcode == OpCodes.Stfld
                        && codes[i].operand.ToString().Contains("EFT.InventoryLogic.Item[] item_0")) // Instruction 49: Opcode=stfld, Operand=EFT.InventoryLogic.Item[] item_0
                    {

                        // Logging INI
                        Debug.Log($"[Transpiler Show] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");
                        Debug.Log($"[Transpiler Show] Initiating Modifying instruction at index {i}");

                        // Cargamos el THIS, ya que la siguiente iteracion IL esta guardada en la propia clase.
                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                        Debug.Log($"[Transpiler Show] Instruction {i + 1}: Opcode={codes[i + 1].opcode}, Operand={codes[i + 1].operand}");

                        // Cargamos en la pila el campo de la clase HandoverRagfairMoneyWindow con nombre ememberCategory_0.
                        codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldfld, typeof(HandoverRagfairMoneyWindow).GetField("ememberCategory_0", BindingFlags.Instance | BindingFlags.NonPublic)));
                        Debug.Log($"[Transpiler Show] Instruction {i + 2}: Opcode={codes[i + 2].opcode}, Operand={codes[i + 2].operand}");

                        // Ejecutamos una llamada a una clase+metodo, la clase se llama ValidTrader y el metodo Check
                        codes.Insert(i + 3, new CodeInstruction(OpCodes.Call, typeof(ValidTrader).GetMethod("CheckTrader", new[] { typeof(EMemberCategory) })));
                        Debug.Log($"[Transpiler Show] Instruction {i + 3}: Opcode={codes[i + 3].opcode}, Operand={codes[i + 3].operand}");

                        // Logging END
                        Debug.Log($"[Transpiler Show] Ending Modifying instruction at index {i}");

                        i += 4; // Cantidad de instrucciones añadidas.

                    }
                }

                // Logging total
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
            var Discount = generator.DeclareLocal(typeof(string)).LocalIndex; // Nueva variable.

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode != null && codes[i].operand != null)
                {

                    // BUSCAR EN EL PARAMETRO ITEM, QUE SOLO SE HA SELECCIONADO MUNICION.
                    // QUIERO INSERTAR DESPUES DE LO DETECTADO POR ESO EMPIEZO CON +1


                    if (codes[i].opcode == OpCodes.Stloc_S && codes[i].operand.ToString().Contains("System.String (31)"))
                    {
                        // Logging inicio 
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");
                        Debug.Log($"[Transpiler Method4] Initiating modification at index {i}");

                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldloca_S, 33));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 1}: Opcode={codes[i + 1].opcode}, Operand={codes[i + 1].operand}");

                        // Obtener la propiedad `Key` del `KeyValuePair` retornado por `FirstOrDefault`
                        codes.Insert(i + 2, new CodeInstruction(OpCodes.Call, typeof(KeyValuePair<GInterface195, int>).GetProperty("Key").GetGetMethod()));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 2}: Opcode={codes[i + 2].opcode}, Operand={codes[i + 2].operand}");

                        // Acceder a la propiedad `Item` de la clave (suponiendo que `Key` es de tipo `GInterface195`)
                        codes.Insert(i + 3, new CodeInstruction(OpCodes.Callvirt, typeof(GInterface195).GetProperty("Item").GetGetMethod()));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 3}: Opcode={codes[i + 3].opcode}, Operand={codes[i + 3].operand}");

                        // Llamar a item.Template
                        codes.Insert(i + 4, new CodeInstruction(OpCodes.Callvirt, typeof(Item).GetProperty("Template").GetGetMethod()));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 4}: Opcode={codes[i + 4].opcode}, Operand={codes[i + 4].operand}");

                        // Llamar a item.Template.GetType()
                        codes.Insert(i + 5, new CodeInstruction(OpCodes.Callvirt, typeof(object).GetMethod("GetType")));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 5}: Opcode={codes[i + 5].opcode}, Operand={codes[i + 5].operand}");

                        // Llamar a GetType().FullName
                        codes.Insert(i + 6, new CodeInstruction(OpCodes.Callvirt, typeof(System.Type).GetProperty("FullName").GetGetMethod()));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 6}: Opcode={codes[i + 6].opcode}, Operand={codes[i + 6].operand}");

                        // Llamar al metodo que comprueba si el template del objeto es valido.
                        codes.Insert(i + 7, new CodeInstruction(OpCodes.Call, typeof(ValidTemplate).GetMethod("CheckTemplate", new[] { typeof(string) })));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 7}: Opcode={codes[i + 7].opcode}, Operand={codes[i + 7].operand}");

                        // Definir un nuevo label para saltar
                        Label skipValidationLabel1 = generator.DefineLabel();
                        Label skipValidationLabel2 = generator.DefineLabel();

                        // Obtener la varible de los trader validos.
                        codes.Insert(i + 8, new CodeInstruction(OpCodes.Ldsfld, typeof(AmmoPackDiscountConfig).GetField("IsValidTrader", BindingFlags.Static | BindingFlags.Public)));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 8}: Opcode={codes[i + 8].opcode}, Operand={codes[i + 8].operand}");

                         // Hacer el IF (si es falso, lo salta)
                        codes.Insert(i + 9, new CodeInstruction(OpCodes.Brfalse_S, skipValidationLabel1));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 9}: Opcode={codes[i + 9].opcode}, Operand={codes[i + 9].operand}");

                        // Obtener la varible de los template validos.
                        codes.Insert(i + 10, new CodeInstruction(OpCodes.Ldsfld, typeof(AmmoPackDiscountConfig).GetField("IsValidTemplate", BindingFlags.Static | BindingFlags.Public)));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 10}: Opcode={codes[i + 10].opcode}, Operand={codes[i + 10].operand}");

                        // Hacer el IF (si es falso, lo salta)
                        codes.Insert(i + 11, new CodeInstruction(OpCodes.Brfalse_S, skipValidationLabel2));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 11}: Opcode={codes[i + 11].opcode}, Operand={codes[i + 11].operand}");

                        // Obtener el precio del objeto // int
                        codes.Insert(i + 12, new CodeInstruction(OpCodes.Ldloc_S, 29));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 12}: Opcode={codes[i + 12].opcode}, Operand={codes[i + 12].operand}");

                        // Obtener la cantidad de objetos // string
                        codes.Insert(i + 13, new CodeInstruction(OpCodes.Ldloc_S, 31)); 
                        Debug.Log($"[Transpiler Method4] Instruction {i + 13}: Opcode={codes[i + 13].opcode}, Operand={codes[i + 13].operand}");

                        // Llamar al calculo
                        codes.Insert(i + 14, new CodeInstruction(OpCodes.Call, typeof(Helpers.Calculation).GetMethod("BulkDiscountRagfair", new[] { typeof(int), typeof(string) })));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 14}: Opcode={codes[i + 14].opcode}, Operand={codes[i + 14].operand}");

                        // Guardar la tupla
                        var tupleLocalIndex = generator.DeclareLocal(typeof(ValueTuple<int, string>)).LocalIndex;  // Nueva variable.
                        codes.Insert(i + 15, new CodeInstruction(OpCodes.Stloc, tupleLocalIndex));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 15}: Opcode={codes[i + 15].opcode}, Operand={codes[i + 15].operand}");

                        // Cargar el campo 'Item1' (int) de la tupla
                        codes.Insert(i + 16, new CodeInstruction(OpCodes.Ldloca, tupleLocalIndex)); // Cargar referencia a la tupla
                        Debug.Log($"[Transpiler Method4] Instruction {i + 16}: Opcode={codes[i + 16].opcode}, Operand={codes[i + 16].operand}");

                        codes.Insert(i + 17, new CodeInstruction(OpCodes.Ldfld, typeof(ValueTuple<int, string>).GetField("Item1"))); // Campo Item1
                        Debug.Log($"[Transpiler Method4] Instruction {i + 17}: Opcode={codes[i + 17].opcode}, Operand={codes[i + 17].operand}");

                        // Guardar el campo 'Item1' en una variable local
                        codes.Insert(i + 18, new CodeInstruction(OpCodes.Stloc, 29));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 18}: Opcode={codes[i + 18].opcode}, Operand={codes[i + 18].operand}");

                        // Cargar el campo 'Item2' (double) de la tupla 
                        codes.Insert(i + 19, new CodeInstruction(OpCodes.Ldloca, tupleLocalIndex)); // Cargar referencia a la tupla
                        Debug.Log($"[Transpiler Method4] Instruction {i + 19}: Opcode={codes[i + 19].opcode}, Operand={codes[i + 19].operand}");

                        codes.Insert(i + 20, new CodeInstruction(OpCodes.Ldfld, typeof(ValueTuple<int, string>).GetField("Item2"))); // Campo Item2
                        Debug.Log($"[Transpiler Method4] Instruction {i + 20}: Opcode={codes[i + 20].opcode}, Operand={codes[i + 20].operand}");

                        // Guardar el campo 'Item2' en una variable local
                        codes.Insert(i + 21, new CodeInstruction(OpCodes.Stloc, Discount));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 21}: Opcode={codes[i + 21].opcode}, Operand={codes[i + 21].operand}");

                        codes.Insert(i + 22, new CodeInstruction(OpCodes.Ldloc, Discount));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 22}: Opcode={codes[i + 22].opcode}, Operand={codes[i + 22].operand}");

                        codes.Insert(i + 23, new CodeInstruction(OpCodes.Call,typeof(DebugHelpers).GetMethod("LogStackValue").MakeGenericMethod(typeof(string))));
                        Debug.Log($"[Transpiler Method4] Instruction {i + 23}: Opcode={codes[i + 23].opcode}, Operand={codes[i + 23].operand}");

                        // Label del FIN DEL IF
                        codes[i + 24].labels.Add(skipValidationLabel2);
                        codes[i + 24].labels.Add(skipValidationLabel1);


                        // Logging END
                        Debug.Log($"[Transpiler Method4] Ending Modifying instruction at index {i}");

                        i += 24; // Cantidad de instrucciones añadidas.

                    }
                    
                    if (codes[i].opcode == OpCodes.Ldstr && codes[i].operand.ToString().Contains("ragfair/Are you sure you want to buy {0} ({1}) for {2}?"))
                    {

                        Debug.Log($"[Transpiler Method4] Initiating modification at index {i}");

                        // Definir un nuevo label para saltar
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
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, 4));  // Cuatro posiciones
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Newarr, typeof(string))); // Crear un nuevo array de strings
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Dup));  // Duplicar el array
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, 0));  // Índice 0
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 30));  // Parámetro {0} - text
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stelem_Ref));  // Insertar en el array
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Dup));  // Duplicar el array
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, 1));  // Índice 1
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 31));  // Parámetro {1} - text2
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stelem_Ref));  // Insertar en el array
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Dup));  // Duplicar el array
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, 2));  // Índice 2
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 32));  // Parámetro {2} - text3
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stelem_Ref));  // Insertar en el array
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}"); 
                        
                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Dup));  // Duplicar el array
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, 3));  // Índice 3
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc, Discount));  // Parámetro {3} - Discount
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stelem_Ref));  // Insertar el valor como string
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, typeof(string).GetMethod("Format", new[] { typeof(string), typeof(string[]) })));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Stloc, 2));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldloc, 2));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, typeof(DebugHelpers).GetMethod("LogStackValue").MakeGenericMethod(typeof(string))));
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Br, skipValidationLabel3));  // Saltar al "else"
                        Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

                        i++;
                        codes[i].labels.Add(skipValidationLabel2);  // Etiqueta para saltar si la segunda condición es false
                        codes[i].labels.Add(skipValidationLabel1);  // Etiqueta para saltar si la primera condición es false

                        // LOG
                        Debug.Log($"[Transpiler Method4] Ending Modifying instruction at index {i}");

                        i = i + 8;
                        codes[i].labels.Add(skipValidationLabel3);
                        Debug.Log($"[Transpiler Method4] Inserting Br with label: skipValidationLabel3 at index {i}");
                        i = i - 8;

                        i++;
                    }
                }

                // Logging total
                Debug.Log($"[Transpiler Method4] Instruction {i}: Opcode={codes[i].opcode}, Operand={codes[i].operand}");

            }
            return codes;

        }
    }

    public class ValidTrader
    {
        public static void CheckTrader(EMemberCategory Trader)
        {

            //Plugin.LogSource.LogWarning("IsValidTraderAntes: " + AmmoPackDiscountConfig.IsValidTrader.ToString());

            if (Trader.ToString() == "Trader")
            {
                AmmoPackDiscountConfig.IsValidTrader = true;
            }
            else
            {
                AmmoPackDiscountConfig.IsValidTrader = false;
            }

            Plugin.LogSource.LogWarning("IsValidTraderDespues: " + AmmoPackDiscountConfig.IsValidTrader.ToString());

        }
    }

    public class ValidTemplate
    {
        public static void CheckTemplate(string Template)
        {

            //Plugin.LogSource.LogWarning("IsValidTemplateAntes: " + AmmoPackDiscountConfig.IsValidTemplate.ToString());

            if (Template.ToString() == "EFT.InventoryLogic.AmmoTemplate" || Template.ToString() == "EFT.InventoryLogic.AmmoBoxTemplate")
            {
                AmmoPackDiscountConfig.IsValidTemplate = true;
            }
            else
            {
                AmmoPackDiscountConfig.IsValidTemplate = false;
            }

            Plugin.LogSource.LogWarning("IsValidTemplateDespues: " + AmmoPackDiscountConfig.IsValidTemplate.ToString());

        }
    }

    public static class DebugHelpers
    {
        public static void LogStackValue<T>(T value)
        {            
            if (value is Array arr)
            {
                // Si es un array, hacer un log de todos los elementos
                string arrayValues = string.Join(", ", arr.Cast<object>().Select(x => x.ToString()).ToArray());
                Plugin.LogSource.LogWarning($"[Stack Debug] Array Values: {arrayValues} (Type: {typeof(T)})");
            }
            else
            {
                // Si no es un array, simplemente mostrar el valor
                Plugin.LogSource.LogWarning($"[Stack Debug] Value: {value} (Type: {typeof(T)})");
            }
        }
    }

}