using IOrder.Application.Services.Payment;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Security.Services;

namespace IOrder.Application.UseCases.UserCard.Queries;

public class GetUserCardsUseCase : IGetUserCardsUseCase
{
    private readonly IProfileReadOnlyRepository _profileReadOnlyRepository;
    private readonly IPaymentService _paymentService;
    private readonly ILoggedUserService _loggedUserService;

    public GetUserCardsUseCase(
        IProfileReadOnlyRepository profileReadOnlyRepository,
        IPaymentService paymentService,
        ILoggedUserService loggedUserService)
    {
        _profileReadOnlyRepository = profileReadOnlyRepository;
        _paymentService = paymentService;
        _loggedUserService = loggedUserService;
    }

    public async Task<IList<UserCardResponseDto>> Execute()
    {
        var userId = _loggedUserService.GetUserId();
        var profile = await _profileReadOnlyRepository.GetByUserId(userId);
        
        if (profile == null || string.IsNullOrEmpty(profile.StripeCustomerId))
        {
            return new List<UserCardResponseDto>();
        }

        var cards = await _paymentService.ListCardsAsync(profile.StripeCustomerId);

        return cards.Select(card => new UserCardResponseDto
        {
            Id = card.GatewayCardId,
            LastFourDigits = card.LastFourDigits,
            Brand = card.Brand,
            ExpirationMonth = card.ExpirationMonth,
            ExpirationYear = card.ExpirationYear,
            GatewayCardId = card.GatewayCardId
        }).ToList();
    }
}
