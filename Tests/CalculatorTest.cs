using PriceCalculator;
using System;
using System.IO;
using Xunit;

namespace Tests
{
    public class CalculatorTest
    {
        const string basePrices = @"..\..\..\..\Tests\json\base-prices.json";
        const string cart11356 = @"..\..\..\..\Tests\json\cart-11356.json";
        const string cart4560 = @"..\..\..\..\Tests\json\cart-4560.json";
        const string cart9363 = @"..\..\..\..\Tests\json\cart-9363.json";
        const string cart9500 = @"..\..\..\..\Tests\json\cart-9500.json";

        const string testPrices = @"..\..\..\..\Tests\json\test-prices.json";
        const string testCart = @"..\..\..\..\Tests\json\test-cart.json";

        const string testPriceOptionMatch = @"..\..\..\..\Tests\json\test-priceOptionMatch.json";
        const string testCartOptionMatch = @"..\..\..\..\Tests\json\test-cartOptionMatch.json";

        [Fact]
        public void LessThanTwoArgumentsThrowsError()
        {
            var args = new string[1];
            var ex = Assert.Throws<ArgumentException>(() => Calculator.Process(args));
            Assert.Equal("Usage: PriceCalculator.exe cart.json prices.json", ex.Message);
        }

        [Fact]
        public void MoreThanTwoArgumentsThrowsError()
        {
            var args = new string[3];
            var ex = Assert.Throws<ArgumentException>(() => Calculator.Process(args));
            Assert.Equal("Usage: PriceCalculator.exe cart.json prices.json", ex.Message);
        }

        [Fact]
        public void CartFileNotFoundThrowsError()
        {
            var args = new string[2];
            args[0] = @"thisfiledoesnotexist.json";
            args[1] = testPrices;
            var ex = Assert.Throws<FileNotFoundException>(() => Calculator.Process(args));
        }

        [Fact]
        public void PriceFileNotFoundThrowsError()
        {
            var args = new string[2];
            args[0] = testCart;
            args[1] = @"thisfiledoesnotexist.json";
            var ex = Assert.Throws<FileNotFoundException>(() => Calculator.Process(args));
        }

        [Fact]
        public void CartIsInitialisedWithAnEmptyList()
        {
            var cart = new Cart();
            Assert.NotNull(cart.Items);
        }

        [Fact]
        public void ThePricesObjectIsCorrectlyLoadedFromJson()
        {
            var prices = Calculator.LoadPrices(testPrices);
            Assert.Equal(3, prices.Count);

            // Check product type with multiple options
            Assert.Equal("hoodie", prices[0].ProductType);
            Assert.Equal(3800, prices[0].BasePrice);
            Assert.Equal(2, prices[0].Options.Count);
            Assert.Equal("white", prices[0].Options["colour"][0]);
            Assert.Equal("dark", prices[0].Options["colour"][1]);

            Assert.Equal("small", prices[0].Options["size"][0]);
            Assert.Equal("medium", prices[0].Options["size"][1]);

            // Check product type with one option
            Assert.Equal("sticker", prices[1].ProductType);
            Assert.Equal(583, prices[1].BasePrice);
            Assert.Single(prices[1].Options);
            Assert.Equal("medium", prices[1].Options["size"][0]);

            // Check product type with no options
            Assert.Equal("leggings", prices[2].ProductType);
            Assert.Equal(5000, prices[2].BasePrice);
            Assert.Empty(prices[2].Options);
        }

        [Fact]
        public void TheCartObjectIsCorrectlyLoadedFromJson()
        {
            var cart = Calculator.LoadCart(testCart);
            Assert.Equal(2, cart.Items.Count);

            Assert.Equal("hoodie", cart.Items[0].ProductType);
            Assert.Equal(20, cart.Items[0].ArtistMarkup);
            Assert.Equal(1, cart.Items[0].Quantity);
            Assert.Equal(3, cart.Items[0].Options.Count);
            Assert.Equal("small", cart.Items[0].Options["size"]);
            Assert.Equal("white", cart.Items[0].Options["colour"]);
            Assert.Equal("front", cart.Items[0].Options["print-location"]);

            Assert.Equal("socks", cart.Items[1].ProductType);
            Assert.Equal(33, cart.Items[1].ArtistMarkup);
            Assert.Equal(2, cart.Items[1].Quantity);
            Assert.Equal(3, cart.Items[1].Options.Count);
            Assert.Equal("xxl", cart.Items[1].Options["size"]);
            Assert.Equal("red", cart.Items[1].Options["colour"]);
            Assert.Equal("side", cart.Items[1].Options["print-location"]);
        }

        // If a cart item has an option in the price file, then only prices matching the option are returned.
        // Our cart has a small, white, hoodie.
        // Size small is ignored. 
        // Colour White is matched with Price Option White.
        [Fact]
        public void IfACartItemHasAnOptionInThePriceFileItMustMatch()
        {
            var prices = Calculator.LoadPrices(testPriceOptionMatch);
            var cart = Calculator.LoadCart(testCartOptionMatch);

            var price = Calculator.ApplyPricing(cart, prices);

            Assert.Equal(1, price);
        }

        // Check calculation is (base_price + round(base_price * artist_markup%)) * quantity
        [Fact]
        public void ArtistsCalculationIsCorrect()
        {
            var basePrice = 8254;
            var artistMarkupPercent = 23;
            var quantity = 7;

            Assert.Equal(71064, Calculator.LineTotal(basePrice, artistMarkupPercent, quantity));
        }

        // Check the examples calculate correctly
        [Fact]
        public void PriceIsCalculatedCorrectly()
        {
            var prices = Calculator.LoadPrices(basePrices);

            Assert.Equal(11356, Calculator.ApplyPricing(Calculator.LoadCart(cart11356), prices));
            Assert.Equal(4560, Calculator.ApplyPricing(Calculator.LoadCart(cart4560), prices));
            Assert.Equal(9363, Calculator.ApplyPricing(Calculator.LoadCart(cart9363), prices));
            Assert.Equal(9500, Calculator.ApplyPricing(Calculator.LoadCart(cart9500), prices));
        }
    }
}
