using Bogus;
using IOrder.Communication.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommomTestUtilities.Requests.Product;

public class RequestPromotionPriceBuilder
{
    public static PromotionPriceResquestDto Build()
    {
        return new Faker<PromotionPriceResquestDto>()
            .RuleFor(promotion => promotion.ProductId, Guid.CreateVersion7())
            .RuleFor(promotion => promotion.Price, 0.000001m)
            .RuleFor(promotion => promotion.InitialTime, (f) => DateTime.UtcNow.AddDays(1))
            .RuleFor(p => p.FinalTime, (f, p) => DateTime.UtcNow.AddDays(1));

    }
}
