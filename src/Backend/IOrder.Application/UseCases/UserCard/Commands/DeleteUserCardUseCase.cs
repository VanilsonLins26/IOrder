using IOrder.Application.Services.Payment;
using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions.ExceptionBase;

namespace IOrder.Application.UseCases.UserCard.Commands;

public class DeleteUserCardUseCase : IDeleteUserCardUseCase
{
    private readonly IProfileReadOnlyRepository _profileReadOnlyRepository;
    private readonly IPaymentService _paymentService;
    private readonly ILoggedUserService _loggedUserService;

    public DeleteUserCardUseCase(
        IProfileReadOnlyRepository profileReadOnlyRepository,
        IPaymentService paymentService,
        ILoggedUserService loggedUserService)
    {
        _profileReadOnlyRepository = profileReadOnlyRepository;
        _paymentService = paymentService;
        _loggedUserService = loggedUserService;
    }

    public async Task Execute(string id)
    {
        var userId = _loggedUserService.GetUserId();
        var profile = await _profileReadOnlyRepository.GetByUserId(userId)
            ?? throw new NotFoundException(["Perfil não encontrado."]);

        if (string.IsNullOrEmpty(profile.StripeCustomerId))
            throw new NotFoundException(["Cliente Stripe não encontrado."]);

        await _paymentService.DeleteCardAsync(profile.StripeCustomerId, id);
    }
}
