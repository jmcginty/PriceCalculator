using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PriceCalculator
{
    public class Calculator
    {
        public static void Process(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException(@"Usage: PriceCalculator.exe cart.json prices.json");
            }

            var cart = LoadCart(args[0]);
            var prices = LoadPrices(args[1]);

            var totalPrice = ApplyPricing(cart, prices);

            Console.WriteLine(totalPrice);
        }

        public static int ApplyPricing(Cart cart, List<Price> prices)
        {
            var totalPrice = 0;

            foreach(var item in cart.Items)
            {
                var price = 0;

                // Find all the price items which match my cart item
                foreach(var priceOption in prices.Where(p => p.ProductType == item.ProductType))
                {
                    var mismatch = false;
                    foreach(var option in item.Options)
                    {
                        if (priceOption.Options.ContainsKey(option.Key))
                        {
                            // must match 
                            if (!priceOption.Options[option.Key].Any(x => x == option.Value))
                            {
                                // This price option does not match
                                mismatch = true;
                                break;
                            }
                        }
                        else
                        {
                            // unknown option -- e.g. "print-location"
                        }
                    }

                    // If option not actively mismatched, then use it.
                    if (!mismatch)
                    {
                        price = priceOption.BasePrice;
                    }
                }

                totalPrice += LineTotal(price, item.ArtistMarkup, item.Quantity);
            }

            return totalPrice;
        }

        public static int LineTotal(int price, int artistMarkupPercent, int quantity)
        {
            decimal markUp = price * artistMarkupPercent / 100;
            return (price + (int)Math.Round(markUp)) * quantity;
        }

        public static List<Price> LoadPrices(string fileName)
        {
            var result = new List<Price>();

            using (var file = File.OpenText(fileName))
            using (var reader = new JsonTextReader(file))
            {
                var jsonPrices = JToken.ReadFrom(reader);
                foreach (var jsonPrice in jsonPrices)
                {
                    var options = new Dictionary<string, List<string>>();
                    foreach (JProperty option in jsonPrice["options"])
                    {
                        var optionValues = new List<string>();
                        foreach (JValue optionValue in option.Value)
                        {
                            optionValues.Add((string)optionValue);
                        }
                        options.Add(option.Name, optionValues);
                    }

                    result.Add(new Price()
                    {
                        ProductType = (string)jsonPrice["product-type"],
                        BasePrice = (int)jsonPrice["base-price"],
                        Options = options
                    });
                }
            }

            return result;
        }

        public static Cart LoadCart(string fileName)
        {
            var result = new Cart();

            using (var file = File.OpenText(fileName))
            using (var reader = new JsonTextReader(file))
            {
                var jsonCart = JToken.ReadFrom(reader);
                foreach (var cartItem in jsonCart)
                {
                    var options = new Dictionary<string, string>();
                    foreach (JProperty option in cartItem["options"])
                    {
                        options.Add(option.Name, (string)option.Value);
                    }

                    result.Items.Add(new CartItem()
                    {
                        ProductType = (string)cartItem["product-type"],
                        ArtistMarkup = (int)cartItem["artist-markup"],
                        Quantity = (int)cartItem["quantity"],
                        Options = options
                    });
                }
            }

            return result;
        }
    }
}
