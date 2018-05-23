# Price Calculator

This price calculator is a [dotnetcore](https://www.microsoft.com/net/learn/get-started/windows) project 

To install dotnetcore, download it from https://www.microsoft.com/net/download/

To run or test the program, open your command line or shell in the root folder of the project.

To run the tests: `dotnet test .\Tests`

To run the program: `dotnet run --project PriceCalculator cart.json prices.json`

For example to try one of the sample cart and price files: `dotnet run --project PriceCalculator .\Tests\json\cart-11356.json .\Tests\json\base-prices.json`

Prices of the items in the cart are calculated as follows..

For each item in the cart, 
    an attempt is made to find a matching price in the prices file.    

For a price to be a match, 
- The product type must match exactly.
- If any of the options of the cart item are listed in the product, they must match.
    e.g. if "colour" is listed in the cart item, and the price item has a "colour" option, the cart item "colour" must be listed in the price "colour" options or the price is excluded.

## As per instructions - these following assumptions are made
- No checking is done that the supplied files are valid json, or match the schema
- There is a base price for each product in the cart
