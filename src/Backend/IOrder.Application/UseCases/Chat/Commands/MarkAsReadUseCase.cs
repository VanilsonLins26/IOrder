using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;

namespace IOrder.Application.UseCases.Chat.Commands;

public interface IMarkAsReadUseCase
{
    Task<int> Execute(Guid orderId);
}

public class MarkAsReadUseCase : IMarkAsReadUseCase
{
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IChatReadOnlyRepository _chatRepository;
    private readonly ILoggedUserService _loggedUserService;

    public MarkAsReadUseCase(
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IChatReadOnlyRepository chatRepository,
        ILoggedUserService loggedUserService)
    {
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _chatRepository = chatRepository;
        _loggedUserService = loggedUserService;
    }

    public async Task<int> Execute(Guid orderId)
    {
        var userId = _loggedUserService.GetUserId();

        await _orderWriteOnlyRepository.MarkMessagesAsReadAsync(orderId, userId);

        return await _chatRepository.GetUnreadCountAsync(orderId, userId);
    }
}
