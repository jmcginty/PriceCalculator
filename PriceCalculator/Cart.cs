using System.Collections.Generic;

namespace PriceCalculator
{
    public class Cart
    {
        public List<CartItem> Items { get; set; }
        public Cart()
        {
            Items = new List<CartItem>();
        }
    }
}