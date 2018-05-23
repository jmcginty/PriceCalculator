using System.Collections.Generic;

namespace PriceCalculator
{
    public class Price
    {
        public string ProductType { get; set; }
        public Dictionary<string, List<string>> Options { get; set; }
        public int BasePrice { get; set; }
    }
}