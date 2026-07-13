namespace IOrder.Application.UseCases.UserCard.Commands;

public interface IDeleteUserCardUseCase
{
    Task Execute(Guid id);
}
