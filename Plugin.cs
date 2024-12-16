using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using underdomeriot_ammopackdiscount.Patches;
using underdomeriot_ammopackdiscount.Config;

// hacer los porcentajes. 
// los literales de ragfair, añadir el % de descuento
// poner los logs en ingles.
// refactorizar
// icono
// descripcion del mod
// subir

// Este es el plugin principal que carga los parches

namespace underdomeriot_ammopackdiscount
{
    [BepInPlugin("underdomeriot.riot.ammo.pack.discount", "underdomeriot-AmmoPackDiscount", "1.0.0")]
    [BepInDependency("com.SPT.core", "3.10.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;

        private void Awake()
        {
            LogSource = Logger;  // Inicialización del LogSource

            LogSource.LogWarning("[AmmoPackDiscount] loading...");

            //Config
            AmmoPackDiscountConfig.InitConfig(Config);

            //Prefix + Postfix
            new Method18Patch().Enable();
            new FormatPricePatch().Enable();
            new GClass2288Patch().Enable();
            new TraderAssortmentControllerClassPatch().Enable();

            //Transpiler
            var harmony = new Harmony("underdomeriot.riot.ammo.pack.discount");
            harmony.PatchAll();  // Aplica todos los parches definidos

            LogSource.LogWarning("[AmmoPackDiscount] loaded!");
        }
    }
}
