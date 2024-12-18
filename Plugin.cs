using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using underdomeriot_ammopackdiscount.Patches;
using underdomeriot_ammopackdiscount.Config;

namespace underdomeriot_ammopackdiscount
{
    [BepInPlugin("underdomeriot.riot.ammo.pack.discount", "underdomeriot-AmmoPackDiscount", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;

        private void Awake()
        {
            LogSource = Logger;

            LogSource.LogDebug("[AmmoPackDiscount] loading...");

            //Config
            AmmoPackDiscountConfig.InitConfig(Config);

            //Prefix + Postfix
            new Method18Patch().Enable();
            new FormatPricePatch().Enable();
            new GClass2288Patch().Enable();
            new TraderAssortmentControllerClassPatch().Enable();
            new PurchasePatch().Enable();

            //Transpiler
            var harmony = new Harmony("underdomeriot.riot.ammo.pack.discount");
            harmony.PatchAll();

            LogSource.LogDebug("[AmmoPackDiscount] loaded!");
        }
    }
}
