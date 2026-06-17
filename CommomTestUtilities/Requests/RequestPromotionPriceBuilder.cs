using Bogus;
using IOrder.Communication.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommomTestUtilities.Requests;

public class RequestPromotionPriceBuilder
{
    public static PromotionPriceResquestDto Build()
    {
        return new Faker<PromotionPriceResquestDto>()
            .RuleFor(promotion => promotion.ProductId, Guid.CreateVersion7())
            .RuleFor(promotion => promotion.Price, (f) => decimal.Parse(f.Commerce.Price()))
            .RuleFor(promotion => promotion.InitialTime, (f) => f.Date.Soon(5))
            .RuleFor(p => p.FinalTime, (f, p) => f.Date.Soon(15, refDate: p.InitialTime));

    }
}
