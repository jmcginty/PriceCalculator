using System.Collections.Generic;

namespace PriceCalculator
{
    public class CartItem
    {
        public string ProductType { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public int ArtistMarkup { get; set; }
        public int Quantity { get; set; }
    }
}