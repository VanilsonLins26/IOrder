using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Security.Services;

namespace IOrder.Application.UseCases.Chat.Queries;

public interface IGetConversationsUseCase
{
    Task<PagedResponse<ConversationResponseDto>> Execute(int pageNumber, int pageSize);
}

public class GetConversationsUseCase : IGetConversationsUseCase
{
    private readonly IChatReadOnlyRepository _chatRepository;
    private readonly IStoreReadOnlyRepository _storeRepository;
    private readonly ILoggedUserService _loggedUserService;

    public GetConversationsUseCase(
        IChatReadOnlyRepository chatRepository,
        IStoreReadOnlyRepository storeRepository,
        ILoggedUserService loggedUserService)
    {
        _chatRepository = chatRepository;
        _storeRepository = storeRepository;
        _loggedUserService = loggedUserService;
    }

    public async Task<PagedResponse<ConversationResponseDto>> Execute(int pageNumber, int pageSize)
    {
        var userId = _loggedUserService.GetUserId();
        var store = await _storeRepository.GetByUserIdAsync(userId);
        var storeUserId = store?.UserId;

        var summaries = await _chatRepository.GetConversationsAsync(userId, storeUserId, pageNumber, pageSize);
        var totalCount = await _chatRepository.GetConversationsCountAsync(userId, storeUserId);

        var conversations = summaries.Select(s => new ConversationResponseDto
        {
            OrderId = s.OrderId,
            StoreId = s.StoreId,
            StoreName = s.StoreName,
            StoreImageUrl = s.StoreImageUrl,
            Status = s.Status,
            LastMessage = s.LastMessage,
            LastMessageAt = s.LastMessageAt,
            LastMessageByRole = s.LastMessageByRole,
            UnreadCount = s.UnreadCount
        }).ToList();

        return new PagedResponse<ConversationResponseDto>(conversations, totalCount, pageNumber, pageSize);
    }
}
