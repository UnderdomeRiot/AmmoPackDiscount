using System;
using System.Text.RegularExpressions;
using underdomeriot_ammopackdiscount.Config;

namespace underdomeriot_ammopackdiscount.Helpers
{
    public class Calculation
    {
        public static int BulkDiscountStore(double unitPrice, int quantity)
        {
            // Check that the quantity is not zero to avoid incorrect calculations
            if (quantity == 0)
            {
                Plugin.LogSource.LogWarning("Invalid quantity (zero) input.");
                return 0; // Return 0 if the quantity is zero
            }

            // Calculate the original price based on the quantity and unit price
            AmmoPackDiscountConfig.OriginalPrice = (int)Math.Ceiling(unitPrice * quantity);
            int finalPrice = AmmoPackDiscountConfig.OriginalPrice;

            // Calculate the discount based on the quantity
            int quantityMultiple = quantity / AmmoPackDiscountConfig.QuantityThreshold;
            double discount = Math.Min(quantityMultiple * AmmoPackDiscountConfig.DiscountRate, AmmoPackDiscountConfig.MaxDiscount);

            // Register the applied discount percentage
            AmmoPackDiscountConfig.TotalDiscountPercentApplied = discount * 100;

            // Calculate the discount amount
            double discountAmount = AmmoPackDiscountConfig.OriginalPrice * discount;

            // Apply the discount if it's greater than zero
            if (discount > 0)
            {
                AmmoPackDiscountConfig.TotalDiscountApplied += discountAmount;
                finalPrice -= (int)discountAmount; // Subtract the discount from the final price
                Plugin.LogSource.LogWarning($"Applied {discount * 100}% discount based on quantity of {quantity}. Discount amount: {discountAmount}. Final price: {finalPrice}");
            }

            return finalPrice;
        }

        public static (int,string) BulkDiscountRagfair(int totalPrice, string dirtyQuantity)
        {
            // Remove non-numeric characters from the quantity
            bool parseSuccess = int.TryParse(Regex.Replace(dirtyQuantity, @"\D", ""), out int quantity);

            if (!parseSuccess || quantity == 0)
            {
                Plugin.LogSource.LogWarning("Invalid quantity input or zero quantity.");
                return (0,0.ToString()); // Return 0 if the quantity is invalid or zero
            }

            // Calculate the unit price
            double pricePerUnit = (double)totalPrice / quantity;

            // Log the unit price and quantity
            Plugin.LogSource.LogWarning($"Price per unit: {pricePerUnit}, Quantity: {quantity}");

            // Calculate the original price based on the quantity
            AmmoPackDiscountConfig.OriginalPrice = (int)Math.Ceiling(pricePerUnit * quantity);

            // Initialize the final price
            int finalPrice = AmmoPackDiscountConfig.OriginalPrice;

            // Calculate the discount based on the quantity
            int quantityMultiple = quantity / AmmoPackDiscountConfig.QuantityThreshold;
            double discount = Math.Min(quantityMultiple * AmmoPackDiscountConfig.DiscountRate, AmmoPackDiscountConfig.MaxDiscount);

            AmmoPackDiscountConfig.TotalDiscountPercentApplied = discount * 100;

            // Calculate the discount amount
            double discountAmount = AmmoPackDiscountConfig.OriginalPrice * discount;

            // Apply the discount if it's greater than zero
            if (discount > 0)
            {
                AmmoPackDiscountConfig.TotalDiscountApplied += discountAmount;
                finalPrice -= (int)discountAmount;
                Plugin.LogSource.LogWarning($"Applied {discount * 100}% discount based on quantity of {quantity}. Discount amount: {discountAmount}. Final price: {finalPrice}");
            }

            return (finalPrice,"Applying " + (discount * 100).ToString() + "% discount");
        }
    }
}
