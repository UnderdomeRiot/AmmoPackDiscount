using BepInEx.Configuration;
using System;

namespace underdomeriot_ammopackdiscount.Config
{
    internal static class AmmoPackDiscountConfig
    {
        private const string ConfigSectionTitle = "Config";

        // logic check vars
        public static bool IsValidTemplate = false;             // Variable to control product type.
        public static bool IsValidCurrencyAndAmount = false;    // Variable to control currency validation.
        public static bool IsValidTrader = false;               // Variable to control ragfair trader.

        // calculation vars
        public static int QuantityThreshold = 30;               // Global variable to control the quantity threshold for discount (30 by default)
        public static double DiscountRate = 0.05;               // Global variable to control the discount rate (5% by default)
        public static double MaxDiscount = 0.25;                // Maximum discount (25% by default)

        // string UI vats
        public static int OriginalPrice = 0;                    // Variable to store the original price
        public static double TotalDiscountApplied = 0;          // Variable to store the total discount applied
        public static double TotalDiscountPercentApplied = 0;   // Variable to store the total discount applied by percentage.

        // config temp vars
        public static ConfigEntry<int> ConfQuantityThreshold;
        public static ConfigEntry<double> ConfDiscountRate;
        public static ConfigEntry<double> ConfMaxDiscount;

        public static void InitConfig(ConfigFile config)
        {

            ConfQuantityThreshold = config.Bind(ConfigSectionTitle, "Quantity Threshold", 30,
                new ConfigDescription(
                    "Items you need to buy to get a single 'Discount Rate' percentage, 30 means you need to buy 30 bullets"
                )
            );

            ConfDiscountRate = config.Bind(ConfigSectionTitle, "Discount Rate", 0.05,
                new ConfigDescription(
                    "Discount percentage for each 'Quantity Threshold', 0.05 means 5% discount, don't set 100% (1)",
                    new AcceptableValueRange<double>(0, 1)
                )
            );

            ConfMaxDiscount = config.Bind(ConfigSectionTitle, "Max Discount", 0.25,
                new ConfigDescription(
                    "Maximum applicable discount, 0.25 means 25%, don't set 100% (1)",
                    new AcceptableValueRange<double>(0,1)
                )                
            );

            ConfQuantityThreshold.SettingChanged += SettingChanged;
            ConfDiscountRate.SettingChanged += SettingChanged;
            ConfMaxDiscount.SettingChanged += SettingChanged;
        }

        private static void SettingChanged(object sender, EventArgs e)
        {
            QuantityThreshold = ConfQuantityThreshold.Value;
            DiscountRate = ConfDiscountRate.Value;
            MaxDiscount = ConfMaxDiscount.Value;
        }
    }
}