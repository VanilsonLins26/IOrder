using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using Mapster;

namespace IOrder.Application.Services.Mapper;

public static class MapsterSettings
{

    public static void Configure()
    {
        TypeAdapterConfig<PromotionPrice, PromotionPriceResponse>
            .NewConfig()
            .Map(dest => dest.Active, src => src.IsActive());
    }
}