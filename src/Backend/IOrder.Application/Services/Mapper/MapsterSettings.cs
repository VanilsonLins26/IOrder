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

        TypeAdapterConfig<Domain.Entities.Order, OrderResponseDto>
            .NewConfig()
            .Map(dest => dest.Status, src => src.Status);

        TypeAdapterConfig<OrderItem, OrderItemResponseDto>
            .NewConfig()
            .Map(dest => dest.TotalPrice, src => src.TotalPrice);

        TypeAdapterConfig<OrderMessage, OrderMessageResponseDto>
            .NewConfig()
            .Map(dest => dest.Type, src => src.Type);
    }
}