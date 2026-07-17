using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Order;
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
            .Map(dest => dest.IsOpen, src => src.IsOpen())
            .Map(dest => dest.Latitude, src => src.Location != null ? src.Location.X : (double?)null)
            .Map(dest => dest.Longitude, src => src.Location != null ? src.Location.Y : (double?)null);

        TypeAdapterConfig<OrderItem, OrderItemResponseDto>
            .NewConfig()
            .Map(dest => dest.TotalPrice, src => src.TotalPrice);

        TypeAdapterConfig<OrderMessage, OrderMessageResponseDto>
            .NewConfig()
            .Map(dest => dest.Type, src => src.Type);

        TypeAdapterConfig<ConversationSummary, ConversationResponseDto>
            .NewConfig()
            .TwoWays();

        TypeAdapterConfig<Domain.Entities.Payment, PaymentResponseDto>
            .NewConfig()
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Method, src => src.Method);

        TypeAdapterConfig<DeliveryAssignment, DeliveryAssignmentResponseDto>
            .NewConfig()
            .Map(dest => dest.Status, src => (Communication.Enums.AssignmentStatusDto)(int)src.Status);

        TypeAdapterConfig<CourierLocation, CourierLocationResponseDto>
            .NewConfig();

        TypeAdapterConfig<Domain.Entities.Order, OrderResponseDto>
            .NewConfig()
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.DeliveryType, src => (Communication.Enums.DeliveryTypeDto)(int)src.DeliveryType)
            .Map(dest => dest.DeliveryFee, src => src.DeliveryFee)
            .Map(dest => dest.ActiveAssignment, src => src.ActiveAssignment);
    }
}