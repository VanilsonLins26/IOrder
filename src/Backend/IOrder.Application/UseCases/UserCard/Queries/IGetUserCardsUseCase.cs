using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.UserCard.Queries;

public interface IGetUserCardsUseCase
{
    Task<IList<UserCardResponseDto>> Execute();
}
