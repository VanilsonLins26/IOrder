using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Payment;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions.ExceptionBase;

namespace IOrder.Application.UseCases.UserCard.Commands;

public class DeleteUserCardUseCase : IDeleteUserCardUseCase
{
    private readonly IUserCardReadOnlyRepository _userCardReadOnlyRepository;
    private readonly IUserCardWriteOnlyRepository _userCardWriteOnlyRepository;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCardUseCase(
        IUserCardReadOnlyRepository userCardReadOnlyRepository,
        IUserCardWriteOnlyRepository userCardWriteOnlyRepository,
        ILoggedUserService loggedUserService,
        IUnitOfWork unitOfWork)
    {
        _userCardReadOnlyRepository = userCardReadOnlyRepository;
        _userCardWriteOnlyRepository = userCardWriteOnlyRepository;
        _loggedUserService = loggedUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(Guid id)
    {
        var userId = _loggedUserService.GetUserId();

        var card = await _userCardReadOnlyRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(["Cartão não encontrado."]);

        if (card.UserId != userId)
            throw new UnauthorizedStoreException(["Cartão inválido."]);

        _userCardWriteOnlyRepository.Delete(card);
        await _unitOfWork.Commit();
    }
}
