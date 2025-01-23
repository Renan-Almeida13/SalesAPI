using Domain.Commons;
using System.Net;

namespace Application.Validation
{
    public class ProductValidation
    {
        public static Response ValidateProductQuantities<T>(List<T> products, Func<T, int> quantitySelector, Func<T, string> productNameSelector)
        {
            foreach (var product in products)
            {
                var quantity = quantitySelector(product);
                if (quantity > 20)
                {
                    string errorMessage = $"Cannot sell more than 20 identical items. You tried to sell {quantity} items of '{productNameSelector(product)}'.";
                    return new Response(HttpStatusCode.BadRequest, new List<string> { errorMessage }, null);
                }
            }
            return null;
        }
    }
}
