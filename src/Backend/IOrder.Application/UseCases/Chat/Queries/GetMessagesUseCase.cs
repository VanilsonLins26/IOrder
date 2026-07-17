using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Order;
using Mapster;

namespace IOrder.Application.UseCases.Chat.Queries;

public interface IGetMessagesUseCase
{
    Task<PagedResponse<OrderMessageResponseDto>> Execute(Guid orderId, int pageNumber, int pageSize);
}

public class GetMessagesUseCase : IGetMessagesUseCase
{
    private readonly IChatReadOnlyRepository _chatRepository;

    public GetMessagesUseCase(IChatReadOnlyRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<PagedResponse<OrderMessageResponseDto>> Execute(Guid orderId, int pageNumber, int pageSize)
    {
        var messages = await _chatRepository.GetMessagesAsync(orderId, pageNumber, pageSize);
        var totalCount = await _chatRepository.GetMessagesCountAsync(orderId);

        return new PagedResponse<OrderMessageResponseDto>(
            messages.Adapt<List<OrderMessageResponseDto>>(), totalCount, pageNumber, pageSize);
    }
}
