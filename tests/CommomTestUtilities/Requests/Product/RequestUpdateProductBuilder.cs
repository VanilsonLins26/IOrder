using Bogus;
using IOrder.Communication.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommomTestUtilities.Requests.Product;

public class RequestUpdateProductBuilder
{
    public static UpdateProductRequestDto Build()
    {
        return new Faker<UpdateProductRequestDto>()
            .RuleFor(product => product.Name, (f) => f.Commerce.ProductName())
            .RuleFor(product => product.Price, (f) => decimal.Parse(f.Commerce.Price()))
            .RuleFor(product => product.Description, (f) => f.Commerce.ProductDescription())
            .RuleFor(product => product.ImageUrl, (f) => f.Image.PicsumUrl(800, 600))
            .RuleFor(product => product.Customizable, (f) => f.Random.Bool());


    }
}

