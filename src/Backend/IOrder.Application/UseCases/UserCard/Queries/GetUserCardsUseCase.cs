using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Payment;
using IOrder.Domain.Security.Services;

namespace IOrder.Application.UseCases.UserCard.Queries;

public class GetUserCardsUseCase : IGetUserCardsUseCase
{
    private readonly IUserCardReadOnlyRepository _userCardReadOnlyRepository;
    private readonly ILoggedUserService _loggedUserService;

    public GetUserCardsUseCase(
        IUserCardReadOnlyRepository userCardReadOnlyRepository,
        ILoggedUserService loggedUserService)
    {
        _userCardReadOnlyRepository = userCardReadOnlyRepository;
        _loggedUserService = loggedUserService;
    }

    public async Task<IList<UserCardResponseDto>> Execute()
    {
        var userId = _loggedUserService.GetUserId();
        var cards = await _userCardReadOnlyRepository.GetByUserIdAsync(userId);

        return cards.Select(card => new UserCardResponseDto
        {
            Id = card.Id,
            LastFourDigits = card.LastFourDigits,
            Brand = card.Brand,
            ExpirationMonth = card.ExpirationMonth,
            ExpirationYear = card.ExpirationYear,
            GatewayCardId = card.GatewayCardId
        }).ToList();
    }
}
