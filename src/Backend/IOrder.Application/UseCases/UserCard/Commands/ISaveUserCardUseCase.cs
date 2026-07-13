using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.UserCard.Commands;

public interface ISaveUserCardUseCase
{
    Task<UserCardResponseDto> Execute(SaveCardRequestDto request);
}
