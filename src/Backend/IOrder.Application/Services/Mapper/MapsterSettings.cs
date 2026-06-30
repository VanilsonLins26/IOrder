using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using Mapster;

namespace IOrder.Application.Services.Mapper;

public static class MapsterSettings
{

    public static void Configure()
    {
        TypeAdapterConfig<PromotionPrice, PromotionPriceResponseDto>
            .NewConfig()
            .Map(dest => dest.Active, src => src.IsActive());

        TypeAdapterConfig<Store, StoreResponseDto>
            .NewConfig()
            .Map(dest => dest.IsOpen, src => src.IsOpen());
    }
}