using Bogus;
using IOrder.Communication.Enums;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests;

public class RequestCreateProductBuilder
{
    public static ProductRequestDto Build()
    {
        return new Faker<ProductRequestDto>()
            .RuleFor(product => product.Name, (f) => f.Commerce.ProductName())
            .RuleFor(product => product.Price, (f) => decimal.Parse(f.Commerce.Price()))
            .RuleFor(product => product.UnitOfMeasure, (f) => f.PickRandom<UnitOfMeasure>())
            .RuleFor(product => product.Description, (f) => f.Commerce.ProductDescription())
            .RuleFor(product => product.ImageUrl, (f) => f.Image.PicsumUrl(800, 600))
            .RuleFor(product => product.Customizable, (f) => f.Random.Bool());


    }
}
