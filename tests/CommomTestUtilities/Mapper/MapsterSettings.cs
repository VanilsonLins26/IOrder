using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommomTestUtilities.Mapper;

public class MapsterSettings
{
    public static void Configure()
    {
        TypeAdapterConfig<PromotionPrice, PromotionPriceResponseDto>
            .NewConfig()
            .Map(dest => dest.Active, src => src.IsActive());
    }
}
