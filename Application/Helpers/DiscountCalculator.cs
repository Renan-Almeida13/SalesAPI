namespace Application.Helpers
{
    public static class DiscountCalculator
    {
        public static decimal CalculateDiscount(int quantity, decimal unitPrice)
        {
            if (quantity >= 4 && quantity < 10)
                return quantity * unitPrice * 0.10m;
            if (quantity >= 10)
                return quantity * unitPrice * 0.20m;
            return 0;
        }
    }
}
