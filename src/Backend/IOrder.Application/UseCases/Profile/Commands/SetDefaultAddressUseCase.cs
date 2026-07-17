using IOrder.Domain.Security.Services;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Profile;
using IOrder.Exceptions.ExceptionBase;

namespace IOrder.Application.UseCases.Profile.Commands;

public class SetDefaultAddressUseCase : ISetDefaultAddressUseCase
{
    private readonly ILoggedUserService _loggedUser;
    private readonly IUserAddressReadOnlyRepository _readRepository;
    private readonly IUserAddressWriteOnlyRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetDefaultAddressUseCase(
        ILoggedUserService loggedUser,
        IUserAddressReadOnlyRepository readRepository,
        IUserAddressWriteOnlyRepository writeRepository,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid addressId)
    {
        
        var userId = _loggedUser.GetUserId();

        var targetAddress = await _readRepository.GetByIdAsync(addressId);
        if (targetAddress == null || targetAddress.UserId != userId)
        {
            throw new NotFoundException(new List<string> { "Address not found." });
        }

        var currentDefault = await _readRepository.GetDefaultAddressAsync(userId);
        if (currentDefault != null && currentDefault.Id != addressId)
        {
            currentDefault.IsDefault = false;
            await _writeRepository.UpdateAsync(currentDefault);
        }

        targetAddress.IsDefault = true;
        await _writeRepository.UpdateAsync(targetAddress);

        await _unitOfWork.Commit();
    }
}
