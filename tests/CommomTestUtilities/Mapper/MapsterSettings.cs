using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Order;
using Mapster;

namespace CommomTestUtilities.Mapper;

public class MapsterSettings
{
    public static void Configure()
    {
        TypeAdapterConfig<PromotionPrice, PromotionPriceResponseDto>
            .NewConfig()
            .Map(dest => dest.Active, src => src.IsActive());

        TypeAdapterConfig<OrderMessage, OrderMessageResponseDto>
            .NewConfig()
            .Map(dest => dest.Type, src => src.Type);

        TypeAdapterConfig<ConversationSummary, ConversationResponseDto>
            .NewConfig();
    }
}
