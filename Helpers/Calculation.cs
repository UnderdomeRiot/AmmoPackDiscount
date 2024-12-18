using System;
using System.Text.RegularExpressions;
using underdomeriot_ammopackdiscount.Config;

namespace underdomeriot_ammopackdiscount.Helpers
{
    public class Calculation
    {
        private static int ApplyBulkDiscount(int quantity, double unitPrice)
        {
            if (quantity == 0)
            {
                return 0;
            }

            AmmoPackDiscountConfig.OriginalPrice = (int)Math.Ceiling(unitPrice * quantity);
            int finalPrice = AmmoPackDiscountConfig.OriginalPrice;

            int quantityMultiple = quantity / AmmoPackDiscountConfig.QuantityThreshold;
            double discount = Math.Min(quantityMultiple * AmmoPackDiscountConfig.DiscountRate, AmmoPackDiscountConfig.MaxDiscount);

            AmmoPackDiscountConfig.TotalDiscountPercentApplied = discount * 100;
            double discountAmount = AmmoPackDiscountConfig.OriginalPrice * discount;

            if (discount > 0)
            {
                AmmoPackDiscountConfig.TotalDiscountApplied += discountAmount;
                finalPrice -= (int)discountAmount;
                Plugin.LogSource.LogDebug($"Applied {discount * 100}% discount based on quantity of {quantity}. Discount amount: {discountAmount}. Final price: {finalPrice}");
            }

            AmmoPackDiscountConfig.FinalPrice = finalPrice;
            return finalPrice;
        }

        public static int BulkDiscountStore(double unitPrice, int quantity)
        {
            return ApplyBulkDiscount(quantity, unitPrice);
        }

        public static (int, string) BulkDiscountRagfair(int totalPrice, string dirtyQuantity)
        {
            bool parseSuccess = int.TryParse(Regex.Replace(dirtyQuantity, @"\D", ""), out int quantity);

            if (!parseSuccess || quantity == 0)
            {
                return (0, "0");
            }

            double pricePerUnit = (double)totalPrice / quantity;
            int finalPrice = ApplyBulkDiscount(quantity, pricePerUnit);

            return (finalPrice, $"Applying {AmmoPackDiscountConfig.TotalDiscountPercentApplied}% discount");
        }
    }
}
