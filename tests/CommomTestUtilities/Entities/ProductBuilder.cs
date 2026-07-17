using Bogus;
using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommomTestUtilities.Entities;

public class ProductBuilder
{

    public static Product Build(Guid? storeId = null)
    {
        var product = new Faker<Product>()
            .RuleFor(product => product.Id, Guid.CreateVersion7())
            .RuleFor(product => product.StoreId, storeId ?? Guid.NewGuid())
            .RuleFor(product => product.Name, (f) => f.Commerce.ProductName())
            .RuleFor(product => product.Price, (f) => decimal.Parse(f.Commerce.Price()))
            .RuleFor(product => product.UnitOfMeasure, (f) => f.PickRandom<UnitOfMeasure>())
            .RuleFor(product => product.Description, (f) => f.Commerce.ProductDescription())
            .RuleFor(product => product.ImageUrl, (f) => f.Image.PicsumUrl(800, 600))
            .RuleFor(product => product.Customizable, (f) => f.Random.Bool());

        return product;

    }
}
