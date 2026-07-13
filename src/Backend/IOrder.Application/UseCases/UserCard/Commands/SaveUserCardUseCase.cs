using FluentValidation;
using IOrder.Application.Services.Payment;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Payment;
using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;

namespace IOrder.Application.UseCases.UserCard.Commands;

public class SaveUserCardUseCase : ISaveUserCardUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IProfileReadOnlyRepository _profileReadOnlyRepository;
    private readonly IProfileWriteOnlyRepository _profileWriteOnlyRepository;
    private readonly IUserCardWriteOnlyRepository _userCardWriteOnlyRepository;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;

    public SaveUserCardUseCase(
        ILoggedUserService loggedUserService,
        IProfileReadOnlyRepository profileReadOnlyRepository,
        IProfileWriteOnlyRepository profileWriteOnlyRepository,
        IUserCardWriteOnlyRepository userCardWriteOnlyRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork)
    {
        _loggedUserService = loggedUserService;
        _profileReadOnlyRepository = profileReadOnlyRepository;
        _profileWriteOnlyRepository = profileWriteOnlyRepository;
        _userCardWriteOnlyRepository = userCardWriteOnlyRepository;
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserCardResponseDto> Execute(SaveCardRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.CardToken))
            throw new ErrorOnValidationException(["Token do cartão não pode ser vazio."]);

        var userId = _loggedUserService.GetUserId();
        var userEmail = _loggedUserService.GetUserEmail();

        var profile = await _profileReadOnlyRepository.GetByUserId(userId)
            ?? throw new NotFoundException(["Perfil não encontrado."]);

        if (string.IsNullOrEmpty(profile.MercadoPagoCustomerId))
        {
            profile.MercadoPagoCustomerId = await _paymentService.GetOrCreateCustomerAsync(userEmail);
            var trackedProfile = await _profileWriteOnlyRepository.GetByUserIdTracking(userId);
            if (trackedProfile != null)
            {
                trackedProfile.MercadoPagoCustomerId = profile.MercadoPagoCustomerId;
                _profileWriteOnlyRepository.Update(trackedProfile);
            }
        }

        var savedCardDto = await _paymentService.SaveCardAsync(profile.MercadoPagoCustomerId, request.CardToken);

        var userCard = new Domain.Entities.UserCard
        {
            UserId = userId,
            GatewayCardId = savedCardDto.GatewayCardId,
            LastFourDigits = savedCardDto.LastFourDigits,
            Brand = savedCardDto.Brand,
            ExpirationMonth = savedCardDto.ExpirationMonth,
            ExpirationYear = savedCardDto.ExpirationYear
        };

        await _userCardWriteOnlyRepository.CreateAsync(userCard);
        await _unitOfWork.Commit();

        return new UserCardResponseDto
        {
            Id = userCard.Id,
            LastFourDigits = userCard.LastFourDigits,
            Brand = userCard.Brand,
            ExpirationMonth = userCard.ExpirationMonth,
            ExpirationYear = userCard.ExpirationYear,
            GatewayCardId = userCard.GatewayCardId
        };
    }
}
